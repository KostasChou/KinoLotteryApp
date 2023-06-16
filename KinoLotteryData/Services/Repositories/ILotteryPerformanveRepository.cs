using KinoLotteryData.Data;
using KinoLotteryData.Data.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinoLotteryData.Services.Repositories
{
    public interface ILotteryPerformanveRepository
    {
        LotteryPerformance[] GetLotteryPerformance();
    }

    public class LotteryPerformanveRepository : ILotteryPerformanveRepository
    {
        private readonly KinoLotteryContext _context;
        private readonly ILogger _logger;
        public LotteryPerformanveRepository(KinoLotteryContext context, ILogger<LotteryPerformanveRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public LotteryPerformance[] GetLotteryPerformance()
        {
            return _context.LotteryPerformances.ToArray();
        }
    }
}
