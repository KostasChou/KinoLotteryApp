using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KinoLotteryData.Dtos.PlayerDtos
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(30)]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string RepeatPassword { get; set; }
        [Required]
        public DateTime? DoB { get; set; }
    }
}
