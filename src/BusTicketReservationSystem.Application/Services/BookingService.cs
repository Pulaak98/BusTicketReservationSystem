using BusTicketReservationSystem.Application.Contracts.DTOs;
using BusTicketReservationSystem.Application.Contracts.Interfaces;
using BusTicketReservationSystem.Domain.DomainService;
using BusTicketReservationSystem.Domain.Entities;
using BusTicketReservationSystem.Domain.Enums;
using BusTicketReservationSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Application.Services
{
    public class BookingService
    {
        private readonly IBusScheduleRepository _schedules;
        private readonly ISeatRepository _seats;
        private readonly ITicketRepository _tickets;
        private readonly IPassengerRepository _passengers;
        private readonly IUnitOfWork _uow;

        public BookingService(
            IBusScheduleRepository schedules,
            ISeatRepository seats,
            ITicketRepository tickets,
            IPassengerRepository passengers,
            IUnitOfWork uow)
        {
            _schedules = schedules;
            _seats = seats;
            _tickets = tickets;
            _passengers = passengers;
            _uow = uow;
        }

        public async Task<SeatPlanDto> GetSeatPlanAsync(Guid busScheduleId)
        {
            var schedule = await _schedules.GetByIdAsync(busScheduleId);

            if (schedule is null)
                return new SeatPlanDto { BusScheduleId = busScheduleId };

            var busSeats = await _seats.GetByBusIdAsync(schedule.BusId);

            return new SeatPlanDto
            {
                BusScheduleId = schedule.Id,
                BusName = schedule.Bus.BusName,
                CompanyName = schedule.Bus.CompanyName,
                BoardingPoint = schedule.Route.BoardingPoint,
                DroppingPoint = schedule.Route.DroppingPoint,
                Seats = busSeats.Select(s => new SeatDto
                {
                    SeatId = s.Id,
                    SeatNumber = s.SeatNumber,
                    Row = s.Row,
                    Status = s.Status.ToString()
                }).ToList()
            };
        }

        public async Task<BookSeatResultDto> BookSeatAsync(BookSeatInputDto input)
        {
            if (input.SeatId == null || input.SeatId.Count == 0)
                return new BookSeatResultDto { Success = false, Message = "Select at least one seat." };

            var schedule = await _schedules.GetByIdAsync(input.BusScheduleId);
            if (schedule is null)
                return new BookSeatResultDto { Success = false, Message = "Schedule not found." };

            await _uow.BeginTransactionAsync();
            try
            {
                var passenger = await _passengers.FindByMobileAsync(input.PassengerMobile)
                                ?? new Passenger(new PassengerContact(input.PassengerName, input.PassengerMobile),
                                                 input.BoardingPoint, input.DroppingPoint);

                if (passenger.Id == Guid.Empty)
                    await _passengers.AddAsync(passenger);

                var seats = await _seats.GetByIdsAsync(input.SeatId);
                var bookedDtos = new List<SeatDto>();

                foreach (var seat in seats)
                {
                    SeatStateService.EnsureCanBook(seat);

                    if (input.Action.ToLower() == "buy")
                    {
                        SeatStateService.Sell(seat);
                    }
                    else
                    {
                        SeatStateService.Book(seat);
                    }

                    var existingTicket = await _tickets.FindBySeatAndScheduleAsync(seat.Id, schedule.Id);

                    if (existingTicket != null && existingTicket.Status == TicketStatus.Cancelled)
                    {
                       
                        existingTicket.Status = input.Action.ToLower() == "buy" ? TicketStatus.Sold : TicketStatus.Booked;
                        existingTicket.PassengerId = passenger.Id;
                        existingTicket.BoardingPoint = input.BoardingPoint;
                        existingTicket.DroppingPoint = input.DroppingPoint;
                     
                    }
                    else
                    {
                        var ticket = new Ticket
                        {
                            Id = Guid.NewGuid(),
                            SeatId = seat.Id,
                            PassengerId = passenger.Id,
                            BusScheduleId = schedule.Id,
                            Status = input.Action.ToLower() == "buy" ? TicketStatus.Sold : TicketStatus.Booked,
                            BoardingPoint = input.BoardingPoint,
                            DroppingPoint = input.DroppingPoint
                        };
                        await _tickets.AddAsync(ticket);
                    }

                    bookedDtos.Add(new SeatDto
                    {
                        SeatId = seat.Id,
                        SeatNumber = seat.SeatNumber,
                        Row = seat.Row,
                        Status = seat.Status.ToString()
                    });
                }

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                return new BookSeatResultDto
                {
                    Success = true,
                    Message = "Seats booked successfully.",
                    Seat = bookedDtos
                };
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return new BookSeatResultDto { Success = false, Message = ex.Message };
            }
        }

        public async Task<BookSeatResultDto> CancelAsync(CancelInputDto input)
        {
            var ticket = await _tickets.GetByIdAsync(input.TicketId);
            if (ticket == null)
                return new BookSeatResultDto { Success = false, Message = "Ticket not found." };

            if (ticket.Status == TicketStatus.Booked || ticket.Status == TicketStatus.Sold)
            {
                SeatStateService.Cancel(ticket.Seat);
                ticket.Status = TicketStatus.Cancelled;
                await _uow.SaveChangesAsync();
                return new BookSeatResultDto { Success = true, Message = "Booking cancelled." };
            }

            return new BookSeatResultDto { Success = false, Message = "Ticket not cancellable." };
        }
    }

}
