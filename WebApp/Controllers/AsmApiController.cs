using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Protocols;
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
    [RoutePrefix("asmapi")]
    public class AsmApiController : Controller
    {

        [Route("subscriptions/{subscriptionId}/storage")]
        public async Task<ActionResult> Storage(Guid subscriptionId)
        {
            string clientId = CustomConfigurationManager.AppSettings["ida:ClientID"];
            string appKey = CustomConfigurationManager.AppSettings["ida:Password"];
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;


            string token = await GetTokenForArmAsync(tenantID, signedInUserID, userObjectID, clientId, appKey);

            var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", token),
                    // didn't seem to take notice of this!
                    //Accept = {
                    //    new MediaTypeWithQualityHeaderValue ("application/json")
                    //    }
                }
            };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                // TODO - parameterise subscription ID
                RequestUri = new Uri($"https://management.core.windows.net/{subscriptionId}/services/storageservices"),
                Headers =
                {
                    { "x-ms-version", "2014-10-01"}
                }
            };
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.SendAsync(request);

            //var response = await client.GetAsync("https://management.core.windows.net/services/storageservices");
            //var storageServices = await response.Content.ReadAsAsync<StorageServices>();
            //return View(storageServices);

            var responseBody = await response.Content.ReadAsStringAsync();
            return Content(responseBody, "application/xml");
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



    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/windowsazure", IsNullable = false)]
    public partial class StorageServices
    {

        private StorageServicesStorageService[] storageServiceField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("StorageService")]
        public StorageServicesStorageService[] StorageService
        {
            get
            {
                return this.storageServiceField;
            }
            set
            {
                this.storageServiceField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class StorageServicesStorageService
    {

        private string urlField;

        private string serviceNameField;

        private StorageServicesStorageServiceStorageServiceProperties storageServicePropertiesField;

        private StorageServicesStorageServiceExtendedProperty[] extendedPropertiesField;

        /// <remarks/>
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        public string ServiceName
        {
            get
            {
                return this.serviceNameField;
            }
            set
            {
                this.serviceNameField = value;
            }
        }

        /// <remarks/>
        public StorageServicesStorageServiceStorageServiceProperties StorageServiceProperties
        {
            get
            {
                return this.storageServicePropertiesField;
            }
            set
            {
                this.storageServicePropertiesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ExtendedProperty", IsNullable = false)]
        public StorageServicesStorageServiceExtendedProperty[] ExtendedProperties
        {
            get
            {
                return this.extendedPropertiesField;
            }
            set
            {
                this.extendedPropertiesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class StorageServicesStorageServiceStorageServiceProperties
    {

        private object descriptionField;

        private string locationField;

        private string labelField;

        private string statusField;

        private string[] endpointsField;

        private string geoPrimaryRegionField;

        private string statusOfPrimaryField;

        private string geoSecondaryRegionField;

        private string statusOfSecondaryField;

        private System.DateTime creationTimeField;

        private object customDomainsField;

        private string accountTypeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public object Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        public string Label
        {
            get
            {
                return this.labelField;
            }
            set
            {
                this.labelField = value;
            }
        }

        /// <remarks/>
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Endpoint", IsNullable = false)]
        public string[] Endpoints
        {
            get
            {
                return this.endpointsField;
            }
            set
            {
                this.endpointsField = value;
            }
        }

        /// <remarks/>
        public string GeoPrimaryRegion
        {
            get
            {
                return this.geoPrimaryRegionField;
            }
            set
            {
                this.geoPrimaryRegionField = value;
            }
        }

        /// <remarks/>
        public string StatusOfPrimary
        {
            get
            {
                return this.statusOfPrimaryField;
            }
            set
            {
                this.statusOfPrimaryField = value;
            }
        }

        /// <remarks/>
        public string GeoSecondaryRegion
        {
            get
            {
                return this.geoSecondaryRegionField;
            }
            set
            {
                this.geoSecondaryRegionField = value;
            }
        }

        /// <remarks/>
        public string StatusOfSecondary
        {
            get
            {
                return this.statusOfSecondaryField;
            }
            set
            {
                this.statusOfSecondaryField = value;
            }
        }

        /// <remarks/>
        public System.DateTime CreationTime
        {
            get
            {
                return this.creationTimeField;
            }
            set
            {
                this.creationTimeField = value;
            }
        }

        /// <remarks/>
        public object CustomDomains
        {
            get
            {
                return this.customDomainsField;
            }
            set
            {
                this.customDomainsField = value;
            }
        }

        /// <remarks/>
        public string AccountType
        {
            get
            {
                return this.accountTypeField;
            }
            set
            {
                this.accountTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/windowsazure")]
    public partial class StorageServicesStorageServiceExtendedProperty
    {

        private string nameField;

        private string valueField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }


}