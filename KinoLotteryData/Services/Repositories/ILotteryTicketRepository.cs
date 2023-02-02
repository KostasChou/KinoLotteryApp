using KinoLotteryData.Data;
using KinoLotteryData.Data.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KinoLotteryData.Services.Repositories
{
    
    public interface ILotteryTicketRepository
    {
        public Task CreateLotteryTicketAsync(List<int> activeTicketIds, int newLotteryId);
    }

    public class LotteryTicketRepository : ILotteryTicketRepository
    {
        private readonly KinoLotteryContext _context;
        private readonly ILogger _logger;
        public LotteryTicketRepository(KinoLotteryContext context, ILogger<LotteryTicketRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task CreateLotteryTicketAsync(List<int> activeTicketIds, int newLotteryId)
        {
            try
            {
                LotteryTicket[] lotteryTickets = new LotteryTicket[activeTicketIds.Count];
                for (var i = 0; i < lotteryTickets.Length; i++)
                {
                    lotteryTickets[i] = new LotteryTicket { LotteryId = newLotteryId, TicketId = activeTicketIds[i] };
                }


                await _context.LotteryTickets.AddRangeAsync(lotteryTickets);

                await _context.SaveChangesAsync();
                _logger.LogInformation("TicketLottery created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message.ToString());
            }
            
        }
    }
}
