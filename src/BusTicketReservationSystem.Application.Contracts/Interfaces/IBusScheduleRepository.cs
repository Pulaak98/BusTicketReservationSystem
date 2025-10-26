using BusTicketReservationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Application.Contracts.Interfaces
{
    public interface IBusScheduleRepository
    {
        Task<BusSchedule?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<BusSchedule>> SearchAsync(string from, string to, DateTime journeyDate);
    }
}
