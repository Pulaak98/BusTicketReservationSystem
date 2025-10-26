using BusTicketReservationSystem.Application.Services;
using BusTicketReservationSystem.Domain.Entities;
using BusTicketReservationSystem.Domain.Enums;
using BusTicketReservationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Tests
{
    using Xunit;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BusTicketReservationSystem.Application.Services;
    using BusTicketReservationSystem.Application.Contracts.DTOs;
    using BusTicketReservationSystem.Application.Contracts.Interfaces;
    using BusTicketReservationSystem.Domain.Entities;
    using BusTicketReservationSystem.Domain.Enums;
    using BusTicketReservationSystem.Domain.ValueObjects;

    public class BookingTests
    {
        private readonly Mock<IBusScheduleRepository> _mockScheduleRepo = new();
        private readonly Mock<ISeatRepository> _mockSeatRepo = new();
        private readonly Mock<ITicketRepository> _mockTicketRepo = new();
        private readonly Mock<IPassengerRepository> _mockPassengerRepo = new();
        private readonly Mock<IUnitOfWork> _mockUow = new();

        private readonly BookingService _bookingService;

        public BookingTests()
        {
            _bookingService = new BookingService(
                _mockScheduleRepo.Object,
                _mockSeatRepo.Object,
                _mockTicketRepo.Object,
                _mockPassengerRepo.Object,
                _mockUow.Object
            );
        }

        [Fact]
        public async Task BookSeatAsync_ShouldReturnSuccess_WhenValidInputProvided()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var seatId = Guid.NewGuid();
            var passengerId = Guid.NewGuid();

            var input = new BookSeatInputDto
            {
                BusScheduleId = scheduleId,
                SeatId = new List<Guid> { seatId },
                PassengerName = "Test Passenger",
                PassengerMobile = "01700000000",
                BoardingPoint = "Dhaka",
                DroppingPoint = "Mymensingh",
                Action = "book"
            };

            var schedule = new BusSchedule
            {
                Id = scheduleId,
                BusId = Guid.NewGuid(),
                RouteId = Guid.NewGuid(),
                JourneyDate = DateTime.Today,
                StartTime = DateTime.Now,
                ArrivalTime = DateTime.Now.AddHours(4),
                Bus = new Bus { BusName = "GreenLine", CompanyName = "GreenLine Ltd." },
                Route = new Route { BoardingPoint = "Dhaka", DroppingPoint = "Mymensingh" }
            };

            var seat = new Seat(new SeatPosition("A1", 1))
            {
                Id = seatId,
                BusId = schedule.BusId,
                Status = SeatStatus.Available
            };

            var passenger = new Passenger(new PassengerContact(input.PassengerName, input.PassengerMobile), input.BoardingPoint, input.DroppingPoint)
            {
                Id = passengerId
            };

            _mockScheduleRepo.Setup(r => r.GetByIdAsync(scheduleId)).ReturnsAsync(schedule);
            _mockSeatRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<Seat> { seat });
            _mockPassengerRepo.Setup(r => r.FindByMobileAsync(input.PassengerMobile)).ReturnsAsync(passenger);
            _mockTicketRepo.Setup(r => r.AddAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _bookingService.BookSeatAsync(input);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Seats booked successfully.", result.Message);
            Assert.Single(result.Seat);
            Assert.Equal(seatId, result.Seat[0]?.SeatId);
            Assert.Equal("Booked", result.Seat[0]?.Status);
        }

        [Fact]
        public async Task BookSeatAsync_ShouldFail_WhenSeatAlreadyBooked()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var seatId = Guid.NewGuid();

            var input = new BookSeatInputDto
            {
                BusScheduleId = scheduleId,
                SeatId = new List<Guid> { seatId },
                PassengerName = "Test Passenger",
                PassengerMobile = "01700000000",
                BoardingPoint = "Dhaka",
                DroppingPoint = "Mymensingh",
                Action = "book"
            };

            var schedule = new BusSchedule { Id = scheduleId, BusId = Guid.NewGuid(), Route = new Route() };
            var seat = new Seat(new SeatPosition("A1", 1)) { Id = seatId, BusId = schedule.BusId, Status = SeatStatus.Booked };

            _mockScheduleRepo.Setup(r => r.GetByIdAsync(scheduleId)).ReturnsAsync(schedule);
            _mockSeatRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<Seat> { seat });
            _mockPassengerRepo.Setup(r => r.FindByMobileAsync(input.PassengerMobile)).ReturnsAsync((Passenger)null);

            // Act
            var result = await _bookingService.BookSeatAsync(input);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("already booked", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task BookSeatAsync_ShouldFail_WhenNoSeatsSelected()
        {
            // Arrange
            var input = new BookSeatInputDto
            {
                BusScheduleId = Guid.NewGuid(),
                SeatId = new List<Guid>(), // empty
                PassengerName = "Test Passenger",
                PassengerMobile = "01700000000",
                BoardingPoint = "Dhaka",
                DroppingPoint = "Mymensingh",
                Action = "book"
            };

            // Act
            var result = await _bookingService.BookSeatAsync(input);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Select at least one seat.", result.Message);
        }

        [Fact]
        public async Task BookSeatAsync_ShouldMarkSeatAsSold_WhenActionIsBuy()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var seatId = Guid.NewGuid();
            var passengerId = Guid.NewGuid();

            var input = new BookSeatInputDto
            {
                BusScheduleId = scheduleId,
                SeatId = new List<Guid> { seatId },
                PassengerName = "Test Passenger",
                PassengerMobile = "01700000000",
                BoardingPoint = "Dhaka",
                DroppingPoint = "Mymensingh",
                Action = "buy"
            };

            var schedule = new BusSchedule
            {
                Id = scheduleId,
                BusId = Guid.NewGuid(),
                Route = new Route { BoardingPoint = "Dhaka", DroppingPoint = "Mymensingh" },
                Bus = new Bus { BusName = "GreenLine", CompanyName = "GreenLine Ltd." }
            };

            var seat = new Seat(new SeatPosition("A1", 1)) { Id = seatId, BusId = schedule.BusId, Status = SeatStatus.Available };
            var passenger = new Passenger(new PassengerContact(input.PassengerName, input.PassengerMobile), input.BoardingPoint, input.DroppingPoint) { Id = passengerId };

            _mockScheduleRepo.Setup(r => r.GetByIdAsync(scheduleId)).ReturnsAsync(schedule);
            _mockSeatRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<Seat> { seat });
            _mockPassengerRepo.Setup(r => r.FindByMobileAsync(input.PassengerMobile)).ReturnsAsync(passenger);
            _mockTicketRepo.Setup(r => r.AddAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _bookingService.BookSeatAsync(input);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Seats booked successfully.", result.Message);
            Assert.Equal("Sold", result.Seat[0]?.Status);
        }

        [Fact]
        public async Task BookSeatAsync_ShouldMarkSeatAsBooked_WhenActionIsBook()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var seatId = Guid.NewGuid();
            var passengerId = Guid.NewGuid();

            var input = new BookSeatInputDto
            {
                BusScheduleId = scheduleId,
                SeatId = new List<Guid> { seatId },
                PassengerName = "Test Passenger",
                PassengerMobile = "01700000000",
                BoardingPoint = "Dhaka",
                DroppingPoint = "Mymensingh",
                Action = "book"
            };

            var schedule = new BusSchedule
            {
                Id = scheduleId,
                BusId = Guid.NewGuid(),
                Route = new Route { BoardingPoint = "Dhaka", DroppingPoint = "Mymensingh" },
                Bus = new Bus { BusName = "GreenLine", CompanyName = "GreenLine Ltd." }
            };

            var seat = new Seat(new SeatPosition("A1", 1))
            {
                Id = seatId,
                BusId = schedule.BusId,
                Status = SeatStatus.Available
            };

            var passenger = new Passenger(new PassengerContact(input.PassengerName, input.PassengerMobile), input.BoardingPoint, input.DroppingPoint)
            {
                Id = passengerId
            };

            _mockScheduleRepo.Setup(r => r.GetByIdAsync(scheduleId)).ReturnsAsync(schedule);
            _mockSeatRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<Seat> { seat });
            _mockPassengerRepo.Setup(r => r.FindByMobileAsync(input.PassengerMobile)).ReturnsAsync(passenger);
            _mockTicketRepo.Setup(r => r.AddAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _bookingService.BookSeatAsync(input);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Seats booked successfully.", result.Message);
            Assert.Equal("Booked", result.Seat[0]?.Status);
        }




    }


}
