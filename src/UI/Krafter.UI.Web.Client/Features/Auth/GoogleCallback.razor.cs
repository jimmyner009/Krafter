using Krafter.Api.Client.Models;
using Krafter.UI.Web.Client.Common.Models;
using Krafter.UI.Web.Client.Features.Auth._Shared;
using Microsoft.AspNetCore.WebUtilities;

namespace Krafter.UI.Web.Client.Features.Auth;

public partial class GoogleCallback(IAuthenticationService authenticationService, NavigationManager navigationManager)
    : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("code", out var code))
        {
            try
            {
                string returnUrl = "";
                string host = "";
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("state", out var encodedState))
                {
                    var decoded = Uri.UnescapeDataString(encodedState);
                    var parts = decoded.Split("|||");
                    host = parts[0];
                    returnUrl = parts.Length > 1 ? parts[1] : "";
                    if (host != uri.Host)
                    {
                        uri = new UriBuilder(uri) { Host = host }.Uri;
                        navigationManager.NavigateTo(uri.ToString(), forceLoad: true);
                        return;
                    }
                }

                if (!OperatingSystem.IsBrowser())
                {
                    return;
                }

                if (host == uri.Host)
                {
                    var ReturnUrl = returnUrl;
                    LocalAppSate.GoogleLoginReturnUrl = ReturnUrl;
                    var isSuccess = await authenticationService.LoginAsync(new TokenRequestInput()
                    {
                        IsExternalLogin = true,
                        Code = code.ToString()
                    });
                    if (isSuccess)
                    {
                        if (!string.IsNullOrWhiteSpace(ReturnUrl))
                        {
                            navigationManager.NavigateTo(ReturnUrl);
                        }
                        else
                        {
                            navigationManager.NavigateTo("/");
                        }
                    }
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Google login: {ex.Message}");
            }
        }
        else
        {
            navigationManager.NavigateTo("/login?error=google-auth-failed");
        }
    }
}