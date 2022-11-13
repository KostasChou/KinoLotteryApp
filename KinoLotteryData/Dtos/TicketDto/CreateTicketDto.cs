using KinoLotteryData.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KinoLotteryData.Dtos.TicketDto
{
    public class CreateTicketDto
    {
        [Required]
        public int NumberOfNumbers { get; set; }
        [Required]
        public string NumbersPlayed { get; set; }
        [Required]
        public int PlayerId { get; set; }
        [Required]
        public int NumberOfLotteries { get; set; }
        [Required]
        public int MoneyPlayedPerLottery { get; set; }


    }
}
