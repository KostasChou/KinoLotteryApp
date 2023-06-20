using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KinoLotteryData.Data.Entities
{
    public class Lottery
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime LotteryDateTime { get; set; }
        [Required]
        public string WinningNumbers { get; set; }
        public decimal MoneyPlayed { get; set; }
        public decimal MoneyWon { get; set; }
        public ICollection<LotteryTicket> LotteryTickets { get; set; }


    }
}
