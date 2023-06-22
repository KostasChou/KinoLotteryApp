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
                    
                    await Task.Delay(waitTime, stoppingToken);
                    await LotteryMethod(stoppingToken);

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
            var randomNumberSeed = await GetTrueRandomSeedNumber(scope);
            var winningNumbers = RandomNumberGenerator(randomNumberSeed);

            //Second part: Store the newly created lottery in the database and do not leave the loop until we get the Id of the newly added lottery entity
            int newLotteryId = 0;
            Lottery newLottery = new Lottery();
            var lotteryRepository = scope.ServiceProvider.GetRequiredService<ILotteryRepository>();

            do
            {
                
                newLottery = await lotteryRepository.CreateLotteryAsync(new Lottery
                {
                    LotteryDateTime = DateTime.Now,
                    WinningNumbers = String.Join(',', winningNumbers),
                });
                newLotteryId = newLottery.Id;
            } while (newLotteryId == 0);

            //Third part: call the method that sends the winning numbers to the front-end with Signul-R
            Task.Run(() => SendLotteryToFrontService(scope, newLottery.WinningNumbers, stoppingToken));

            //Fourth part: Get all tickets with remaining lotteries and 
            var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
            List<Ticket> activeTickets = await ticketRepository.GetActiveTicketsAsync(newLottery.LotteryDateTime);


            decimal moneyPlayed, moneyWon = 0;
            moneyPlayed = activeTickets.Select(t => t.MoneyPlayedPerLottery).Sum();

            //Fifth part: Find the tickets that won ,how much, and create the lottery tickets to store
            var performanceRepository = scope.ServiceProvider.GetRequiredService<ILotteryPerformanveRepository>();
            _performances = performanceRepository.GetLotteryPerformance();

            _logger.LogInformation($"Just before FindWinningTickets{DateTime.Now}");
            
            List<LotteryTicket> lotteryTicketsToStore = FindWinningTickets(scope, newLottery, activeTickets, ref moneyWon);

            _logger.LogInformation($"Just after FindWinningTickets{DateTime.Now}");

            //Sixth part: create and store the middle many to many entities (lotteryTicket) to the Databse
            var lotteryTicketRepository = scope.ServiceProvider.GetRequiredService<ILotteryTicketRepository>();
            await lotteryTicketRepository.CreateLotteryTicketAsync(lotteryTicketsToStore);

            //Seventh part: store the info about money played and won in the lottery 
            newLottery.MoneyPlayed = moneyPlayed;
            newLottery.MoneyWon = moneyWon;

            lotteryRepository.UpdateLotteryWithMoneyPlayerandWon(newLottery);
        }
        private async Task<int> GetTrueRandomSeedNumber(IServiceScope scope)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    IApiUriRepository repo = scope.ServiceProvider.GetRequiredService<IApiUriRepository>();
                    string apiUriString = repo.GetApiUriString();
                    string responseString = await client.GetStringAsync(apiUriString);
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

        private int[] RandomNumberGenerator(int seed)
        {
            int[] randomNumbers = new int[20];
            Random rnd = new Random(seed);
            for (int i = 0; i < randomNumbers.Length; i++)
            {   
                var x = rnd.Next(1, 81);
                do
                {
                    x = rnd.Next(1, 81);
                } while (randomNumbers.Contains(x));
                randomNumbers[i] = x;
            }

            return randomNumbers;
        }

        private async Task SendLotteryToFrontService(IServiceScope scope, string winningNumbersString, CancellationToken stoppingToken)
        {
            numbersShownToUI = new List<int>();
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

                await Task.Delay(850, stoppingToken).ConfigureAwait(false); ;
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

        private List<LotteryTicket> FindWinningTickets(IServiceScope scope, Lottery lottery, List<Ticket> tickets, ref decimal moneyWon)
        {
            int[] winningNumbers = lottery.WinningNumbers.Split(',').Select(int.Parse).ToArray();
            List<LotteryTicket> lotteryTickets = new List<LotteryTicket>();

            for(int i = 0; i < tickets.Count; i++)
            {
                _logger.LogInformation($"{i} Ticket entered loop{DateTime.Now}");
                int numbersMatched = 0;

                int[] ticketNumbers = tickets[i].NumbersPlayed.Split(',').Select(int.Parse).ToArray();
                for (int j = 0; j < tickets[i].NumberOfNumbers; j++)
                {
                    if (winningNumbers.Contains(ticketNumbers[j]))
                    {
                        numbersMatched++;
                    }
                }

                decimal ticketMultiplier = _performances.Where(p => p.NumberOfNumbers == tickets[i].NumberOfNumbers && p.NumbersMatched == numbersMatched).Select(p => p.PayoutMultiplier).First();


                lotteryTickets.Add(new LotteryTicket()
                {
                    LotteryId = lottery.Id,
                    TicketId = tickets[i].Id,
                    NumbersMatched = numbersMatched,
                    MoneyWon = tickets[i].MoneyPlayedPerLottery * ticketMultiplier
            });

                moneyWon += lotteryTickets[i].MoneyWon;
                _logger.LogInformation($"{i} Ticket leaving loop {DateTime.Now}");
            }

            return lotteryTickets;
        }
    }
}
