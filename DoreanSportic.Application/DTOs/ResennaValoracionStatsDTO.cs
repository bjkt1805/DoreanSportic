using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoreanSportic.Application.DTOs
{
    public record ResennaValoracionStatsDTO
    {
        public int Star5 { get; set; }
        public int Star4 { get; set; }
        public int Star3 { get; set; }
        public int Star2 { get; set; }
        public int Star1 { get; set; }
        public int Total { get; set; }
        public double Average { get; set; }
    }
}
