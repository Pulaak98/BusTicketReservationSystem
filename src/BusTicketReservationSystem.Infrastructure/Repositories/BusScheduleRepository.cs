using BusTicketReservationSystem.Application.Contracts.Interfaces;
using BusTicketReservationSystem.Domain.Entities;
using BusTicketReservationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Infrastructure.Repositories
{
    public class BusScheduleRepository : IBusScheduleRepository
    {
        private readonly AppDbContext _db;
        public BusScheduleRepository(AppDbContext db) => _db = db;

        public Task<BusSchedule?> GetByIdAsync(Guid id) =>
            _db.BusSchedules
                .Include(s => s.Tickets)
               .Include(x => x.Bus)
               .Include(x => x.Route)
               .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IReadOnlyList<BusSchedule>> SearchAsync(string from, string to, DateTime journeyDate)
        {
            return await _db.BusSchedules
                .Include(x => x.Bus)
                .Include(x => x.Route)
                .Where(x => x.Route.FromCity.ToLower() == from &&
                            x.Route.ToCity.ToLower() == to &&
                            x.JourneyDate.Date == journeyDate.Date)
                .OrderBy(x => x.StartTime)
                .ToListAsync();
        }

    }

}
