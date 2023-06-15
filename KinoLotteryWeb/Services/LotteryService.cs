using KinoLotteryData.Data.Entities;
using KinoLotteryData.Services.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KinoLotteryWeb.Services
{
    public class LotteryService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        internal static List<int> numbersShownToUI = new List<int>();
        private LotteryPerformance[] _performances = new LotteryPerformance[85];
        public LotteryService(ILogger<LotteryService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        //In this Method only actions related with time are implemented, the rest is inside the LotteryMethod()
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                
                if (DateTime.Now.TimeOfDay < TimeSpan.FromHours(9))
                {
                    var now = DateTime.Now;
                    var nextInterval = now.AddHours(8 - now.Hour).AddMinutes(59 - now.Minute).AddSeconds(59 - now.Second).AddMilliseconds(999);

                    await Task.Delay(nextInterval.TimeOfDay, stoppingToken);
                    await LotteryMethod(stoppingToken);

                    //_logger.LogInformation($"First Lottery of the day line 35 {DateTime.Now}");

                }
                else
                {
                    var now = DateTime.Now;
                    var nextInterval = now.AddMinutes(5 - (now.Minute % 5));
                    nextInterval = nextInterval.AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
                    var waitTime = (int)(nextInterval - now).TotalMilliseconds;

                    //_logger.LogInformation($"var waitTime: {waitTime}");

                    //_logger.LogInformation($"BEFORE TASK DELAY AND LOTTERYMETHOD {DateTime.Now}");
                    //_logger.LogInformation($"WAITING TIME: {waitTime}");
                    await Task.Delay(waitTime, stoppingToken);
                    await LotteryMethod(stoppingToken);

                    //_logger.LogInformation($"AFTER TASK DELAY AND LOTTERYMETHOD {DateTime.Now}");


                    //the last lottery of the day at 23:55
                    if(DateTime.Now.TimeOfDay > new TimeSpan(23, 55, 00))
                        await Task.Delay(TimeSpan.FromHours(9), stoppingToken);
                }
                //_logger.LogInformation($"END OF WHILE LOOP{DateTime.Now}");
            }
        }

        private async Task LotteryMethod(CancellationToken stoppingToken)
        {
            
            using IServiceScope scope = _serviceProvider.CreateScope();
            //First part: Request a true random nambor from an api and seed to to Random Class to create the random winning numbers.
            var randomNumberSeed = await GetTrueRandomSeedNumber();
            var winningNumbers = RandomNumberGenerator(randomNumberSeed);

            //Second part: Store the newly created lottery in the database and do not leave the loop until we get the Id of the newly added lottery entity
            int newLotteryId = 0;
            do
            {
                var lotteryRepository = scope.ServiceProvider.GetRequiredService<ILotteryRepository>();
                var newLottery = await lotteryRepository.CreateLotteryAsync(new Lottery
                {
                    LotteryDateTime = DateTime.Now,
                    WinningNumbers = String.Join(',', winningNumbers),
                });
                newLotteryId = newLottery.Id;
            } while (newLotteryId == 0);

            //Third part: call the method that gets the winning numbers (and create all the temporary ones) and sends them to the front-end
            SendLotteryToFrontService(scope, newLotteryId, stoppingToken);

            //Fourth part: Get all tickets with remaining lotteries and 
            var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
            List<Ticket> activeTickets = await ticketRepository.GetActiveTicketsAsync();

            //Fifth part: Find the tickets who won and how much

            FindWinningTickets(scope, newLotteryId, activeTickets);

            //Sixth part: create and store the middle many to many entities (lotteryTicket) to the Databse
            var lotteryTicketRepository = scope.ServiceProvider.GetRequiredService<ILotteryTicketRepository>();
            await lotteryTicketRepository.CreateLotteryTicketAsync(activeTickets.Select(x => x.Id).ToList(), newLotteryId);



            //_logger.LogInformation($"Part 5 lottery METHOD ENDED {DateTime.Now}");
        }

        private int[] RandomNumberGenerator(int seed)
        {
            int[] randomNumbers = new int[20];
            for (int i = 0; i < randomNumbers.Length; i++)
            {
                Random rnd = new Random(seed);
                var x = rnd.Next(1, 81);
                do
                {
                    x = rnd.Next(1, 81);
                } while (randomNumbers.Contains(x));
                randomNumbers[i] = x;
            }

            //_logger.LogInformation($"Part 1 random numBer FINISHED {DateTime.Now.Second} + {DateTime.Now.Millisecond}");

            return randomNumbers;
        }

        private async Task<int> GetTrueRandomSeedNumber()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string responseString = await client.GetStringAsync("https://www.random.org/integers/?num=1&min=1&max=999999999&col=1&base=10&format=plain&rnd=new");
                    string numbersString = responseString.ToString();
                    return Convert.ToInt32(numbersString);
                }
            }
            catch
            {
                using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                {
                    var randomNumber = new byte[4];
                    rng.GetBytes(randomNumber);
                    return BitConverter.ToInt32(randomNumber, 0);
                }
            }
        }

        private async void SendLotteryToFrontService(IServiceScope scope, int lotteryId, CancellationToken stoppingToken)
        {
            numbersShownToUI = new List<int>();
            ILotteryRepository lotteryRepository = scope.ServiceProvider.GetRequiredService<ILotteryRepository>();
            string winningNumbersString = lotteryRepository.GetLotteryNumbersById(lotteryId);
            var lotteryHub = scope.ServiceProvider.GetRequiredService<IHubContext<LotteryHub>>();
            int[] allLotteryNumbers = GetAllLotteryNumbers(winningNumbersString);

            for (int i = 1; i <= allLotteryNumbers.Length; i++)
            {
                if (i % 4 == 0)
                {
                    await lotteryHub.Clients.All.SendAsync("ReceiveLotteryNumbers", new { Number = allLotteryNumbers[i - 1], IsWinningNumber = true });
                    numbersShownToUI.Add(allLotteryNumbers[i - 1]);
                }
                else
                {
                    await lotteryHub.Clients.All.SendAsync("ReceiveLotteryNumbers", new { Number = allLotteryNumbers[i - 1], IsWinningNumber = false });
                }

                await Task.Delay(850, stoppingToken);
            }

            //_logger.LogInformation($"Part 3 lottery sent to front FINISHED {DateTime.Now.Second} + {DateTime.Now.Millisecond}");
        }

        private int[] GetAllLotteryNumbers(string winningNumbersString)
        {
            int[] allLotteryNumbers = new int[80];
            int[] onlyWinningNumbers = winningNumbersString.Split(',').Select(int.Parse).ToArray();
            int tempNumb1 = 0;

            int[] tempWinningArray = new int[20]; //In order for the random temp numbers shown before the winning ones to be the same as future winning numbers but not already shown winning numbers

            //int tempNumber = 0;

            for (int i = 1; i <= 80; i++)
            {
                if (i % 4 == 3)
                {
                    allLotteryNumbers[i - 1] = allLotteryNumbers[i] = tempWinningArray[i / 4] = onlyWinningNumbers[i / 4];
                    i++;
                }
                else
                {
                    Random rnd = new Random();
                    int tempNumb2 = 0;

                    do
                    {
                        tempNumb2 = rnd.Next(1, 81);
                    } while (tempNumb1 == tempNumb2 || tempWinningArray.Contains(tempNumb2));
                    tempNumb1 = tempNumb2;
                    allLotteryNumbers[i - 1] = tempNumb1;
                }
            }

            return allLotteryNumbers;
        }

        private List<LotteryTicket> FindWinningTickets(IServiceScope scope, int lotteryId, List<Ticket> tickets)
        {
            ILotteryRepository lotteryRepository = scope.ServiceProvider.GetRequiredService<ILotteryRepository>();
            string winningNumbersString = lotteryRepository.GetLotteryNumbersById(lotteryId);
            int[] winningNumbers = winningNumbersString.Split(',').Select(int.Parse).ToArray();
            List<LotteryTicket> lotteryTickets = new List<LotteryTicket>();

            for(int i = 0; i < tickets.Count; i++)
            {
                int numbersMatched = 0;

                LotteryPerformance[] performanceForSpecificTicket = _performances.Where(p => p.NumberOfNumbers == tickets[i].NumberOfNumbers).ToArray();

                for (int j = 0; j < tickets[i].NumberOfNumbers; j++)
                {
                    int[] ticketNumbers = tickets[i].NumbersPlayed.Split(',').Select(int.Parse).ToArray();

                    if (winningNumbers.Contains(ticketNumbers[j]))
                    {
                        numbersMatched++;
                    }
                }

                lotteryTickets.Add(new LotteryTicket()
                {
                    LotteryId = lotteryId,
                    TicketId = tickets[i].Id,
                    NumbersMatched = numbersMatched,
                    MoneyWon = Convert.ToDecimal(performanceForSpecificTicket
                                                            .Where(p => p.NumberOfNumbers == tickets[i].NumberOfNumbers && p.NumbersMatched == numbersMatched)
                                                            .Select(p => p.PayoutMultiplier * tickets[i].MoneyPlayedPerLottery))
                });
            }

            return lotteryTickets;
        }
    }
}
