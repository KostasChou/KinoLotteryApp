using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KinoLotteryData.Data.Entities
{
    public class LotteryPerformance
    {
        [Key]
        public int LotteryPerformanceId { get; set; }
        [Required]
        public int NumberOfNumbers { get; set; }
        [Required]
        public int NumbersMatched { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PayoutMultiplier { get; set; }
    }
}
