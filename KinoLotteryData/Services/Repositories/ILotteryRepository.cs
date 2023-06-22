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
        //Task<List<Lottery>> GetLotteriesByIdAsync(List<int> ids);
        void UpdateLotteryWithMoneyPlayerandWon(Lottery lottery);
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
                
                return a.Entity;
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.Message.ToString());
                throw new Exception(ex.Message);
            }
        }
        public void UpdateLotteryWithMoneyPlayerandWon(Lottery lottery)
        {
            if (lottery == null)
                throw new ArgumentNullException(nameof(lottery));

            try
            {
                _context.Entry(lottery).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message.ToString());
                throw new Exception(ex.Message);
            }
        }

        //public async Task<List<Lottery>> GetLotteriesByIdAsync(List<int> ids)
        //{
        //    if(ids.Count == 0)
        //    {

        //    }

        //    return await _context.Lotteries.Where(l => ids.Contains(l.Id)).ToListAsync();
        //}
    }
}
