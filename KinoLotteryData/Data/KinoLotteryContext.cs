using KinoLotteryData.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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

        public DbSet<APIURIEntity> APIURIEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<APIURIEntity>()
                .HasKey(e => e.URIId)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity<APIURIEntity>()
                .HasData(new APIURIEntity { URIId = 1, URIString = "https://www.random.org/integers/?num=1&min=1&max=999999999&col=1&base=10&format=plain&rnd=new" });

            base.OnModelCreating(modelBuilder);
        }

    }
}
