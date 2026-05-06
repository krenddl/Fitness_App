using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fitness_Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "FullName", "Phone" },
                values: new object[,]
                {
                    { 1, "Петров Алексей", "+7 900 111-22-33" },
                    { 2, "Иванова Анна", "+7 901 222-33-44" },
                    { 3, "Сидоров Максим", "+7 902 333-44-55" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "id_Role", "Name" },
                values: new object[,]
                {
                    { 1, "Administrator" },
                    { 2, "Client" },
                    { 3, "Trainer" },
                    { 4, "SalesManager" }
                });

            migrationBuilder.InsertData(
                table: "Trainers",
                columns: new[] { "Id", "FullName", "Specialization" },
                values: new object[,]
                {
                    { 1, "Артем Орлов", "Силовой тренинг" },
                    { 2, "Екатерина Белова", "Йога" },
                    { 3, "Иван Смирнов", "Функциональные тренировки" }
                });

            migrationBuilder.InsertData(
                table: "ChatMessages",
                columns: new[] { "Id", "ClientId", "SenderRole", "SentAt", "Text", "TrainerId" },
                values: new object[] { 1, 1, "Trainer", new DateTime(2026, 5, 6, 10, 0, 0, 0, DateTimeKind.Utc), "Не забудь про тренировку завтра", 1 });

            migrationBuilder.InsertData(
                table: "Memberships",
                columns: new[] { "Id", "ClientId", "EndDate", "ReminderSent", "StartDate", "Status", "Type" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Стандарт" },
                    { 2, 2, new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, "Премиум" }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "ClientId", "SentAt", "Text" },
                values: new object[] { 1, 1, new DateTime(2026, 5, 6, 11, 0, 0, 0, DateTimeKind.Utc), "Тренировка завтра в 18:00" });

            migrationBuilder.InsertData(
                table: "Plans",
                columns: new[] { "Id", "ClientId", "NutritionPlan", "ProgressPercent", "TrainerId", "TrainingPlan" },
                values: new object[] { 1, 1, "Белок 1.6 г/кг, вода 2 л в день", 35, 1, "3 силовые тренировки в неделю" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "id_User", "Client_Id", "Description", "Email", "Name", "Password", "Role_Id", "Trainer_Id" },
                values: new object[,]
                {
                    { 1, null, "Администратор клуба", "admin@pulse.local", "Pulse Admin", "588C55F3CE2B8569B153C5ABBF13F9F74308B88A20017CC699B835CC93195D16", 1, null },
                    { 2, null, "Менеджер по продажам", "sales@pulse.local", "Sales Lead", "588C55F3CE2B8569B153C5ABBF13F9F74308B88A20017CC699B835CC93195D16", 4, null },
                    { 3, null, "Силовой тренинг", "trainer1@pulse.local", "Артем Орлов", "588C55F3CE2B8569B153C5ABBF13F9F74308B88A20017CC699B835CC93195D16", 3, 1 },
                    { 4, 1, "Клиент фитнес-клуба", "client1@pulse.local", "Петров Алексей", "588C55F3CE2B8569B153C5ABBF13F9F74308B88A20017CC699B835CC93195D16", 2, null }
                });

            migrationBuilder.InsertData(
                table: "Visits",
                columns: new[] { "Id", "AccessType", "ClientId", "EnteredAt", "ExitedAt" },
                values: new object[] { 1, "QR", 1, new DateTime(2026, 5, 6, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 6, 13, 30, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Workouts",
                columns: new[] { "Id", "Capacity", "ClientIds", "StartAt", "Title", "TrainerId" },
                values: new object[,]
                {
                    { 1, 15, new List<int> { 1 }, new DateTime(2026, 5, 7, 18, 0, 0, 0, DateTimeKind.Utc), "Functional Core", 1 },
                    { 2, 12, new List<int>(), new DateTime(2026, 5, 8, 19, 0, 0, 0, DateTimeKind.Utc), "Yoga Flow", 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ChatMessages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Memberships",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Memberships",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Trainers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "id_User",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "id_User",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "id_User",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "id_User",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Visits",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Workouts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Workouts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "id_Role",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "id_Role",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "id_Role",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "id_Role",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Trainers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Trainers",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
