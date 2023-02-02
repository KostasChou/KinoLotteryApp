using KinoLotteryData.Data.Entities;
using KinoLotteryData.Dtos.TicketDto;
using KinoLotteryData.Services.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace KinoLotteryWeb.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository _repo;
        public TicketController(ITicketRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(CreateTicketDto createTicketDto)
        {
            if (User.FindFirst("Id").Value == null)
                return BadRequest(new ProblemDetails() { Detail = "Please log in to continue." });


            if (createTicketDto == null)
            {
                return NotFound();
            }
            Ticket ticket = createTicketDto;
            try
            {
                ticket.PlayerId = Convert.ToInt32(User.FindFirst("Id").Value);
            }
            catch 
            {
                return BadRequest(new ProblemDetails() { Detail = "Please log in to continue." });
            }
            await _repo.CreateTicketAsync(ticket);
            return Ok();
        }
    }
}
