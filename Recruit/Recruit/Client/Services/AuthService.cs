using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Recruit.Shared;
using Recruit.Shared.ViewModels;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Recruit.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorageService;

        public AuthService(HttpClient httpClient,
            AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorageService = localStorageService;
        }

        public async Task<AuthResult> Login(UserViewModel user)
        {
            var dataJson = JsonSerializer.Serialize(user);
            var response = await _httpClient.PostAsync("api/Accounts/Login", new StringContent(dataJson, Encoding.UTF8, "application/json"));
            var loginResult = JsonSerializer.Deserialize<AuthResult>(await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!response.IsSuccessStatusCode)
            {
                return loginResult ?? new AuthResult();
            }

            await _localStorageService.SetItemAsync("authToken", loginResult?.Token);
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(user.Email, loginResult?.FullName!);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult?.Token);

            return loginResult ?? new AuthResult();
        }

        public async Task<AuthResult> Register(UserViewModel user)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Accounts/Register", user);
            var authResult = await response.Content.ReadFromJsonAsync<AuthResult>();
            return authResult ?? new AuthResult();
        }


        public async Task Logout()
        {
            await _localStorageService.RemoveItemAsync("authToken");
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
