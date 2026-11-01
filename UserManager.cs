using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CRDUProject
{
    internal class UserManager : IUserManager
    {
        public async Task<IResult> AddUser(HttpContext context)
        {
            string name = context.Request.Form["Name"];
            string email = context.Request.Form["Email"];
            string password = context.Request.Form["Password"];
            string confirmPassword = context.Request.Form["confirmPassword"];

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return Results.BadRequest(new { error = "Заполните все поля" });
            if (password != confirmPassword)
                return Results.BadRequest(new { error = "Пароли не совпадают" });

            using (var dbContext = new DBContext())
            {
                if (dbContext.Users.Any(u => u.Email == email))
                    return Results.Conflict(new { error = "Пользователь с таким email уже есть" });



                var user = new Users
                {
                    Name = name,
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(password)
                };

                await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();
            }

            return Results.Ok();
        }

        public async Task<bool> IniUser(string email, string password)
        {
            using (var dbContext = new DBContext())
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(p => p.Email == email);

                if (user == null) return false;

                return BCrypt.Net.BCrypt.Verify(password, user.Password);
            }
            ;
        }

        public async Task<int> GetUserId(string email)
        {
            using (var dbContext = new DBContext())
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                var id = user?.Id;

                return (int)id;
            }
        }
    }
}