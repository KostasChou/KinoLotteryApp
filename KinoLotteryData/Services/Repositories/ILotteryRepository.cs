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
    public interface ILotteryRepository
    {
        Task<Lottery> CreateLotteryAsync(Lottery lottery);
        string GetLotteryNumbersById(int lotteryId);
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
        public async Task<Lottery> CreateLotteryAsync(Lottery lottery)
        {
            if(lottery == null)
                throw new ArgumentNullException(nameof(lottery));
            try
            {
                var a = await _context.Lotteries.AddAsync(lottery);
                await _context.SaveChangesAsync();
                //_logger.LogInformation("Lottery created successfully.");

                //_logger.LogInformation($"Part 2 lottery created FINISHED {DateTime.Now.Second} + {DateTime.Now.Millisecond}");
                return a.Entity;
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.Message.ToString());
                throw new Exception(ex.Message);
            }
        }
        public string GetLotteryNumbersById(int lotteryId)
        {
            return _context.Lotteries.Where(x => x.Id == lotteryId).Select(x => x.WinningNumbers).FirstOrDefault();
                //_context.Lotteries.OrderByDescending(x => x.Id).Select(x => x.WinningNumbers).FirstOrDefault().ToString();
            //if ((DateTime.Now.Minute % 5 == 0 && DateTime.Now.Second >= 0) &&
            //    (DateTime.Now.Minute % 5 == 0 && DateTime.Now.Second < 1))
        }
    }
}
