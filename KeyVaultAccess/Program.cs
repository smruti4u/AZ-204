using Microsoft.Azure.KeyVault;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeyVaultAccess
{
    //ADAL
    //MSAL
    class Program
    {
        static async Task Main(string[] args)
        {
            var kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
            var secret = await kvClient.GetSecretAsync("https://azkeyvault1989.vault.azure.net/secrets/connectionstring/").ConfigureAwait(false);
            
            Console.WriteLine(secret.Value);
            Console.Read();
        }

        public async static Task<string> GetAccessToken(string a, string b, string c)
        {
            const string ClientId = "2a0245aa-474b-4214-a7cf-acc735707ea0";
            const string TeanantId = "549a973a-fda2-4190-9ee8-67445857b006";

            var app = ConfidentialClientApplicationBuilder.Create(ClientId)
                .WithAuthority(AzureCloudInstance.AzurePublic, TeanantId)
                .WithClientSecret("6-9~lPF6bsdG60SKZf~.m.5Q9XR-D9pc1l").Build();
            var scopes = new List<string>() { "https://vault.azure.net/.default" };
            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync().ConfigureAwait(false);

            return result.AccessToken;
        }
    }
}
