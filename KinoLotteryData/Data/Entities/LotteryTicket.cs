using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KinoLotteryData.Data.Entities
{
    public class LotteryTicket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Lottery")]
        public int LotteryId { get; set; }
        public Lottery Lottery { get; set; }
        
        [Required]
        [ForeignKey("Ticket")]
        public int TicketId { get; set; }
        public Ticket ticket { get; set; }
    }
}
