using System.Net.Http.Json;
using Fitness_Client.Models;

namespace Fitness_Client.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly SessionStorageService _storage;
    private readonly CustomAuthStateProvider _authStateProvider;

    public AuthService(HttpClient http, SessionStorageService storage, CustomAuthStateProvider authStateProvider)
    {
        _http = http;
        _storage = storage;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthResponseModel?> LoginAsync(LoginRequestModel model)
    {
        var response = await _http.PostAsJsonAsync("api/user/authorize", model);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseModel>();

        if (!response.IsSuccessStatusCode || result is null || !result.Status || string.IsNullOrWhiteSpace(result.Token))
        {
            return result;
        }

        var stored = new StoredSessionModel
        {
            Token = result.Token,
            User = result.User
        };

        await _storage.SaveSessionAsync(stored);
        _authStateProvider.NotifyUserAuthentication(stored);

        return result;
    }

    public async Task<ApiStatusResponseModel?> RegisterAsync(RegistrationRequestModel model)
    {
        var response = await _http.PostAsJsonAsync("api/user/registration", model);
        return await response.Content.ReadFromJsonAsync<ApiStatusResponseModel>();
    }

    public async Task LogoutAsync()
    {
        var session = await _storage.GetSessionAsync();
        if (!string.IsNullOrWhiteSpace(session?.Token))
        {
            _http.DefaultRequestHeaders.Remove("Authorization");
            _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {session.Token}");
            try
            {
                await _http.PostAsync("api/user/logout", null);
            }
            catch
            {
            }
        }

        _http.DefaultRequestHeaders.Remove("Authorization");
        await _storage.ClearSessionAsync();
        _authStateProvider.NotifyUserLogout();
    }

    public async Task<StoredSessionModel?> GetStoredSessionAsync()
    {
        return await _storage.GetSessionAsync();
    }
}
