using KinoLotteryData.Data;
using KinoLotteryData.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KinoLotteryData.Services.Repositories
{
    public interface ITicketRepository
    {
        Task CreateTicketAsync(Ticket ticket);
    }

    public class TicketRepository : ITicketRepository
    {
        private readonly KinoLotteryContext _context;
        public TicketRepository(KinoLotteryContext context)
        {
            _context = context;
        }

        public Task CreateTicketAsync(Ticket ticket)
        {
            if(ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            try
            {
                _context.Tickets.Add(ticket);
                return _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
