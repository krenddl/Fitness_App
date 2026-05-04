using System.Text.Json;
using Fitness_Client.Models;
using Microsoft.JSInterop;

namespace Fitness_Client.Services;

public class SessionStorageService
{
    private const string SessionKey = "fitness-session";
    private readonly IJSRuntime _jsRuntime;

    public SessionStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SaveSessionAsync(StoredSessionModel session)
    {
        var json = JsonSerializer.Serialize(session);
        await _jsRuntime.InvokeVoidAsync("authStorage.set", SessionKey, json);
    }

    public async Task<StoredSessionModel?> GetSessionAsync()
    {
        var json = await _jsRuntime.InvokeAsync<string?>("authStorage.get", SessionKey);
        return string.IsNullOrWhiteSpace(json)
            ? null
            : JsonSerializer.Deserialize<StoredSessionModel>(json);
    }

    public async Task<string?> GetTokenAsync()
    {
        return (await GetSessionAsync())?.Token;
    }

    public async Task ClearSessionAsync()
    {
        await _jsRuntime.InvokeVoidAsync("authStorage.remove", SessionKey);
    }
}
