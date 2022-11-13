using KinoLotteryData.Dtos.TicketDto;
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
        public string NumbersPlayed { get; set; }

        [Required]
        [ForeignKey("Player")]
        public int PlayerId { get; set; }
        public Player Player { get; set; }

        [Required]
        public int MoneyPlayedPerLottery { get; set; }
        public int TotalMoney
        {
            get { return MoneyPlayedPerLottery * NumberOfLotteries; }
        }

        [Required]
        public int NumberOfLotteries { get; set; }
        public ICollection<LotteryTicket> LotteryTickets { get; set; }


        public static implicit operator Ticket(CreateTicketDto createTicketDto)
        {
            return new Ticket()
            {
                NumberOfNumbers = createTicketDto.NumberOfNumbers,
                NumbersPlayed = createTicketDto.NumbersPlayed,
                PlayerId = createTicketDto.PlayerId,
                NumberOfLotteries = createTicketDto.NumberOfLotteries
            };
        }
    }
}
