using Fitness_Api.Data;
using Fitness_Api.Hubs;
using Fitness_Api.Interfaces;
using Fitness_Api.Services;
using Fitness_Api.UniversalMethods;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 1024 * 1024 * 10;
});

builder.Services.AddDbContext<FitnessDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("FitnessDbString")));

builder.Services.AddSingleton<JwtGenerator>();
builder.Services.AddScoped<SessionResolver>();

builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IClientServices, ClientServices>();
builder.Services.AddScoped<ITrainerServices, TrainerServices>();
builder.Services.AddScoped<IMembershipServices, MembershipServices>();
builder.Services.AddScoped<IWorkoutServices, WorkoutServices>();
builder.Services.AddScoped<IPlanServices, PlanServices>();
builder.Services.AddScoped<IVisitServices, VisitServices>();
builder.Services.AddScoped<ICommunicationServices, CommunicationServices>();
builder.Services.AddScoped<IReportServices, ReportServices>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
