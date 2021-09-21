using MassTransit;
using Sample.MessageTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FundService
{
    public class CheckOrderStatusConsumer : IConsumer<CheckTransferStatus>
    {
        public async Task Consume(ConsumeContext<CheckTransferStatus> context)
        {
            await context.RespondAsync(new TransferStatusResult
            {
                RefNo = GetRefNo().ToString()
            });
        }

        private long GetRefNo(long min = 1000000000, long max = 9999999999)
        {
            Random rand = new Random();
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }
    }
}
