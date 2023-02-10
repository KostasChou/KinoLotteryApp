using KinoLotteryData.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KinoLotteryWeb.Services
{
    public class SendLotteryToFrontService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        public SendLotteryToFrontService(ILogger<LotteryService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                
                ILotteryRepository lotteryRepository = scope.ServiceProvider.GetRequiredService<ILotteryRepository>();
                string winningNumbersString = lotteryRepository.GetLotteryNumbers();
                
                LotteryHub lotteryHub = scope.ServiceProvider.GetRequiredService<LotteryHub>();
                
                await lotteryHub.SendLotteryNumbers(winningNumbers);
            }
        }

        private int[] GetAllLotteryNumbers(string winningNumbersString)
        {
            int[] allLotteryNumbers = new int[60];


            return allLotteryNumbers;
        }
    }
}
