using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Portal.Shared.Models.Api.Request.Identity;
using Portal.Shared.Models.Api.Response;
using Portal.Shared.Services;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Portal.Shared.Constants.Storage;
using Portal.Shared.Routes;
using Portal.Services.AuthProviders;
using Portal.Shared.Extensions;
namespace Portal.Services.API
{
    internal class AuthenticationService : IAuthenticationService
    {
        #region prop
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        #endregion

        public AuthenticationService(
            IHttpClientFactory httpClientFactory,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClientFactory.CreateClient("PortalApiHttpClient");
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ClaimsPrincipal> CurrentUser()
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return state.User;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IResult> Login(TokenRequest model)
        {
            var response = await _httpClient.PostAsJsonAsync(TokenEndpoints.Get, model);
            var result = await response.ToResult<TokenResponse>();
            if (result.Succeeded)
            {
                var token = result.Data.Token;
                var refreshToken = result.Data.RefreshToken;
                var userImageURL = result.Data.UserImageURL;
                await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
                await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
                if (!string.IsNullOrEmpty(userImageURL))
                {
                    await _localStorage.SetItemAsync(StorageConstants.Local.UserImageURL, userImageURL);
                }

                await ((AuthStateProvider)this._authenticationStateProvider).StateChangedAsync();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                return await Result.SuccessAsync();
            }
            else
            {
                return await Result.FailAsync(result.Messages);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IResult> Logout()
        {
            await _localStorage.RemoveItemAsync(StorageConstants.Local.AuthToken);
            await _localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);
            await _localStorage.RemoveItemAsync(StorageConstants.Local.UserImageURL);
            ((AuthStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
            return await Result.SuccessAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<string> RefreshToken()
        {
            var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
            var refreshToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);

            var response = await _httpClient.PostAsJsonAsync(Shared.Routes.TokenEndpoints.Refresh, new RefreshTokenRequest { Token = token, RefreshToken = refreshToken });

            var result = await response.ToResult<TokenResponse>();

            if (!result.Succeeded)
            {
                throw new ApplicationException("Something went wrong during the refresh token action");
            }

            token = result.Data.Token;
            refreshToken = result.Data.RefreshToken;
            await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
            await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> TryRefreshToken()
        {
            //check if token exists
            var availableToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);
            if (string.IsNullOrEmpty(availableToken)) return string.Empty;
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
            var timeUTC = DateTime.UtcNow;
            var diff = expTime - timeUTC;
            if (diff.TotalMinutes <= 1)
                return await RefreshToken();
            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> TryForceRefreshToken()
        {
            return await RefreshToken();
        }
    }
}
