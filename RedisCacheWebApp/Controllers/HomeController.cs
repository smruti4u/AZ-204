using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RedisCacheWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly Services.ICacheService cacheService;
        private readonly Services.IDataService dataService;
        public HomeController(ILogger<HomeController> logger,
            Services.ICacheService cacheService,
            Services.IDataService dataService)
        {
            _logger = logger;
            this.cacheService = cacheService;
            this.dataService = dataService;
        }

        public async Task<IActionResult> Index()
        {
            var cahedResult = await this.cacheService.GetData<Services.State>("stateKey");
            Services.State result = null;
            if(cahedResult == null)
            {
                result =  await this.dataService.GetData(2);

                if( result !=  null)
                {
                    await this.cacheService.SetData<Services.State>("stateKey", result, TimeSpan.FromMinutes(10));
                }                
            }
            else
            {
                result = cahedResult;
            }

            return Content(result.DisplayName);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
