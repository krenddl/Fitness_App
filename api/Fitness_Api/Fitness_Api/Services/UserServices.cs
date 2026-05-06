using Fitness_Api.Data;
using Fitness_Api.Interfaces;
using Fitness_Api.Models;
using Fitness_Api.Requests;
using Fitness_Api.UniversalMethods;
using Microsoft.AspNetCore.Mvc;

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

    public Task<IActionResult> Registration(Registration regUser)
    {
        if (_context.Users.Any(x => x.Email.ToLower() == regUser.Email.ToLower()))
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new
            {
                status = false,
                message = "Пользователь с таким email уже существует"
            }));
        }

        var client = new Client
        {
            FullName = regUser.Name,
            Phone = regUser.Phone
        };

        _context.Clients.Add(client);
        _context.SaveChanges();

        var user = new User
        {
            Name = regUser.Name,
            Email = regUser.Email,
            Password = PasswordHasher.HashPassword(regUser.Password),
            Description = regUser.Description,
            Role_Id = RoleIds.Client,
            Client_Id = client.Id
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Task.FromResult<IActionResult>(new OkObjectResult(new
        {
            status = true,
            message = "Регистрация прошла успешно"
        }));
    }

    public Task<IActionResult> Authorize(Auth authUser)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email.ToLower() == authUser.Email.ToLower());

        if (user is null || !PasswordHasher.Verify(authUser.Password, user.Password))
        {
            return Task.FromResult<IActionResult>(new UnauthorizedObjectResult(new
            {
                status = false,
                message = "Неверный email или пароль"
            }));
        }

        _context.Sessions.RemoveRange(_context.Sessions.Where(x => x.User_Id == user.id_User));

        var token = _jwtGenerator.GenerateToken(user);
        _context.Sessions.Add(new Session
        {
            Token = token,
            User_Id = user.id_User,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        });
        _context.SaveChanges();

        return Task.FromResult<IActionResult>(new OkObjectResult(new
        {
            status = true,
            token,
            user = BuildUserResult(user)
        }));
    }

    public Task<IActionResult> Logout(string token)
    {
        var session = _sessionResolver.GetSession(token);
        if (session is null)
        {
            return Task.FromResult<IActionResult>(new UnauthorizedObjectResult(new
            {
                status = false,
                message = "Сессия не найдена"
            }));
        }

        _context.Sessions.Remove(session);
        _context.SaveChanges();

        return Task.FromResult<IActionResult>(new OkObjectResult(new
        {
            status = true,
            message = "Выход выполнен"
        }));
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

    public Task<IActionResult> Profile(Profile profile, string token)
    {
        var user = _sessionResolver.GetUser(token);
        if (user is null)
        {
            return Task.FromResult<IActionResult>(new UnauthorizedObjectResult(new
            {
                status = false,
                message = "Сессия не найдена"
            }));
        }

        var existingEmail = _context.Users.FirstOrDefault(x =>
            x.Email.ToLower() == profile.Email.ToLower() && x.id_User != user.id_User);

        if (existingEmail is not null)
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new
            {
                status = false,
                message = "Этот email уже занят"
            }));
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
            var client = _context.Clients.FirstOrDefault(x => x.Id == user.Client_Id.Value);
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
            var trainer = _context.Trainers.FirstOrDefault(x => x.Id == user.Trainer_Id.Value);
            if (trainer is not null)
            {
                trainer.FullName = profile.Name;
            }
        }

        _context.SaveChanges();

        return Task.FromResult<IActionResult>(new OkObjectResult(new
        {
            status = true,
            user = BuildUserResult(user)
        }));
    }

    public Task<IActionResult> GetAllUsers()
    {
        var users = _context.Users
            .OrderBy(x => x.Name)
            .ToList()
            .Select(BuildUserResult)
            .ToList();

        return Task.FromResult<IActionResult>(new OkObjectResult(new
        {
            status = true,
            users
        }));
    }

    public Task<IActionResult> CreateNewUser(CreateNewUser regUser)
    {
        if (_context.Users.Any(x => x.Email.ToLower() == regUser.Email.ToLower()))
        {
            return Task.FromResult<IActionResult>(new BadRequestObjectResult(new
            {
                status = false,
                message = "Пользователь с таким email уже существует"
            }));
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
            _context.Clients.Add(client);
            _context.SaveChanges();
            clientId = client.Id;
        }

        if (regUser.Role_Id == RoleIds.Trainer)
        {
            var trainer = new Trainer
            {
                FullName = regUser.Name,
                Specialization = regUser.Specialization ?? "Общая подготовка"
            };
            _context.Trainers.Add(trainer);
            _context.SaveChanges();
            trainerId = trainer.Id;
        }

        var user = new User
        {
            Name = regUser.Name,
            Email = regUser.Email,
            Password = PasswordHasher.HashPassword(regUser.Password),
            Description = regUser.Description,
            Role_Id = regUser.Role_Id,
            Client_Id = clientId,
            Trainer_Id = trainerId
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Task.FromResult<IActionResult>(new OkObjectResult(new
        {
            status = true,
            user = BuildUserResult(user)
        }));
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
