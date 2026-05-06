using Fitness_Api.Models;
using Fitness_Api.UniversalMethods;
using Microsoft.EntityFrameworkCore;

namespace Fitness_Api.Data;

public static class DbInitializer
{
    public static void Seed(FitnessDbContext context)
    {
        if (context.Roles.Any())
        {
            ResetSequences(context);
            return;
        }

        context.Roles.AddRange(
            new Role { id_Role = RoleIds.Administrator, Name = "Administrator" },
            new Role { id_Role = RoleIds.Client, Name = "Client" },
            new Role { id_Role = RoleIds.Trainer, Name = "Trainer" },
            new Role { id_Role = RoleIds.SalesManager, Name = "SalesManager" });

        var trainers = new List<Trainer>
        {
            new() { FullName = "Артем Орлов", Specialization = "Силовой тренинг" },
            new() { FullName = "Екатерина Белова", Specialization = "Йога" },
            new() { FullName = "Иван Смирнов", Specialization = "Функциональные тренировки" }
        };
        context.Trainers.AddRange(trainers);

        var clients = new List<Client>
        {
            new() { FullName = "Петров Алексей", Phone = "+7 900 111-22-33" },
            new() { FullName = "Иванова Анна", Phone = "+7 901 222-33-44" },
            new() { FullName = "Сидоров Максим", Phone = "+7 902 333-44-55" }
        };
        context.Clients.AddRange(clients);
        context.SaveChanges();

        context.Users.AddRange(
            new User
            {
                Name = "Pulse Admin",
                Email = "admin@pulse.local",
                Password = PasswordHasher.HashPassword("Demo123!"),
                Description = "Администратор клуба",
                Role_Id = RoleIds.Administrator
            },
            new User
            {
                Name = "Sales Lead",
                Email = "sales@pulse.local",
                Password = PasswordHasher.HashPassword("Demo123!"),
                Description = "Менеджер по продажам",
                Role_Id = RoleIds.SalesManager
            },
            new User
            {
                Name = trainers[0].FullName,
                Email = "trainer1@pulse.local",
                Password = PasswordHasher.HashPassword("Demo123!"),
                Description = trainers[0].Specialization,
                Role_Id = RoleIds.Trainer,
                Trainer_Id = trainers[0].Id
            },
            new User
            {
                Name = clients[0].FullName,
                Email = "client1@pulse.local",
                Password = PasswordHasher.HashPassword("Demo123!"),
                Description = "Клиент фитнес-клуба",
                Role_Id = RoleIds.Client,
                Client_Id = clients[0].Id
            });

        context.Memberships.AddRange(
            new Membership
            {
                ClientId = clients[0].Id,
                Type = "Стандарт",
                StartDate = DateTime.UtcNow.Date.AddDays(-10),
                EndDate = DateTime.UtcNow.Date.AddMonths(1),
                Status = MembershipStatus.Active
            },
            new Membership
            {
                ClientId = clients[1].Id,
                Type = "Премиум",
                StartDate = DateTime.UtcNow.Date.AddDays(-20),
                EndDate = DateTime.UtcNow.Date.AddDays(5),
                Status = MembershipStatus.Active
            });

        context.Workouts.AddRange(
            new WorkoutSession
            {
                Title = "Functional Core",
                TrainerId = trainers[0].Id,
                StartAt = DateTime.UtcNow.AddDays(1).Date.AddHours(18),
                Capacity = 15,
                ClientIds = new List<int> { clients[0].Id }
            },
            new WorkoutSession
            {
                Title = "Yoga Flow",
                TrainerId = trainers[1].Id,
                StartAt = DateTime.UtcNow.AddDays(2).Date.AddHours(19),
                Capacity = 12,
                ClientIds = new List<int>()
            });

        context.Plans.Add(new PersonalPlan
        {
            ClientId = clients[0].Id,
            TrainerId = trainers[0].Id,
            TrainingPlan = "3 силовые тренировки в неделю",
            NutritionPlan = "Белок 1.6 г/кг, вода 2 л в день",
            ProgressPercent = 35
        });

        context.Visits.Add(new Visit
        {
            ClientId = clients[0].Id,
            EnteredAt = DateTime.UtcNow.AddHours(-2),
            ExitedAt = DateTime.UtcNow.AddHours(-1),
            AccessType = "QR"
        });

        context.ChatMessages.Add(new ChatMessage
        {
            TrainerId = trainers[0].Id,
            ClientId = clients[0].Id,
            SenderRole = "Trainer",
            Text = "Не забудь про тренировку завтра",
            SentAt = DateTime.UtcNow.AddHours(-3)
        });

        context.Notifications.Add(new PushNotification
        {
            ClientId = clients[0].Id,
            Text = "Тренировка завтра в 18:00",
            SentAt = DateTime.UtcNow.AddHours(-1)
        });

        context.SaveChanges();
        ResetSequences(context);
    }

    private static void ResetSequences(FitnessDbContext context)
    {
        context.Database.ExecuteSqlRaw("""
            SELECT setval(pg_get_serial_sequence('"Roles"', 'id_Role'), COALESCE((SELECT MAX("id_Role") FROM "Roles"), 1));
            SELECT setval(pg_get_serial_sequence('"Users"', 'id_User'), COALESCE((SELECT MAX("id_User") FROM "Users"), 1));
            SELECT setval(pg_get_serial_sequence('"Sessions"', 'id_Session'), COALESCE((SELECT MAX("id_Session") FROM "Sessions"), 1));
            SELECT setval(pg_get_serial_sequence('"Clients"', 'Id'), COALESCE((SELECT MAX("Id") FROM "Clients"), 1));
            SELECT setval(pg_get_serial_sequence('"Trainers"', 'Id'), COALESCE((SELECT MAX("Id") FROM "Trainers"), 1));
            SELECT setval(pg_get_serial_sequence('"Memberships"', 'Id'), COALESCE((SELECT MAX("Id") FROM "Memberships"), 1));
            SELECT setval(pg_get_serial_sequence('"Workouts"', 'Id'), COALESCE((SELECT MAX("Id") FROM "Workouts"), 1));
            SELECT setval(pg_get_serial_sequence('"Plans"', 'Id'), COALESCE((SELECT MAX("Id") FROM "Plans"), 1));
            SELECT setval(pg_get_serial_sequence('"Visits"', 'Id'), COALESCE((SELECT MAX("Id") FROM "Visits"), 1));
            SELECT setval(pg_get_serial_sequence('"ChatMessages"', 'Id'), COALESCE((SELECT MAX("Id") FROM "ChatMessages"), 1));
            SELECT setval(pg_get_serial_sequence('"Notifications"', 'Id'), COALESCE((SELECT MAX("Id") FROM "Notifications"), 1));
            """);
    }
}
