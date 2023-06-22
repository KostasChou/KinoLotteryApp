using KinoLotteryData.Services.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        public ActionResult<string> GetAllTicketsForUser()
        {
            if (!User.Identity.IsAuthenticated)
                return BadRequest("You need to log in to create a ticket.");
            else if (User.FindFirst("Id") == null)
            {
                return BadRequest("Please log with a valid account in to continue.");
            }
            else if (User.FindFirst("Id").Value == null)
                return BadRequest("Please log with a valid account in to continue.");

            var tickets = _ticketRepository.GetTicketsByUserIdAsync(Convert.ToInt32(User.FindFirst("Id").Value));

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return Ok(JsonConvert.SerializeObject(tickets, settings));
        }
    }
}
