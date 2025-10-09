using System.Globalization;
using Krafter.UI.Web.Client.Infrastructure.Services;
using Krafter.UI.Web.Client.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Krafter.UI.Web.Client.Infrastructure.Http
{
    public static class HttpClientTenantConfigurator
    {
        public static void SetAPITenantHttpClientDefaults(IServiceCollection service, string remoteHostUrl, HttpClient client)
        {
            string navigationManagerBaseUri = string.Empty;
            string tenantIdentifier;
            string host;
            bool isRunningLocally;

            // Create a single service provider instance
            var serviceProvider = service.BuildServiceProvider();

            // Get form factor from IFormFactor
            var formFactor = serviceProvider.GetRequiredService<IFormFactor>();
            var formFactorType = formFactor.GetFormFactor();
            if (formFactorType == "Web")
            {
                // Resolve IHttpContextAccessor
                var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                var request = httpContextAccessor.HttpContext?.Request;
                if (request == null)
                {
                    throw new Exception("Request is null");
                }

                navigationManagerBaseUri = $"{request.Scheme}://{request.Host}";
            }
            else if (formFactorType == "WebAssembly")
            {
                var navigationManager = serviceProvider.GetRequiredService<NavigationManager>();
                navigationManagerBaseUri = navigationManager.BaseUri;
            }
            else
            {
                navigationManagerBaseUri = "https://krafter.krafter.com";
            }

            Uri uri = new Uri(navigationManagerBaseUri);
            host = uri.Host;
            isRunningLocally = host == "localhost" || host == "127.0.0.1";
            if (isRunningLocally)
            {
                tenantIdentifier = navigationManagerBaseUri == "https://localhost:7291/" ? "krafter" : "krafter";
                TenantInfo.Identifier = tenantIdentifier;
                TenantInfo.HostUrl = remoteHostUrl;
            }
            else
            {
                var strings = host.Split('.');
                tenantIdentifier = strings.Length > 2 ? strings[0] : "api";
                TenantInfo.Identifier = tenantIdentifier;
                var remoteHostUrl1 = "https://" + tenantIdentifier + "." + remoteHostUrl;
                TenantInfo.HostUrl = remoteHostUrl1;
            }
            TenantInfo.MainDomain = host;
            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
            client.BaseAddress = new Uri(TenantInfo.HostUrl);
            client.DefaultRequestHeaders.Add("x-tenant-identifier", tenantIdentifier);
        }

        public static void SetBFFTenantHttpClientDefaults(IServiceCollection service, string remoteHostUrl, HttpClient client)
        {
            string navigationManagerBaseUri = string.Empty;
            string tenantIdentifier;
            string host;
            bool isRunningLocally;

            // Create a single service provider instance
            var serviceProvider = service.BuildServiceProvider();

            // Get form factor from IFormFactor
            var formFactor = serviceProvider.GetRequiredService<IFormFactor>();
            var formFactorType = formFactor.GetFormFactor();
            if (formFactorType == "Web")
            {
                // Resolve IHttpContextAccessor
                var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                var request = httpContextAccessor.HttpContext?.Request;
                if (request == null)
                {
                    throw new Exception("Request is null");
                }

                navigationManagerBaseUri = $"{request.Scheme}://{request.Host}";
            }
            else if (formFactorType == "WebAssembly")
            {
                var navigationManager = serviceProvider.GetRequiredService<NavigationManager>();
                navigationManagerBaseUri = navigationManager.BaseUri;
            }
            else
            {
                navigationManagerBaseUri = "https://krafter.krafter.com";
            }

            Uri uri = new Uri(navigationManagerBaseUri);
            host = uri.Host;
            isRunningLocally = host == "localhost" || host == "127.0.0.1";

            var baseaddress = string.Empty;
            if (isRunningLocally)
            {
                tenantIdentifier = navigationManagerBaseUri == "https://localhost:7291/" ? "krafter" : "krafter";
                TenantInfo.Identifier = tenantIdentifier;
                baseaddress = $"{uri.Scheme}://{uri.Host}:{uri.Port}";
                TenantInfo.HostUrl = remoteHostUrl;
            }
            else
            {
                var strings = host.Split('.');
                tenantIdentifier = strings.Length > 2 ? strings[0] : "api";
                TenantInfo.Identifier = tenantIdentifier;
                var remoteHostUrl1 = "https://" + tenantIdentifier + "." + remoteHostUrl;
                TenantInfo.HostUrl = remoteHostUrl1;
                baseaddress = $"{uri.Scheme}://{uri.Host}:{uri.Port}";
            }
            TenantInfo.MainDomain = host;
            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
            client.BaseAddress = new Uri(baseaddress);
            client.DefaultRequestHeaders.Add("x-tenant-identifier", tenantIdentifier);
        }
    }
}