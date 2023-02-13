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
        private readonly ILogger<SendLotteryToFrontService> _logger;
        private readonly IServiceProvider _serviceProvider;
        public SendLotteryToFrontService(ILogger<SendLotteryToFrontService> logger, IServiceProvider serviceProvider)
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

                //implementation of run every five minutes
                var now = DateTime.Now;
                var nextInterval = now.AddMinutes(5 - (now.Minute % 5));
                nextInterval = nextInterval.AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
                var waitTime = (int)(nextInterval - now).TotalMilliseconds;
                await Task.Delay(waitTime, stoppingToken);

                foreach (int number in allLotteryNumbers)
                {
                    await lotteryHub.SendLotteryNumbers(number);
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }

        private int[] GetAllLotteryNumbers(string winningNumbersString)
        {
            int[] allLotteryNumbers = new int[60];
            int[] onlyWinningNumbers = winningNumbersString.Split(',').Select(int.Parse).ToArray();

            Random rnd = new Random();
            int tempNumb1 = 0;
            //int tempNumber = 0;
            for (int i = 0; i < 60; i++)
            {
                if ((i + 1) % 3 == 0)
                    allLotteryNumbers[i] = onlyWinningNumbers[i / 3];
                else if ((i + 1) % 3 == 1)
                {
                    tempNumb1 = rnd.Next(1, 81);
                    allLotteryNumbers[i] = tempNumb1;
                }
                else
                {
                    int tempNumb2 = rnd.Next(1, 81);
                    while (tempNumb1 == tempNumb2)
                    {
                        tempNumb2 = rnd.Next(1, 81);
                    }
                    allLotteryNumbers[i] = tempNumb2;
                }
            }

            return allLotteryNumbers;
        }
    }
}
