using KinoLotteryData.Services.Repositories;
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
        public LotteryController(ILotteryRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public ActionResult<string> GetLotteryNumbers()
        {
            var a = _repo.GetLotteryNumbers();
            return Ok(a);
        }
    }
}
