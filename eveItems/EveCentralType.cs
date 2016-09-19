using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveCentral
{
    public class OrderData
    {
        public long volume { get; set; }
        public double fivePercent { get; set; }
    }
    public class MarketType
    {
        public OrderData buy { get; set; }
        public OrderData sell { get; set; }
    }
}
