using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fitness_Client.Models;

namespace Fitness_Client.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly SessionStorageService _storage;

    public ApiService(HttpClient http, SessionStorageService storage)
    {
        _http = http;
        _storage = storage;
    }

    public Task<DemoAccountsResponseModel?> GetDemoAccounts() => _http.GetFromJsonAsync<DemoAccountsResponseModel>("api/user/demoaccounts");

    public async Task<AuthResponseModel?> GetSession()
    {
        await ApplyAuthHeaderAsync();
        return await _http.GetFromJsonAsync<AuthResponseModel>("api/user/session");
    }

    public async Task<UsersResponseModel?> GetUsers()
    {
        await ApplyAuthHeaderAsync();
        return await _http.GetFromJsonAsync<UsersResponseModel>("api/user/getallusers");
    }

    public async Task<HttpResponseMessage> CreateUser(CreateUserRequestModel model)
    {
        await ApplyAuthHeaderAsync();
        return await _http.PostAsJsonAsync("api/user/createnewuser", model);
    }

    public async Task<List<ClientModel>?> GetClients()
    {
        await ApplyAuthHeaderAsync();
        return await _http.GetFromJsonAsync<List<ClientModel>>("api/clients");
    }

    public async Task<ClientModel?> GetMyClient()
    {
        await ApplyAuthHeaderAsync();
        return await _http.GetFromJsonAsync<ClientModel>("api/clients/me");
    }

    public async Task<List<TrainerModel>?> GetTrainers()
    {
        await ApplyAuthHeaderAsync();
        return await _http.GetFromJsonAsync<List<TrainerModel>>("api/trainers");
    }

    public async Task<TrainerModel?> GetMyTrainer()
    {
        await ApplyAuthHeaderAsync();
        return await _http.GetFromJsonAsync<TrainerModel>("api/trainers/me");
    }

    public async Task<List<MembershipModel>?> GetMemberships()
    {
        await ApplyAuthHeaderAsync();
        return await _http.GetFromJsonAsync<List<MembershipModel>>("api/memberships");
    }

    public async Task<HttpResponseMessage> AddMembership(MembershipModel model)
    {
        await ApplyAuthHeaderAsync();
        return await _http.PostAsJsonAsync("api/memberships", model);
    }

    public async Task<HttpResponseMessage> FreezeMembership(int id)
    {
        await ApplyAuthHeaderAsync();
        return await _http.PostAsync($"api/memberships/{id}/freeze", null);
    }

    public async Task<HttpResponseMessage> ExtendMembership(int id, int days)
    {
        await ApplyAuthHeaderAsync();
        return await _http.PostAsync($"api/memberships/{id}/extend/{days}", null);
    }

    public async Task<List<MembershipModel>?> GetMembershipReminders()
    {
        await ApplyAuthHeaderAsync();
        return await _http.GetFromJsonAsync<List<MembershipModel>>("api/memberships/reminders");
    }

    public async Task<List<WorkoutModel>?> GetWorkouts()
    {
        await ApplyAuthHeaderAsync();
        return await _http.GetFromJsonAsync<List<WorkoutModel>>("api/workouts");
    }

    public async Task<HttpResponseMessage> AddWorkout(WorkoutModel model)
    {
        await ApplyAuthHeaderAsync();
        return await _http.PostAsJsonAsync("api/workouts", model);
    }

    public async Task<HttpResponseMessage> Enroll(int workoutId, int clientId)
    {
        await ApplyAuthHeaderAsync();
        return await _http.PostAsJsonAsync($"api/workouts/{workoutId}/enroll", new EnrollWorkoutModel { ClientId = clientId });
    }

    public async Task<List<PlanModel>?> GetPlans()
    {
        await ApplyAuthHeaderAsync();
        return await _http.GetFromJsonAsync<List<PlanModel>>("api/plans");
    }

    public async Task<HttpResponseMessage> AddPlan(PlanModel model)
    {
        await ApplyAuthHeaderAsync();
        return await _http.PostAsJsonAsync("api/plans", model);
    }

    public async Task<HttpResponseMessage> UpdateProgress(int planId, int progress)
    {
        await ApplyAuthHeaderAsync();
        return await _http.PostAsync($"api/plans/{planId}/progress/{progress}", null);
    }

    public async Task<List<VisitModel>?> GetVisits()
    {
        await ApplyAuthHeaderAsync();
        return await _http.GetFromJsonAsync<List<VisitModel>>("api/visits");
    }

    public async Task<HttpResponseMessage> Enter(EnterVisitRequestModel model)
    {
        await ApplyAuthHeaderAsync();
        return await _http.PostAsJsonAsync("api/visits/enter", model);
    }

    public async Task<HttpResponseMessage> Exit(int visitId)
    {
        await ApplyAuthHeaderAsync();
        return await _http.PostAsync($"api/visits/exit/{visitId}", null);
    }

    public async Task<List<ChatMessageModel>?> GetChat()
    {
        await ApplyAuthHeaderAsync();
        return await _http.GetFromJsonAsync<List<ChatMessageModel>>("api/communication/chat");
    }

    public async Task<HttpResponseMessage> SendChat(SendChatRequestModel model)
    {
        await ApplyAuthHeaderAsync();
        return await _http.PostAsJsonAsync("api/communication/chat", model);
    }

    public async Task<List<PushNotificationModel>?> GetPush()
    {
        await ApplyAuthHeaderAsync();
        return await _http.GetFromJsonAsync<List<PushNotificationModel>>("api/communication/notifications");
    }

    public async Task<HttpResponseMessage> SendPush(SendPushRequestModel model)
    {
        await ApplyAuthHeaderAsync();
        return await _http.PostAsJsonAsync("api/communication/notifications", model);
    }

    public async Task<ReportSummaryModel?> GetSummary()
    {
        await ApplyAuthHeaderAsync();
        return await _http.GetFromJsonAsync<ReportSummaryModel>("api/reports/summary");
    }

    private async Task ApplyAuthHeaderAsync()
    {
        var token = await _storage.GetTokenAsync();
        _http.DefaultRequestHeaders.Remove("Authorization");

        if (!string.IsNullOrWhiteSpace(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
