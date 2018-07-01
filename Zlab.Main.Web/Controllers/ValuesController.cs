using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zlab.DataCore;
using Zlab.UtilsCore;
using Zlab.Web.Service.Implments;
using Zlab.Web.Service.Interfaces;

namespace Zlab.Main.Web.Controllers
{
    [Route("api/values")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IIdsService idService;
        public ValuesController(IIdsService idsService)
        {
            this.idService = idsService;
        }
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<string>> Get(string key, int value, string token)
        {
            try
            {
                if (token == "token-zlab-yixin")
                {
                    await idService.SetIdAsync(key, value);
                    return ReturnResult.Success();
                }

                else
                {
                    LogHelper.Log("token error");
                    return ReturnResult.Fail();
                }
                   
            }
           catch(Exception ex)
            {
                LogHelper.Error(ex);
                return ReturnResult.Fail("error");
            }
        }


    }
}
