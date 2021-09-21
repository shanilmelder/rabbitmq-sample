using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FundService.Models
{
    public class Fund
    {
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public double Amount { get; set; }
    }
}
