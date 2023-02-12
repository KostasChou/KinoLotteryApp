using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace KinoLotteryWeb.Services
{
    public class LotteryHub : Hub
    {
        public async Task SendLotteryNumbers(int lotteryNumber)
        {
            await Clients.All.SendAsync("ReceiveLotteryNumbers", lotteryNumber);
        }
    }
}
