using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KinoLotteryData.Data.Entities
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int NumberOfNumbers { get; set; }
        [Required]
        public string NumbersPlayer { get; set; }

        [Required]
        [ForeignKey("Player")]
        public int PlayerId { get; set; }
        public Player Player { get; set; }

        [Required]
        public ICollection<LotteryTicket> LotteryTickets { get; set; }
    }
}
