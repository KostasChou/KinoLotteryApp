using KinoLotteryData.Services.Repositories;
using KinoLotteryWeb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KinoLotteryWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LotteryController : ControllerBase
    {
        private readonly ILotteryRepository _repo;
        
        
        //private readonly LotteryService _service;




        public LotteryController(ILotteryRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public ActionResult<string> GetAlreadyShownLotteryNumbers()
        {
            return Ok(string.Join(',', LotteryService.numbersShownToUI.ToArray()));
        }
    }
}
