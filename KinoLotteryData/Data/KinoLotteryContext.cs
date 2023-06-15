using KinoLotteryData.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KinoLotteryData.Data
{
    public class KinoLotteryContext : DbContext
    {
        public KinoLotteryContext(DbContextOptions<KinoLotteryContext> options) : base(options)
        {

        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Lottery> Lotteries { get; set; }
        public DbSet<LotteryTicket> LotteryTickets { get; set; }
        public DbSet<LotteryPerformance> LotteryPerformances { get; set; }

    }
}
