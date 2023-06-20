using KinoLotteryData.Services.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KinoLotteryWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyTicketsController : ControllerBase
    {
        private readonly ILotteryRepository _lotteryRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ILotteryTicketRepository _lotteryTicketRepository;

        public MyTicketsController(ILotteryRepository lotteryRepository, ITicketRepository ticketRepository, ILotteryTicketRepository lotteryTicketRepository)
        {
            _lotteryRepository = lotteryRepository;
            _ticketRepository = ticketRepository;
            _lotteryTicketRepository = lotteryTicketRepository;
        }

        [HttpGet]
        public ActionResult<string> GetAlreadyShownLotteryNumbers()
        {

            return Ok();
        }
    }
}
