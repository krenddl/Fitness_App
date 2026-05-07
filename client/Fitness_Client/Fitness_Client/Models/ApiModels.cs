namespace Fitness_Client.Models;

public class ApiStatusResponseModel { public bool Status { get; set; } public string Message { get; set; } = string.Empty; }
public class LoginRequestModel { public string Email { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }
public class RegistrationRequestModel { public string Name { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; public string Phone { get; set; } = string.Empty; public string? Description { get; set; } }
public class UserSessionModel { public int Id_User { get; set; } public string Name { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string? Description { get; set; } public int Role_Id { get; set; } public string Role { get; set; } = string.Empty; public int? Client_Id { get; set; } public int? Trainer_Id { get; set; } }
public class AuthResponseModel { public bool Status { get; set; } public string Token { get; set; } = string.Empty; public UserSessionModel User { get; set; } = new(); public string Message { get; set; } = string.Empty; }
public class StoredSessionModel { public string Token { get; set; } = string.Empty; public UserSessionModel User { get; set; } = new(); }
public class DemoAccountModel { public string Role { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }
public class DemoAccountsResponseModel { public bool Status { get; set; } public List<DemoAccountModel> Accounts { get; set; } = new(); }
public class UsersResponseModel { public bool Status { get; set; } public List<UserSessionModel> Users { get; set; } = new(); }
public class CreateUserRequestModel { public string Name { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; public int Role_Id { get; set; } public string? Description { get; set; } public string? Phone { get; set; } public string? Specialization { get; set; } }
public class ClientModel { public int Id { get; set; } public string FullName { get; set; } = string.Empty; public string Phone { get; set; } = string.Empty; }
public class TrainerModel { public int Id { get; set; } public string FullName { get; set; } = string.Empty; public string Specialization { get; set; } = string.Empty; }
public class MembershipModel { public int Id { get; set; } public int ClientId { get; set; } public string Type { get; set; } = string.Empty; public DateTime StartDate { get; set; } public DateTime EndDate { get; set; } public string Status { get; set; } = "Active"; }
public class WorkoutModel { public int Id { get; set; } public string Title { get; set; } = string.Empty; public int TrainerId { get; set; } public DateTime StartAt { get; set; } public int Capacity { get; set; } public List<int> ClientIds { get; set; } = new(); }
public class PlanModel { public int Id { get; set; } public int ClientId { get; set; } public int TrainerId { get; set; } public string TrainingPlan { get; set; } = string.Empty; public string NutritionPlan { get; set; } = string.Empty; public int ProgressPercent { get; set; } }
public class VisitModel { public int Id { get; set; } public int ClientId { get; set; } public DateTime EnteredAt { get; set; } public DateTime? ExitedAt { get; set; } public string AccessType { get; set; } = "QR"; }
public class ChatMessageModel { public int Id { get; set; } public int TrainerId { get; set; } public int ClientId { get; set; } public string SenderRole { get; set; } = "Trainer"; public string Text { get; set; } = string.Empty; public DateTime SentAt { get; set; } }
public class PushNotificationModel { public int Id { get; set; } public int ClientId { get; set; } public string Text { get; set; } = string.Empty; public DateTime SentAt { get; set; } }
public class ReportSummaryModel { public int Active { get; set; } public int Frozen { get; set; } public int Attendance { get; set; } public double Occupancy { get; set; } public int Revenue { get; set; } public int PlansInProgress { get; set; } }
public class EnrollWorkoutModel { public int ClientId { get; set; } }
public class EnterVisitRequestModel { public int ClientId { get; set; } public string AccessType { get; set; } = "QR"; }
public class SendChatRequestModel { public int TrainerId { get; set; } public int ClientId { get; set; } public string Text { get; set; } = string.Empty; }
public class SendPushRequestModel { public int ClientId { get; set; } public string Text { get; set; } = string.Empty; }
