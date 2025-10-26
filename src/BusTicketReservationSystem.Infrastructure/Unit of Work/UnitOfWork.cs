using BusTicketReservationSystem.Application.Contracts.Interfaces;
using BusTicketReservationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTicketReservationSystem.Infrastructure.Unit_of_Work
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        private IDbContextTransaction? _tx;

        public UnitOfWork(AppDbContext db) => _db = db;

        public async Task BeginTransactionAsync() =>
            _tx = await _db.Database.BeginTransactionAsync();

        public Task SaveChangesAsync() => _db.SaveChangesAsync();

        public async Task CommitAsync()
        {
            if (_tx != null) await _tx.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            if (_tx != null) await _tx.RollbackAsync();
        }
    }

}
