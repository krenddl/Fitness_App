using System.Net.Http.Json;
using Fitness_Client.Models;

namespace Fitness_Client.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly SessionStorageService _storage;

    public string? Token { get; private set; }
    public UserSessionModel? CurrentUser { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);

    public AuthService(HttpClient http, SessionStorageService storage)
    {
        _http = http;
        _storage = storage;
    }

    public async Task<AuthResponseModel?> LoginAsync(LoginRequestModel model)
    {
        var response = await _http.PostAsJsonAsync("api/user/authorize", model);
        AuthResponseModel? result;
        try
        {
            result = await response.Content.ReadFromJsonAsync<AuthResponseModel>();
        }
        catch
        {
            return new AuthResponseModel { Status = false, Message = "Не удалось выполнить вход. Проверьте email и пароль." };
        }

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
        Token = stored.Token;
        CurrentUser = stored.User;

        return result;
    }

    public async Task<ApiStatusResponseModel?> RegisterAsync(RegistrationRequestModel model)
    {
        var response = await _http.PostAsJsonAsync("api/user/registration", model);
        try
        {
            var result = await response.Content.ReadFromJsonAsync<ApiStatusResponseModel>();
            if (!response.IsSuccessStatusCode && string.IsNullOrWhiteSpace(result?.Message))
            {
                return new ApiStatusResponseModel { Status = false, Message = "Проверьте ФИО, телефон, email и пароль." };
            }

            return result;
        }
        catch
        {
            return new ApiStatusResponseModel { Status = false, Message = "Проверьте ФИО, телефон, email и пароль." };
        }
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

        await ClearSessionAsync();
    }

    public async Task ClearSessionAsync()
    {
        _http.DefaultRequestHeaders.Remove("Authorization");
        await _storage.ClearSessionAsync();
        Token = null;
        CurrentUser = null;
    }

    public async Task<StoredSessionModel?> GetStoredSessionAsync()
    {
        var session = await _storage.GetSessionAsync();
        Token = session?.Token;
        CurrentUser = session?.User;
        return session;
    }
}
