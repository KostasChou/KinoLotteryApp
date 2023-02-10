using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace KinoLotteryWeb.Services
{
    public class LotteryHub : Hub
    {
        public async Task SendLotteryNumbers(int[] winningNumbers)
        {
            await Clients.All.SendAsync("ReceiveLotteryNumbers", winningNumbers);
        }
    }
}
