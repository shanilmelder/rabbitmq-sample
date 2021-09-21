using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.MessageTypes
{
    public class CheckTransferStatus 
    {
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public double Amount { get; set; }
    }
}
