using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KinoLotteryData.Dtos.PlayerDtos
{
    public class LogInDto
    {
        [Required]
        [MaxLength(30)]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
