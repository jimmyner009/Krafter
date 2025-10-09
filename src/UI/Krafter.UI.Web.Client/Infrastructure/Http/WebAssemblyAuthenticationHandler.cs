using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using Krafter.UI.Web.Client.Common.Models;
using Krafter.UI.Web.Client.Features.Auth._Shared;
using Krafter.UI.Web.Client.Infrastructure.Storage;

namespace Krafter.UI.Web.Client.Infrastructure.Http;

public class WebAssemblyAuthenticationHandler(
    IAuthenticationService authenticationService,
    IKrafterLocalStorageService localStorage,
    ILogger<WebAssemblyAuthenticationHandler> logger)
    : DelegatingHandler
{
    private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);
    private bool _disposed;
    private volatile bool _isLoggedOut = false;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Skip processing if we've already logged out
        if (_isLoggedOut)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var isToServer = request.RequestUri?.AbsoluteUri.StartsWith(TenantInfo.HostUrl ?? "") ?? false;

        // Get token directly from localStorage
        var jwt = await localStorage.GetCachedAuthTokenAsync();
        logger.LogDebug("WASM handler - retrieved JWT token from localStorage");

        if (isToServer && !string.IsNullOrEmpty(jwt))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        var response = await base.SendAsync(request, cancellationToken);
        logger.LogDebug("WASM handler - request completed with status: {StatusCode}", response.StatusCode);

        // Handle 401 responses
        if (!string.IsNullOrEmpty(jwt) &&
            response.StatusCode == HttpStatusCode.Unauthorized &&
            !_isLoggedOut)
        {
            logger.LogInformation("WASM handler - 401 received, checking synchronization status");

            await _refreshSemaphore.WaitAsync(cancellationToken);
            try
            {
                if (_isLoggedOut) return response;

                // STEP 1: Wait for any ongoing synchronization to complete
                if (TokenSynchronizationManager.IsSynchronizing)
                {
                    logger.LogInformation("WASM handler - synchronization in progress, waiting for completion");
                    await TokenSynchronizationManager.WaitForSynchronizationAsync(cancellationToken);

                    // Check if we now have a fresh token after synchronization
                    var freshToken = await localStorage.GetCachedAuthTokenAsync();
                    if (!string.IsNullOrEmpty(freshToken) && freshToken != jwt && !IsTokenExpired(freshToken))
                    {
                        logger.LogInformation("WASM handler - found fresh token after synchronization, retrying request");
                        var retryRequest = await CloneRequestAsync(request);
                        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", freshToken);
                        return await base.SendAsync(retryRequest, cancellationToken);
                    }
                }

                // STEP 2: Check for fresh tokens in localStorage
                var currentToken = await localStorage.GetCachedAuthTokenAsync();
                if (!string.IsNullOrEmpty(currentToken) && currentToken != jwt && !IsTokenExpired(currentToken))
                {
                    logger.LogInformation("WASM handler - found fresh token in localStorage, retrying");
                    var retryRequest = await CloneRequestAsync(request);
                    retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", currentToken);
                    return await base.SendAsync(retryRequest, cancellationToken);
                }

                // STEP 3: Check if we have a recent sync (avoid immediate refresh after sync)
                if (TokenSynchronizationManager.HasRecentSync(TimeSpan.FromSeconds(3)))
                {
                    logger.LogInformation("WASM handler - recent sync detected, skipping refresh to avoid duplicate calls");
                    return response; // Return the 401 response without retrying
                }

                // STEP 4: Only if no fresh tokens available and no recent sync, attempt refresh
                jwt = currentToken ?? jwt;
                if (string.IsNullOrEmpty(jwt) || IsTokenExpired(jwt))
                {
                    logger.LogInformation("WASM handler - no fresh tokens available, attempting refresh as last resort");
                    var refreshSuccess = await authenticationService.RefreshAsync();

                    if (refreshSuccess)
                    {
                        var newJwt = await localStorage.GetCachedAuthTokenAsync();
                        logger.LogInformation("WASM handler - token refreshed successfully");

                        if (!string.IsNullOrEmpty(newJwt) && isToServer)
                        {
                            var retryRequest = await CloneRequestAsync(request);
                            retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newJwt);
                            response = await base.SendAsync(retryRequest, cancellationToken);
                            logger.LogInformation("WASM handler - request retried successfully");
                        }
                    }
                    else
                    {
                        logger.LogWarning("WASM handler - token refresh failed, forcing logout");
                        _isLoggedOut = true;
                        await localStorage.ClearCacheAsync();
                        await authenticationService.LogoutAsync("WebAssemblyAuthenticationHandler - Refresh Failed");
                    }
                }
                else
                {
                    // Token not expired, retry with current token
                    var retryRequest = await CloneRequestAsync(request);
                    retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
                    response = await base.SendAsync(retryRequest, cancellationToken);
                    logger.LogInformation("WASM handler - request retried with current token");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WASM handler - error during token handling");
                _isLoggedOut = true;
                await localStorage.ClearCacheAsync();
                await authenticationService.LogoutAsync("WebAssemblyAuthenticationHandler - Exception");
            }
            finally
            {
                _refreshSemaphore.Release();
            }
        }

        return response;
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri)
        {
            Version = original.Version,
            VersionPolicy = original.VersionPolicy
        };

        foreach (var header in original.Headers.Where(h => !string.Equals(h.Key, "Authorization", StringComparison.OrdinalIgnoreCase)))
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (original.Content != null)
        {
            var contentBytes = await original.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(contentBytes);

            foreach (var header in original.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        foreach (var option in original.Options)
        {
            clone.Options.TryAdd(option.Key, option.Value);
        }

        return clone;
    }

    private bool IsTokenExpired(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (handler.ReadToken(token) is JwtSecurityToken jwtToken)
            {
                return jwtToken.ValidTo <= DateTime.UtcNow.AddMinutes(1);
            }
            return true;
        }
        catch
        {
            return true;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            try
            {
                _refreshSemaphore?.Dispose();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error disposing SemaphoreSlim in WebAssemblyAuthenticationHandler");
            }
            finally
            {
                _disposed = true;
            }
        }
        base.Dispose(disposing);
    }
}

