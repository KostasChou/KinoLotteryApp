using KinoLotteryData.Data;
using KinoLotteryData.Data.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KinoLotteryData.Services.Repositories
{
    public interface ILotteryRepository
    {
        Task<int> CreateLotteryAsync(Lottery lottery);
    }

    public class LotteryRepository : ILotteryRepository
    {
        private readonly KinoLotteryContext _context;
        private readonly ILogger _logger;
        public LotteryRepository(KinoLotteryContext context, ILogger<LotteryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<int> CreateLotteryAsync(Lottery lottery)
        {
            if(lottery == null)
                throw new ArgumentNullException(nameof(lottery));
            try
            {
                var a = await _context.Lotteries.AddAsync(lottery);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Lottery created successfully.");
                return a.Entity.Id;
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.Message.ToString());
                throw new Exception(ex.Message);
            }
            
        }
    }
}
