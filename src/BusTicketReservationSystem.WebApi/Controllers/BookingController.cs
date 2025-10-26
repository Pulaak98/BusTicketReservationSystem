using BusTicketReservationSystem.Application.Contracts.DTOs;
using BusTicketReservationSystem.Application.Services;
using BusTicketReservationSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketReservationSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _booking;
        public BookingController(BookingService booking) => _booking = booking;

        [HttpGet("seatplan/{scheduleId}")]
        public async Task<IActionResult> GetSeatPlan(Guid scheduleId)
        {
            var result = await _booking.GetSeatPlanAsync(scheduleId);
            return Ok(result);
        }

        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] BookSeatInputDto input)
        {
            input.Action = "book";
            var result = await _booking.BookSeatAsync(input);
            return Ok(result);
        }

        [HttpPost("buy")]
        public async Task<IActionResult> Buy([FromBody] BookSeatInputDto input)
        {
            input.Action = "buy";
            var result = await _booking.BookSeatAsync(input);
            return Ok(result);
        }



        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel([FromBody] CancelInputDto input)
        {
            var result = await _booking.CancelAsync(input);
            return Ok(result);
        }
    }

}
