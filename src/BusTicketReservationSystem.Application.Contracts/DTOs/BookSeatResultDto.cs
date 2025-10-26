using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Application.Contracts.DTOs
{
    public class BookSeatResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<SeatDto?> Seat { get; set; } = new();
    }
}
