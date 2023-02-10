using KinoLotteryData.Data.Entities;
using KinoLotteryData.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KinoLotteryWeb.Services
{
    public class LotteryService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        public LotteryService(ILogger<LotteryService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /*The first Lottery of the day is at 9.00am and the last is at 23.55pm. The goal for every lottery is to be created (in this service) within a range
         * of 4.00 min and let's say approximately 4.31 so it has time to be stored in the database and then the controller gets the new lottery in time in
         * order to be displayed in the UI. After the lottery is stored (in the lottery method) then the service find all the tickets that should be related 
         * with the new lottery and assign them the lottery Id (a proccess that realistically takes a lot of time due to the huge ammount of tickets that could 
         * have been played, thus happens also in lottery method but after the new lottery has been stored successfully). First Lottery is happening at 9pm. If 
         * the DateTime.Now is earlier than 9.00 but later than 8.59 then the creation of the first lottery takes place. If it is earlier than 8.59 then the 
         * time is checked again after 30s. If the lottery happens for the first time then the time is checked again after 4 mins. And these delays are not stantard
         * when a lottery takes place then we delay the task for 4 mins. When the time is checked again we delay it for 30s. The delay of this task is 4 insead
         * of 5 mins in order to avoid any loss of time due to tasks that take a lot of time and we have not predict. */

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                
                if (DateTime.Now.TimeOfDay < TimeSpan.FromHours(9))
                {
                    var initialTime = TimeSpan.FromHours(9) - DateTime.Now.TimeOfDay;
                    if (initialTime <= TimeSpan.FromMinutes(1))
                    {
                        _logger.LogInformation("First Lottery of the day line 42 ");
                        LotteryMethod();
                        await Task.Delay(TimeSpan.FromMinutes(4), stoppingToken);
                    }
                    else
                    {
                        await Task.Delay(initialTime - TimeSpan.FromSeconds(30), stoppingToken);
                    }
                }
                else
                {
                    if (DateTime.Now.TimeOfDay > new TimeSpan(23, 55, 0))
                    {
                        await Task.Delay(new TimeSpan(8, 55, 0), stoppingToken);
                    }
                    else
                    {
                        if (DateTime.Now.Minute % 5 == 4)
                        {
                            LotteryMethod();
                            _logger.LogInformation("Line 62 4 mins delay");
                            await Task.Delay(TimeSpan.FromMinutes(4), stoppingToken);
                        }
                        else
                        {
                            _logger.LogInformation("Line 67 1 min delay");
                            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                        }
                    }
                }
            }
        }

        private async void LotteryMethod()
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            _logger.LogInformation("NumberService is running maaaaaaaan");
            var winningNumbers = RandomNumberGenerator();
            int newLotteryId = 0;
            do
            {
                var lotteryRepository = scope.ServiceProvider.GetRequiredService<ILotteryRepository>();
                newLotteryId = await lotteryRepository.CreateLotteryAsync(new Lottery
                {
                    LotteryDateTime = DateTime.Now.AddMinutes(5 - (DateTime.Now.Minute % 5)),
                    WinningNumbers = String.Join(',', winningNumbers),
                    HasBeenShownToUI = false
                });
            } while (newLotteryId == 0);

            var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
            var activeTicketIds = await ticketRepository.GetActiveTicketsAsync();

            _logger.LogInformation("activeTicketIds: " + activeTicketIds.ToString());

            var lotteryTicketRepository = scope.ServiceProvider.GetRequiredService<ILotteryTicketRepository>();
            await lotteryTicketRepository.CreateLotteryTicketAsync(activeTicketIds, newLotteryId);
        }

        private int[] RandomNumberGenerator()
        {
            int[] randomNumbers = new int[20];
            for (int i = 0; i < randomNumbers.Length; i++)
            {
                Random rnd = new Random();
                var x = rnd.Next(1, 81);
                do
                {
                    x = rnd.Next(1, 81);

                } while (Array.Exists(randomNumbers, element => element == x));
                randomNumbers[i] = x;
            }
            return randomNumbers;
        }

    }
}
