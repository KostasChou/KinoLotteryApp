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
        public string WinningNumbers { get; set; }
        public int MoneyPlayed { get; set; }

        public int MoneyWon { get; set; }

        public ICollection<LotteryTicket> LotteryTickets { get; set; }
    }
}
