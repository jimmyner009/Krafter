using System.Globalization;
using Krafter.UI.Web.Client.Common.Models;
using Krafter.UI.Web.Client.Infrastructure.Services;
using Microsoft.AspNetCore.Http;

namespace Krafter.UI.Web.Client.Kiota
{
    //public class TenantHeaderHandler : DelegatingHandler
    //{
    //    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    //    {
    //        // Example: derive tenant identifier dynamically
    //        var tenantIdentifier = "krafter"; // replace with logic if multi-tenant
    //        request.Headers.Remove("x-tenant-identifier");
    //        request.Headers.Add("x-tenant-identifier", tenantIdentifier);

    //        // Add culture header
    //        request.Headers.AcceptLanguage.Clear();
    //        request.Headers.AcceptLanguage.ParseAdd(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

    //        return base.SendAsync(request, cancellationToken);
    //    }
    //}


    public class TenantHeaderHandler : DelegatingHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _remoteHostUrl;

        public TenantHeaderHandler(IServiceProvider serviceProvider, string remoteHostUrl)
        {
            _serviceProvider = serviceProvider;
            _remoteHostUrl = remoteHostUrl;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string navigationManagerBaseUri;
            string tenantIdentifier;
            string host;
            bool isRunningLocally;

            // Resolve form factor
            var formFactor = _serviceProvider.GetRequiredService<IFormFactor>();
            var formFactorType = formFactor.GetFormFactor();

            if (formFactorType == "Web")
            {
                var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
                var httpRequest = httpContextAccessor.HttpContext?.Request;
                if (httpRequest == null)
                    throw new Exception("Request is null");

                navigationManagerBaseUri = $"{httpRequest.Scheme}://{httpRequest.Host}";
            }
            else if (formFactorType == "WebAssembly")
            {
                var navigationManager = _serviceProvider.GetRequiredService<NavigationManager>();
                navigationManagerBaseUri = navigationManager.BaseUri;
            }
            else
            {
                navigationManagerBaseUri = "https://krafter.krafter.com";
            }

            var uri = new Uri(navigationManagerBaseUri);
            host = uri.Host;
            isRunningLocally = host == "localhost" || host == "127.0.0.1";

            if (isRunningLocally)
            {
                tenantIdentifier = "krafter"; // adjust if you want different local logic
                TenantInfo.Identifier = tenantIdentifier;
                TenantInfo.HostUrl = _remoteHostUrl;
            }
            else
            {
                var strings = host.Split('.');
                tenantIdentifier = strings.Length > 2 ? strings[0] : "api";
                TenantInfo.Identifier = tenantIdentifier;
                TenantInfo.HostUrl = $"https://{tenantIdentifier}.{_remoteHostUrl}";
            }

            TenantInfo.MainDomain = host;

            // Inject headers
            request.Headers.Remove("x-tenant-identifier");
            request.Headers.Add("x-tenant-identifier", TenantInfo.Identifier);

            request.Headers.AcceptLanguage.Clear();
            request.Headers.AcceptLanguage.ParseAdd(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            // Ensure base address is set
            if (request.RequestUri != null && !request.RequestUri.IsAbsoluteUri)
            {
                request.RequestUri = new Uri(new Uri(TenantInfo.HostUrl), request.RequestUri);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }

}
