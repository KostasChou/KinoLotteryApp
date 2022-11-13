using KinoLotteryData.Data.Entities;
using KinoLotteryData.Dtos.TicketDto;
using KinoLotteryData.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KinoLotteryWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository _repo;
        public TicketController(ITicketRepository repo)
        {
            _repo = repo;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketDto createTicketDto)
        {
            if(createTicketDto == null)
            {
                return NotFound();
            }
            Ticket ticket = createTicketDto;
            var a = ticket.LotteryTickets;
            await _repo.CreateTicketAsync(ticket);
            return Ok();
        }
    }
}
