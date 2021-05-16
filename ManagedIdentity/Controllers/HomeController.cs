using ManagedIdentity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ManagedIdentity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            this.configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetSecret()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var secret = await keyVaultClient.GetSecretAsync("https://azkeyvault1989.vault.azure.net/secrets/connectionstring/").ConfigureAwait(false);
            return secret.Value;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var secret = await GetSecret();
                return Content($"The Secret Retrieved from Key Vault Is {secret}");
            }
            catch(Exception exe)
            {
                return Content(exe.Message);
            }
        }

        public IActionResult Privacy()
        {
            try
            {
                var connectionString = this.configuration["connectionstring"];
                var secondaryDbConnectionString = this.configuration["SecondaryDBConnectionString"];
                return Content($"connectionString : {connectionString} secondaryDbConnectionString : {secondaryDbConnectionString}");
            }
            catch(Exception exe)
            {
                return Content(exe.Message);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
