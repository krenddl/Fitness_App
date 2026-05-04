using Fitness_Api.Models;
using Fitness_Api.UniversalMethods;

namespace Fitness_Api.Data;

public class InMemoryStore
{
    public List<Role> Roles { get; } = new();
    public List<User> Users { get; } = new();
    public List<Session> Sessions { get; } = new();
    public List<Client> Clients { get; } = new();
    public List<Trainer> Trainers { get; } = new();
    public List<Membership> Memberships { get; } = new();
    public List<WorkoutSession> Workouts { get; } = new();
    public List<PersonalPlan> Plans { get; } = new();
    public List<Visit> Visits { get; } = new();
    public List<ChatMessage> Chat { get; } = new();
    public List<PushNotification> Notifications { get; } = new();

    private int _roleId = 1;
    private int _userId = 1;
    private int _sessionId = 1;
    private int _clientId = 1;
    private int _trainerId = 1;
    private int _membershipId = 1;
    private int _workoutId = 1;
    private int _planId = 1;
    private int _visitId = 1;
    private int _chatId = 1;
    private int _notificationId = 1;

    public InMemoryStore()
    {
        Seed();
    }

    public int NextRoleId() => _roleId++;
    public int NextUserId() => _userId++;
    public int NextSessionId() => _sessionId++;
    public int NextClientId() => _clientId++;
    public int NextTrainerId() => _trainerId++;
    public int NextMembershipId() => _membershipId++;
    public int NextWorkoutId() => _workoutId++;
    public int NextPlanId() => _planId++;
    public int NextVisitId() => _visitId++;
    public int NextChatId() => _chatId++;
    public int NextNotificationId() => _notificationId++;

    private void Seed()
    {
        SeedRoles();
        SeedTrainers();
        SeedClients();
        SeedUsers();
        SeedMemberships();
        SeedWorkouts();
        SeedPlans();
        SeedVisits();
        SeedCommunication();
    }

    private void SeedRoles()
    {
        Roles.Add(new Role { id_Role = NextRoleId(), Name = "Administrator" });
        Roles.Add(new Role { id_Role = NextRoleId(), Name = "Client" });
        Roles.Add(new Role { id_Role = NextRoleId(), Name = "Trainer" });
        Roles.Add(new Role { id_Role = NextRoleId(), Name = "SalesManager" });
    }

    private void SeedTrainers()
    {
        var trainers = new[]
        {
            ("Артем Орлов", "Силовой тренинг"),
            ("Екатерина Белова", "Йога и mobility"),
            ("Иван Смирнов", "Функциональные тренировки"),
            ("Мария Котова", "Пилатес"),
            ("Денис Шевцов", "Кроссфит"),
            ("Наталья Соколова", "Реабилитация"),
            ("Олег Власов", "Плавание"),
            ("Алина Ткачева", "Стретчинг")
        };

        foreach (var trainer in trainers)
        {
            Trainers.Add(new Trainer
            {
                Id = NextTrainerId(),
                FullName = trainer.Item1,
                Specialization = trainer.Item2
            });
        }
    }

    private void SeedClients()
    {
        var rnd = new Random(42);
        var firstNames = new[] { "Алексей", "Максим", "Дмитрий", "Кирилл", "Андрей", "Игорь", "Елена", "Анна", "Дарья", "Полина", "Ольга", "София", "Никита", "Павел", "Виктория", "Юлия" };
        var lastNames = new[] { "Петров", "Иванов", "Сидоров", "Кузнецов", "Лебедев", "Морозов", "Волков", "Семенов", "Егоров", "Новиков", "Попов", "Крылов" };

        for (var i = 0; i < 120; i++)
        {
            Clients.Add(new Client
            {
                Id = NextClientId(),
                FullName = $"{lastNames[rnd.Next(lastNames.Length)]} {firstNames[rnd.Next(firstNames.Length)]}",
                Phone = $"+7 9{rnd.Next(10, 99)} {rnd.Next(100, 999)}-{rnd.Next(10, 99)}-{rnd.Next(10, 99)}"
            });
        }
    }

    private void SeedUsers()
    {
        Users.Add(new User
        {
            id_User = NextUserId(),
            Name = "Pulse Admin",
            Email = "admin@pulse.local",
            Password = PasswordHasher.HashPassword("Demo123!"),
            Description = "Главный администратор клуба",
            Role_Id = RoleIds.Administrator
        });

        Users.Add(new User
        {
            id_User = NextUserId(),
            Name = "Sales Lead",
            Email = "sales@pulse.local",
            Password = PasswordHasher.HashPassword("Demo123!"),
            Description = "Менеджер по продажам",
            Role_Id = RoleIds.SalesManager
        });

        foreach (var trainer in Trainers)
        {
            Users.Add(new User
            {
                id_User = NextUserId(),
                Name = trainer.FullName,
                Email = $"trainer{trainer.Id}@pulse.local",
                Password = PasswordHasher.HashPassword("Demo123!"),
                Description = trainer.Specialization,
                Role_Id = RoleIds.Trainer,
                Trainer_Id = trainer.Id
            });
        }

        foreach (var client in Clients)
        {
            Users.Add(new User
            {
                id_User = NextUserId(),
                Name = client.FullName,
                Email = $"client{client.Id}@pulse.local",
                Password = PasswordHasher.HashPassword("Demo123!"),
                Description = "Клиент фитнес-клуба",
                Role_Id = RoleIds.Client,
                Client_Id = client.Id
            });
        }
    }