public static class TokenSynchronizationManager
{
    private static readonly SemaphoreSlim _synchronizationSemaphore = new(1, 1);
    private static volatile bool _isSynchronizing = false;
    private static DateTime _lastSyncTime = DateTime.MinValue;

    public static bool IsSynchronizing => _isSynchronizing;

    public static async Task<bool> TryExecuteWithSynchronizationAsync<T>(
        Func<Task<T>> operation,
        Func<T, bool> isSuccessful,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        // If already synchronizing, wait for it to complete
        if (_isSynchronizing)
        {
            logger.LogInformation("Synchronization in progress, waiting...");
            await _synchronizationSemaphore.WaitAsync(cancellationToken);
            _synchronizationSemaphore.Release();
            return true; // Assume it succeeded since we waited
        }

        // Execute with synchronization lock
        await _synchronizationSemaphore.WaitAsync(cancellationToken);
        try
        {
            _isSynchronizing = true;
            logger.LogInformation("Starting synchronized operation");

            var result = await operation();
            var success = isSuccessful(result);

            if (success)
            {
                _lastSyncTime = DateTime.UtcNow;
            }

            logger.LogInformation("Synchronized operation completed: {Success}", success);
            return success;
        }
        finally
        {
            _isSynchronizing = false;
            _synchronizationSemaphore.Release();
        }
    }

    public static async Task WaitForSynchronizationAsync(CancellationToken cancellationToken = default)
    {
        if (!_isSynchronizing) return;

        await _synchronizationSemaphore.WaitAsync(cancellationToken);
        _synchronizationSemaphore.Release();
    }

    public static bool HasRecentSync(TimeSpan threshold) =>
        DateTime.UtcNow - _lastSyncTime < threshold;
}