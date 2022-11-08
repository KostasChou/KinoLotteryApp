using KinoLotteryData.Dtos.PlayerDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KinoLotteryData.Data.Entities
{
    public class Player
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [MaxLength(30)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(500)]
        public byte[] Password { get; set; }
        [Required]
        public byte[] Salt { get; set; }
        [Required]
        public DateTime? DoB { get; set; }
        [Required]
        public int Wallet { get; set; }
        public ICollection<Ticket> Tickets { get; set; }

        public static implicit operator Player(RegisterDto user)
        {
            return new Player
            {
                UserName = user.UserName,
                DoB = user.DoB
            };
        }

    }
}