    private void SeedMemberships()
    {
        var rnd = new Random(43);
        var membershipTypes = new[] { "Базовый", "Стандарт", "Премиум", "Персональный" };

        foreach (var client in Clients)
        {
            var start = DateTime.UtcNow.AddDays(-rnd.Next(5, 180));
            var end = start.AddDays(rnd.Next(30, 180));
            var status = end < DateTime.UtcNow ? MembershipStatus.Expired : (rnd.NextDouble() < 0.12 ? MembershipStatus.Frozen : MembershipStatus.Active);

            Memberships.Add(new Membership
            {
                Id = NextMembershipId(),
                ClientId = client.Id,
                Type = membershipTypes[rnd.Next(membershipTypes.Length)],
                StartDate = start,
                EndDate = end,
                Status = status,
                ReminderSent = rnd.NextDouble() < 0.35
            });
        }
    }

    private void SeedWorkouts()
    {
        var rnd = new Random(44);
        var workoutTitles = new[]
        {
            "HIIT 45", "Functional Core", "Power Pump", "Yoga Flow", "Stretch & Relax", "Tabata Burn",
            "Cross Training", "Cycling Pro", "Pilates Balance", "Body Sculpt", "Boxing Cardio", "Aqua Mix"
        };

        for (var i = 0; i < 55; i++)
        {
            var workout = new WorkoutSession
            {
                Id = NextWorkoutId(),
                Title = workoutTitles[rnd.Next(workoutTitles.Length)],
                TrainerId = Trainers[rnd.Next(Trainers.Count)].Id,
                StartAt = DateTime.UtcNow.AddDays(rnd.Next(-8, 14)).AddHours(rnd.Next(7, 22)),
                Capacity = rnd.Next(10, 26)
            };

            var attendees = rnd.Next(4, workout.Capacity);
            var shuffled = Clients.OrderBy(_ => rnd.Next()).Take(attendees).Select(c => c.Id);
            workout.ClientIds.AddRange(shuffled);
            Workouts.Add(workout);
        }
    }

    private void SeedPlans()
    {
        var rnd = new Random(45);

        foreach (var client in Clients.OrderBy(_ => rnd.Next()).Take(90))
        {
            Plans.Add(new PersonalPlan
            {
                Id = NextPlanId(),
                ClientId = client.Id,
                TrainerId = Trainers[rnd.Next(Trainers.Count)].Id,
                TrainingPlan = "3 силовые + 2 кардио в неделю, фокус на технику и прогрессию нагрузки",
                NutritionPlan = "Белок 1.6 г/кг, вода 2.5л, контроль калорий с дефицитом 10%",
                ProgressPercent = rnd.Next(8, 100)
            });
        }
    }

    private void SeedVisits()
    {
        var rnd = new Random(46);

        for (var d = 0; d < 30; d++)
        {
            var day = DateTime.UtcNow.Date.AddDays(-d);
            var dayVisits = rnd.Next(18, 45);
            for (var i = 0; i < dayVisits; i++)
            {
                var enter = day.AddHours(rnd.Next(6, 23)).AddMinutes(rnd.Next(0, 60));
                Visits.Add(new Visit
                {
                    Id = NextVisitId(),
                    ClientId = Clients[rnd.Next(Clients.Count)].Id,
                    EnteredAt = enter,
                    ExitedAt = enter.AddMinutes(rnd.Next(35, 130)),
                    AccessType = rnd.NextDouble() switch
                    {
                        < 0.6 => "QR",
                        < 0.85 => "Barcode",
                        _ => "RFID"
                    }
                });
            }
        }
    }

    private void SeedCommunication()
    {
        var rnd = new Random(47);
        var phrases = new[]
        {
            "Подтверди, пожалуйста, участие на завтра",
            "Скорректировал план питания, посмотри обновления",
            "Отличный прогресс за неделю, так держать",
            "Добавил заминку и растяжку после тренировки",
            "Не забудь отметиться на рецепции"
        };

        for (var i = 0; i < 180; i++)
        {
            var client = Clients[rnd.Next(Clients.Count)];
            var trainer = Trainers[rnd.Next(Trainers.Count)];
            Chat.Add(new ChatMessage
            {
                Id = NextChatId(),
                TrainerId = trainer.Id,
                ClientId = client.Id,
                SenderRole = rnd.NextDouble() < 0.72 ? "Trainer" : "Client",
                Text = phrases[rnd.Next(phrases.Length)],
                SentAt = DateTime.UtcNow.AddMinutes(-rnd.Next(20, 40000))
            });
        }

        for (var i = 0; i < 90; i++)
        {
            var client = Clients[rnd.Next(Clients.Count)];
            Notifications.Add(new PushNotification
            {
                Id = NextNotificationId(),
                ClientId = client.Id,
                Text = i % 3 == 0
                    ? "Напоминание: тренировка через 2 часа"
                    : i % 3 == 1
                        ? "Абонемент скоро истекает, доступно продление"
                        : "Обновлен персональный план на неделю",
                SentAt = DateTime.UtcNow.AddMinutes(-rnd.Next(30, 30000))
            });
        }

        Chat.Sort((a, b) => a.SentAt.CompareTo(b.SentAt));
        Notifications.Sort((a, b) => b.SentAt.CompareTo(a.SentAt));
    }
}
