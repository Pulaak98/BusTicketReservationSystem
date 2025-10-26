using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Application.Contracts.DTOs
{
    public class BookSeatInputDto
    {
        public Guid BusScheduleId { get; set; }
        public List<Guid> SeatId { get; set; } = new();
        public string PassengerName { get; set; } = string.Empty;
        public string PassengerMobile { get; set; } = string.Empty;
        public string BoardingPoint { get; set; } = string.Empty;
        public string DroppingPoint { get; set; } = string.Empty;
        public string Action { get; set; } = "book"; 
    }
}
