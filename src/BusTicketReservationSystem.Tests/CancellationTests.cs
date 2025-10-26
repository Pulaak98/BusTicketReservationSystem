using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Tests
{
    using BusTicketReservationSystem.Application.Contracts.DTOs;
    using BusTicketReservationSystem.Application.Contracts.Interfaces;
    using BusTicketReservationSystem.Application.Services;
    using BusTicketReservationSystem.Domain.Entities;
    using BusTicketReservationSystem.Domain.Enums;
    using BusTicketReservationSystem.Domain.ValueObjects;
    using BusTicketReservationSystem.Infrastructure.Data;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class CancellationTests
    {
        [Fact]
        public async Task CancelAsync_ShouldMarkTicketAsCancelled_WhenTicketIsValid()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var seatId = Guid.NewGuid();
            var passengerId = Guid.NewGuid();
            var scheduleId = Guid.NewGuid();

            var seat = new Seat(new SeatPosition("A1", 1)) { Id = seatId, Status = SeatStatus.Booked };
            var ticket = new Ticket
            {
                Id = ticketId,
                SeatId = seatId,
                Seat = seat,
                PassengerId = passengerId,
                BusScheduleId = scheduleId,
                Status = TicketStatus.Booked
            };

            var input = new CancelInputDto { TicketId = ticketId };

            var mockTicketRepo = new Mock<ITicketRepository>();
            var mockUow = new Mock<IUnitOfWork>();

            mockTicketRepo.Setup(r => r.GetByIdAsync(ticketId)).ReturnsAsync(ticket);
            mockUow.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            var service = new BookingService(
                Mock.Of<IBusScheduleRepository>(),
                Mock.Of<ISeatRepository>(),
                mockTicketRepo.Object,
                Mock.Of<IPassengerRepository>(),
                mockUow.Object
            );

            // Act
            var result = await service.CancelAsync(input);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Booking cancelled.", result.Message);
            Assert.Equal(TicketStatus.Cancelled, ticket.Status);
            Assert.Equal(SeatStatus.Available, seat.Status);
        }

        [Fact]
        public async Task CancelTicket_ShouldUpdateStatusAndFreeSeat()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var seatId = Guid.NewGuid();
            var passengerId = Guid.NewGuid();
            var scheduleId = Guid.NewGuid();

            var seat = new Seat(new SeatPosition("A1", 1)) { Id = seatId, Status = SeatStatus.Booked };
            var ticket = new Ticket
            {
                Id = ticketId,
                SeatId = seatId,
                Seat = seat,
                PassengerId = passengerId,
                BusScheduleId = scheduleId,
                Status = TicketStatus.Booked
            };

            var input = new CancelInputDto { TicketId = ticketId };

            var mockTicketRepo = new Mock<ITicketRepository>();
            var mockUow = new Mock<IUnitOfWork>();

            mockTicketRepo.Setup(r => r.GetByIdAsync(ticketId)).ReturnsAsync(ticket);
            mockUow.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            var service = new BookingService(
                Mock.Of<IBusScheduleRepository>(),
                Mock.Of<ISeatRepository>(),
                mockTicketRepo.Object,
                Mock.Of<IPassengerRepository>(),
                mockUow.Object
            );

            // Act
            var result = await service.CancelAsync(input);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Booking cancelled.", result.Message);
            Assert.Equal(TicketStatus.Cancelled, ticket.Status);
            Assert.Equal(SeatStatus.Available, seat.Status);
        }


    }

}
