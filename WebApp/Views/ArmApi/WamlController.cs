using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Protocols;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TodoListWebApp.DAL;
using TodoListWebApp.Services;

namespace TodoListWebApp.Controllers
{
    /// <summary>
    /// Controller for calling the ARM API
    /// </summary>
    [Authorize]
    public class WamlController : Controller
    {

        public async Task<ActionResult> Storage(Guid subscriptionId)
        {
            string clientId = CustomConfigurationManager.AppSettings["ida:ClientID"];
            string appKey = CustomConfigurationManager.AppSettings["ida:Password"];
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;


            string token = await GetTokenForArmAsync(tenantID, signedInUserID, userObjectID, clientId, appKey);


            var credentials = new TokenCloudCredentials(subscriptionId.ToString(), token);

            //var client = CloudContext.Clients.CreateMediaServicesManagementClient()
            var client = CloudContext.Clients.CreateStorageManagementClient(credentials);
            var accounts = await client.StorageAccounts.ListAsync();

            return View(accounts);
        }

        private async Task<string> GetTokenForArmAsync(string tenantID, string signedInUserID, string userObjectID, string clientId, string appKey)
        {
            // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
            ClientCredential clientcred = new ClientCredential(clientId, appKey);
            // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's EF DB
            AuthenticationContext authContext = new AuthenticationContext(string.Format("https://login.microsoftonline.com/{0}", tenantID), new EFADALTokenCache(signedInUserID));
            AuthenticationResult result = await authContext.AcquireTokenSilentAsync("https://management.core.windows.net/", clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
            return result.AccessToken;
        }
    }
}