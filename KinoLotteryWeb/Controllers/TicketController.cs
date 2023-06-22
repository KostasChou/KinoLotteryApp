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
    //[Authorize]
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
        //[Authorize]
        public async Task<IActionResult> CreateTicket(CreateTicketDto createTicketDto)
        {
            //var a = User.FindFirst("id");
            if (!User.Identity.IsAuthenticated)
                return BadRequest("You need to log in to create a ticket.");
            else if (User.FindFirst("Id") == null)
            {
                return BadRequest("Please log with a valid account in to continue.");
            }
            else if (User.FindFirst("Id").Value == null)
                return BadRequest("Please log with a valid account in to continue.");




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
                return BadRequest( "Please log in to continue." );
            }
            await _repo.CreateTicketAsync(ticket);
            return Ok();
        }
    }
}
