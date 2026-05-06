using Fitness_Api.Data;
using Fitness_Api.Interfaces;
using Fitness_Api.Models;
using Fitness_Api.Requests;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fitness_Api.Services;

public class UserServices : IUserServices
{
    private readonly FitnessDbContext _context;
    private readonly JwtGenerator _jwtGenerator;
    private readonly SessionResolver _sessionResolver;

    public UserServices(FitnessDbContext context, JwtGenerator jwtGenerator, SessionResolver sessionResolver)
    {
        _context = context;
        _jwtGenerator = jwtGenerator;
        _sessionResolver = sessionResolver;
    }

    public async Task<IActionResult> Registration(Registration regUser)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == regUser.Email);
        if (existingUser is not null)
        {
            return new BadRequestObjectResult(new
            {
                status = false,
                message = "Пользователь с таким email уже существует"
            });
        }

        var client = new Client
        {
            FullName = regUser.Name,
            Phone = regUser.Phone
        };

        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();

        var user = new User
        {
            Email = regUser.Email,
            Password = PasswordHasher.HashPassword(regUser.Password),
            Name = regUser.Name,
            Description = regUser.Description,
            Role_Id = RoleIds.Client,
            Client_Id = client.Id
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return new OkObjectResult(new
        {
            status = true,
            message = "Регистрация прошла успешно"
        });
    }

    public async Task<IActionResult> Authorize(Auth authUser)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == authUser.Email);
        if (user is null || !PasswordHasher.Verify(authUser.Password, user.Password))
        {
            return new UnauthorizedObjectResult(new
            {
                status = false,
                message = "Неверный email или пароль"
            });
        }

        var oldSessions = await _context.Sessions.Where(x => x.User_Id == user.id_User).ToListAsync();
        _context.Sessions.RemoveRange(oldSessions);

        var token = _jwtGenerator.GenerateToken(user);
        await _context.Sessions.AddAsync(new Session
        {
            Token = token,
            User_Id = user.id_User,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        });

        await _context.SaveChangesAsync();

        return new OkObjectResult(new
        {
            status = true,
            token,
            user = BuildUserResult(user)
        });
    }

    public async Task<IActionResult> Logout(string token)
    {
        var session = _sessionResolver.GetSession(token);
        if (session is null)
        {
            return new UnauthorizedObjectResult(new
            {
                status = false,
                message = "Сессия не найдена"
            });
        }

        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();

        return new OkObjectResult(new
        {
            status = true,
            message = "Выход выполнен"
        });
    }

    public Task<IActionResult> Session(string token)
    {
        var user = _sessionResolver.GetUser(token);
        var session = _sessionResolver.GetSession(token);

        if (user is null || session is null)
        {
            return Task.FromResult<IActionResult>(new UnauthorizedObjectResult(new
            {
                status = false,
                message = "Сессия не найдена"
            }));
        }

        return Task.FromResult<IActionResult>(new OkObjectResult(new
        {
            status = true,
            token = session.Token,
            user = BuildUserResult(user)
        }));
    }

    public async Task<IActionResult> Profile(Profile profile, string token)
    {
        var user = _sessionResolver.GetUser(token);
        if (user is null)
        {
            return new UnauthorizedObjectResult(new
            {
                status = false,
                message = "Сессия не найдена"
            });
        }

        var existingEmail = await _context.Users.FirstOrDefaultAsync(x => x.Email == profile.Email && x.id_User != user.id_User);
        if (existingEmail is not null)
        {
            return new BadRequestObjectResult(new
            {
                status = false,
                message = "Этот email уже занят"
            });
        }

        user.Name = profile.Name;
        user.Email = profile.Email;
        user.Description = profile.Description;

        if (!string.IsNullOrWhiteSpace(profile.Password))
        {
            user.Password = PasswordHasher.HashPassword(profile.Password);
        }

        if (user.Client_Id.HasValue)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.Id == user.Client_Id.Value);
            if (client is not null)
            {
                client.FullName = profile.Name;
                if (!string.IsNullOrWhiteSpace(profile.Phone))
                {
                    client.Phone = profile.Phone;
                }
            }
        }

        if (user.Trainer_Id.HasValue)
        {
            var trainer = await _context.Trainers.FirstOrDefaultAsync(x => x.Id == user.Trainer_Id.Value);
            if (trainer is not null)
            {
                trainer.FullName = profile.Name;
            }
        }

        await _context.SaveChangesAsync();

        return new OkObjectResult(new
        {
            status = true,
            user = BuildUserResult(user)
        });
    }

    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users
            .OrderBy(x => x.Name)
            .Select(user => new
            {
                user.id_User,
                user.Name,
                user.Email,
                user.Description,
                user.Role_Id,
                Role = _context.Roles.FirstOrDefault(role => role.id_Role == user.Role_Id)!.Name,
                user.Client_Id,
                user.Trainer_Id
            })
            .ToListAsync();

        return new OkObjectResult(new
        {
            status = true,
            users
        });
    }

    public async Task<IActionResult> CreateNewUser(CreateNewUser regUser)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == regUser.Email);
        if (existingUser is not null)
        {
            return new BadRequestObjectResult(new
            {
                status = false,
                message = "Пользователь с таким email уже существует"
            });
        }

        int? clientId = null;
        int? trainerId = null;

        if (regUser.Role_Id == RoleIds.Client)
        {
            var client = new Client
            {
                FullName = regUser.Name,
                Phone = regUser.Phone ?? string.Empty
            };

            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
            clientId = client.Id;
        }

        if (regUser.Role_Id == RoleIds.Trainer)
        {
            var trainer = new Trainer
            {
                FullName = regUser.Name,
                Specialization = regUser.Specialization ?? "Общая подготовка"
            };

            await _context.Trainers.AddAsync(trainer);
            await _context.SaveChangesAsync();
            trainerId = trainer.Id;
        }

        var user = new User
        {
            Email = regUser.Email,
            Password = PasswordHasher.HashPassword(regUser.Password),
            Name = regUser.Name,
            Description = regUser.Description,
            Role_Id = regUser.Role_Id,
            Client_Id = clientId,
            Trainer_Id = trainerId
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return new OkObjectResult(new
        {
            status = true,
            user = BuildUserResult(user)
        });
    }

    public Task<IActionResult> GetDemoAccounts()
    {
        var demoAccounts = new[]
        {
            new { role = "Administrator", email = "admin@pulse.local", password = "Demo123!" },
            new { role = "SalesManager", email = "sales@pulse.local", password = "Demo123!" },
            new { role = "Trainer", email = "trainer1@pulse.local", password = "Demo123!" },
            new { role = "Client", email = "client1@pulse.local", password = "Demo123!" }
        };

        return Task.FromResult<IActionResult>(new OkObjectResult(new
        {
            status = true,
            accounts = demoAccounts
        }));
    }

    private object BuildUserResult(User user)
    {
        return new
        {
            user.id_User,
            user.Name,
            user.Email,
            user.Description,
            user.Role_Id,
            Role = _sessionResolver.GetRoleName(user.Role_Id),
            user.Client_Id,
            user.Trainer_Id
        };
    }
}
