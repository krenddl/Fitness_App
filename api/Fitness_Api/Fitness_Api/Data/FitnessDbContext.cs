using Fitness_Api.Models;
using Fitness_Api.UniversalMethods;
using Microsoft.EntityFrameworkCore;

namespace Fitness_Api.Data;

public class FitnessDbContext : DbContext
{
    public FitnessDbContext(DbContextOptions<FitnessDbContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<WorkoutSession> Workouts { get; set; }
    public DbSet<PersonalPlan> Plans { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<PushNotification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasKey(x => x.id_Role);
        modelBuilder.Entity<User>().HasKey(x => x.id_User);
        modelBuilder.Entity<Session>().HasKey(x => x.id_Session);

        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<Session>().Property(x => x.CreatedAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<Membership>().Property(x => x.StartDate).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<Membership>().Property(x => x.EndDate).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<WorkoutSession>().Property(x => x.StartAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<Visit>().Property(x => x.EnteredAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<Visit>().Property(x => x.ExitedAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<ChatMessage>().Property(x => x.SentAt).HasColumnType("timestamp without time zone");
        modelBuilder.Entity<PushNotification>().Property(x => x.SentAt).HasColumnType("timestamp without time zone");

        modelBuilder.Entity<User>()
            .HasOne<Role>()
            .WithMany()
            .HasForeignKey(x => x.Role_Id);

        modelBuilder.Entity<User>()
            .HasOne<Client>()
            .WithMany()
            .HasForeignKey(x => x.Client_Id)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<User>()
            .HasOne<Trainer>()
            .WithMany()
            .HasForeignKey(x => x.Trainer_Id)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Session>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.User_Id);

        modelBuilder.Entity<Membership>()
            .HasOne<Client>()
            .WithMany()
            .HasForeignKey(x => x.ClientId);

        modelBuilder.Entity<WorkoutSession>()
            .HasOne<Trainer>()
            .WithMany()
            .HasForeignKey(x => x.TrainerId);

        modelBuilder.Entity<PersonalPlan>()
            .HasOne<Client>()
            .WithMany()
            .HasForeignKey(x => x.ClientId);

        modelBuilder.Entity<PersonalPlan>()
            .HasOne<Trainer>()
            .WithMany()
            .HasForeignKey(x => x.TrainerId);

        modelBuilder.Entity<Visit>()
            .HasOne<Client>()
            .WithMany()
            .HasForeignKey(x => x.ClientId);

        modelBuilder.Entity<ChatMessage>()
            .HasOne<Client>()
            .WithMany()
            .HasForeignKey(x => x.ClientId);

        modelBuilder.Entity<ChatMessage>()
            .HasOne<Trainer>()
            .WithMany()
            .HasForeignKey(x => x.TrainerId);

        modelBuilder.Entity<PushNotification>()
            .HasOne<Client>()
            .WithMany()
            .HasForeignKey(x => x.ClientId);

        modelBuilder.Entity<Role>().HasData(
            new Role { id_Role = RoleIds.Administrator, Name = "Administrator" },
            new Role { id_Role = RoleIds.Client, Name = "Client" },
            new Role { id_Role = RoleIds.Trainer, Name = "Trainer" },
            new Role { id_Role = RoleIds.SalesManager, Name = "SalesManager" });

        modelBuilder.Entity<Client>().HasData(
            new Client { Id = 1, FullName = "Петров Алексей", Phone = "+7 900 111-22-33" },
            new Client { Id = 2, FullName = "Иванова Анна", Phone = "+7 901 222-33-44" },
            new Client { Id = 3, FullName = "Сидоров Максим", Phone = "+7 902 333-44-55" });

        modelBuilder.Entity<Trainer>().HasData(
            new Trainer { Id = 1, FullName = "Артем Орлов", Specialization = "Силовой тренинг" },
            new Trainer { Id = 2, FullName = "Екатерина Белова", Specialization = "Йога" },
            new Trainer { Id = 3, FullName = "Иван Смирнов", Specialization = "Функциональные тренировки" });

        modelBuilder.Entity<User>().HasData(
            new User
            {
                id_User = 1,
                Name = "Pulse Admin",
                Email = "admin@pulse.local",
                Password = PasswordHasher.HashPassword("Demo123!"),
                Description = "Администратор клуба",
                Role_Id = RoleIds.Administrator
            },
            new User
            {
                id_User = 2,
                Name = "Sales Lead",
                Email = "sales@pulse.local",
                Password = PasswordHasher.HashPassword("Demo123!"),
                Description = "Менеджер по продажам",
                Role_Id = RoleIds.SalesManager
            },
            new User
            {
                id_User = 3,
                Name = "Артем Орлов",
                Email = "trainer1@pulse.local",
                Password = PasswordHasher.HashPassword("Demo123!"),
                Description = "Силовой тренинг",
                Role_Id = RoleIds.Trainer,
                Trainer_Id = 1
            },
            new User
            {
                id_User = 4,
                Name = "Петров Алексей",
                Email = "client1@pulse.local",
                Password = PasswordHasher.HashPassword("Demo123!"),
                Description = "Клиент фитнес-клуба",
                Role_Id = RoleIds.Client,
                Client_Id = 1
            });

        modelBuilder.Entity<Membership>().HasData(
            new Membership
            {
                Id = 1,
                ClientId = 1,
                Type = "Стандарт",
                StartDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                Status = MembershipStatus.Active
            },
            new Membership
            {
                Id = 2,
                ClientId = 2,
                Type = "Премиум",
                StartDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 5, 20, 0, 0, 0, DateTimeKind.Utc),
                Status = MembershipStatus.Active
            });

        modelBuilder.Entity<WorkoutSession>().HasData(
            new WorkoutSession
            {
                Id = 1,
                Title = "Functional Core",
                TrainerId = 1,
                StartAt = new DateTime(2026, 5, 7, 18, 0, 0, DateTimeKind.Utc),
                Capacity = 15,
                ClientIds = new List<int> { 1 }
            },
            new WorkoutSession
            {
                Id = 2,
                Title = "Yoga Flow",
                TrainerId = 2,
                StartAt = new DateTime(2026, 5, 8, 19, 0, 0, DateTimeKind.Utc),
                Capacity = 12,
                ClientIds = new List<int>()
            });

        modelBuilder.Entity<PersonalPlan>().HasData(new PersonalPlan
        {
            Id = 1,
            ClientId = 1,
            TrainerId = 1,
            TrainingPlan = "3 силовые тренировки в неделю",
            NutritionPlan = "Белок 1.6 г/кг, вода 2 л в день",
            ProgressPercent = 35
        });

        modelBuilder.Entity<Visit>().HasData(new Visit
        {
            Id = 1,
            ClientId = 1,
            EnteredAt = new DateTime(2026, 5, 6, 12, 0, 0, DateTimeKind.Utc),
            ExitedAt = new DateTime(2026, 5, 6, 13, 30, 0, DateTimeKind.Utc),
            AccessType = "QR"
        });

        modelBuilder.Entity<ChatMessage>().HasData(new ChatMessage
        {
            Id = 1,
            TrainerId = 1,
            ClientId = 1,
            SenderRole = "Trainer",
            Text = "Не забудь про тренировку завтра",
            SentAt = new DateTime(2026, 5, 6, 10, 0, 0, DateTimeKind.Utc)
        });

        modelBuilder.Entity<PushNotification>().HasData(new PushNotification
        {
            Id = 1,
            ClientId = 1,
            Text = "Тренировка завтра в 18:00",
            SentAt = new DateTime(2026, 5, 6, 11, 0, 0, DateTimeKind.Utc)
        });
    }
}
