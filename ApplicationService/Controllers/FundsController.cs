using ApplicationService.Models;
using ApplicationService.Repositories;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sample.MessageTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FundsController : ControllerBase
    {
        IRequestClient<CheckTransferStatus> _client;

        public FundsController(IRequestClient<CheckTransferStatus> client)
        {
            _client = client;
        }

        [HttpPost("Transfer")]
        public async Task<ActionResult> Transfer(Fund fund)
        {
            string refNo = string.Empty;
            using (var request = _client.Create(new CheckTransferStatus { ToAccount = fund.ToAccount, FromAccount = fund.FromAccount, Amount = fund.Amount}))
            {
                request.UseExecute(x => x.Headers.Set("custom-header", "abc@123"));
                request.UseExecute(x => x.Headers.Set("custom-header-new", "abc@123456"));
                var response = await request.GetResponse<TransferStatusResult>();
                refNo = response.Message.RefNo;
            }

            return Ok("Ref No. :- " + refNo);
        }
    }
}
