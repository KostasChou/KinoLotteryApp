using KinoLotteryData.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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
                int[] allLotteryNumbers = GetAllLotteryNumbers(winningNumbersString);

                foreach (int number in allLotteryNumbers)
                    await lotteryHub.SendLotteryNumbers(number);
            }
        }

        private int[] GetAllLotteryNumbers(string winningNumbersString)
        {
            int[] allLotteryNumbers = new int[60];
            int[] onlyWinningNumbers = winningNumbersString.Split(',').Select(int.Parse).ToArray();

            Random rnd = new Random();
            int tempNumb1 = 0;
            //int tempNumber = 0;
            for (int i = 1; i <= 60; i++)
            {
                if (i % 3 == 0)
                    allLotteryNumbers[i] = onlyWinningNumbers[i / 3];
                else
                {
                    int tempNumb2 = rnd.Next(1, 81);
                    while (tempNumb1 == tempNumb2)
                    {
                        tempNumb2 = rnd.Next(1, 81);
                        _logger.LogInformation(tempNumb2.ToString());
                    }
                }
                
                
            }

            return allLotteryNumbers;
        }
    }
}
