using KinoLotteryData.Data;
using KinoLotteryData.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinoLotteryData.Services.Repositories
{
    public interface ITicketRepository
    {
        Task<int> CreateTicketAsync(Ticket ticket);

        Task<List<Ticket>> GetActiveTicketsAsync(DateTime lotteryDateTime);
    }

    public class TicketRepository : ITicketRepository
    {
        private readonly KinoLotteryContext _context;
        private readonly ILogger _logger;
        public TicketRepository(KinoLotteryContext context, ILogger<TicketRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> CreateTicketAsync(Ticket ticket)
        {
            if(ticket == null)
            {
                _logger.LogInformation("ticket is null.");
                throw new ArgumentNullException(nameof(ticket));
            }
            try
            {
                _context.Tickets.Add(ticket);
                return await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.Message.ToString());
                throw new Exception(ex.Message);

            }
        }

        //pass the lotteryDateTime as an argument to get tickets that created 30s before the minute the lottery was created (e.g hh:04:30)
        public async Task<List<Ticket>> GetActiveTicketsAsync(DateTime lotteryDateTime)
        {
            try
            {

                var timeThreshold = lotteryDateTime.AddSeconds(-29);
                var activeTickets = await _context.Tickets
                    .Where(t => t.RemainingLotteries > 0 && t.DateTimeCreated < timeThreshold)
                    .ToListAsync();
                foreach (var ticket in activeTickets)
                {
                    ticket.RemainingLotteries--;
                }
                await _context.SaveChangesAsync();
                return activeTickets;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message.ToString());
                return null;
            }
        }


    }
}
