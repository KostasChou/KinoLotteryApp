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
                //_logger.LogInformation($"Part 4 tickets STARTED {DateTime.Now}");
                //var nextInterval = now.AddHours(8 - now.Hour).AddMinutes(59 - now.Minute).AddSeconds(59 - now.Second).AddMilliseconds(999);
                //var nextInterval = now.AddMinutes(5 - (now.Minute % 5));
                var activeTickets = await _context.Tickets.Where(x => x.RemainingLotteries > 0 && (lotteryDateTime - x.DateTimeCreated).TotalSeconds > 29).ToListAsync();
                foreach (var ticket in activeTickets)
                {
                    ticket.RemainingLotteries--;
                }
                await _context.SaveChangesAsync();

                //_logger.LogInformation("active tickets retrieved successfully");

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
