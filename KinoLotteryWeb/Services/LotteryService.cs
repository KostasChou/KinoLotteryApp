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
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    if (DateTime.Now.TimeOfDay < TimeSpan.FromHours(9))
                    {
                        //32400 seconds => 9:00:00 AM
                        var initialTime = TimeSpan.FromHours(9) - DateTime.Now.TimeOfDay;

                        if (initialTime <= TimeSpan.FromMinutes(1))
                        {
                            _logger.LogInformation("First Lottery of the day And also call lottery random number method");
                            var winningNumbers = RandomNumberGenerator();
                            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                        }
                        else
                        {
                            await Task.Delay(initialTime - TimeSpan.FromMinutes(1), stoppingToken);
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
                                _logger.LogInformation("NumberService is running maaaaaaaan");
                                var winningNumbers = RandomNumberGenerator();
                                int newLotteryId = 0;
                                do
                                {
                                    var lotteryRepository = scope.ServiceProvider.GetRequiredService<ILotteryRepository>();
                                    newLotteryId = await lotteryRepository.CreateLotteryAsync(new Lottery
                                    {
                                        LotteryDateTime = DateTime.Now.AddMinutes(5 - (DateTime.Now.Minute % 5)),
                                        WinningNumbers = String.Join(',', winningNumbers)
                                    });
                                } while (newLotteryId == 0);
                                

                                //AssignLotteryToTickets(scope);
                                var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
                                var activeTicketIds = await ticketRepository.GetActiveTicketsAsync();

                                _logger.LogInformation("activeTicketIds: " + activeTicketIds.ToString());

                                var lotteryTicketRepository = scope.ServiceProvider.GetRequiredService<ILotteryTicketRepository>();
                                await lotteryTicketRepository.CreateLotteryTicketAsync(activeTicketIds, newLotteryId);


                                _logger.LogInformation("Line 75 4 mins delay");
                                await Task.Delay(TimeSpan.FromMinutes(4), stoppingToken);
                            }
                            else
                            {
                                _logger.LogInformation("Line 80 1 min delay");
                                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                            }
                        }
                    }
                }
            }
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
            Array.Sort(randomNumbers);
            //foreach (var item in randomNumbers)
            //{
            //    _logger.LogInformation("The magic number is " + item.ToString());
            //}
            return randomNumbers;
        }

    }
}
