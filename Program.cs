using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace CRDUProject
{
    public class Program
    {
        private static string _index = "index.html";
        private static string _registorPage = "pages/register.html";
        private static string _loginPage = "pages/login.html";
        private static string _notesPage = "pages/notes.html";
        private static string _homePage = "pages/home.html";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();
            builder.Services.AddScoped<IUserManager, UserManager>();
            builder.Services.AddScoped<INotesManager, NotesManager>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,

                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,

                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey =
                            AuthOptions.GetSymmetricSecurityKey(),

                        ClockSkew = TimeSpan.FromSeconds(30)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token =
                                context.Request.Cookies["auth_token"];

                            return Task.CompletedTask;
                        }
                    };
                });

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            using (var db = new DBContext())
                db.Database.EnsureCreated();

            //--------------------------------------------------------------------------
            var homeGroup = app.MapGroup("/home");
            homeGroup.MapGet("/", [Authorize] async (HttpContext context) =>
            {
                await SendHtmlAsSync(context, _homePage);
            });

            //---------------------------------------------------------------------------
            var notesGroup = app.MapGroup("/notes");
            notesGroup.MapGet("/", [Authorize] async () => await NotesPage());
            notesGroup.MapPost("/", [Authorize] async (HttpContext context, INotesManager notesManager) =>
            {
                if (!int.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
                    return Results.Unauthorized();

                var form = context.Request.Form;
                string operation = form["operation"]!;
                string note = form["note"]!;
                string name = form["name"]!;

                switch (operation)
                {
                    case "Created":
                        await notesManager.CreatedNote(userId, name, note);
                        break;

                    case "Read":
                        string? data = await notesManager.ReadNote(userId, name);
                        return await NotesPage(data ?? "");

                    case "Updated":
                        await notesManager.UpdateNote(userId, name, note);
                        break;

                    case "Deleted": await notesManager.DeleteNote(userId, name); break;
                }

                return Results.Redirect("/notes");
            });

            //---------------------------------------------------------------------------
            var loginGroup = app.MapGroup("/login");
            loginGroup.MapGet("/", async (HttpContext context) =>
            {
                await SendHtmlAsSync(context, _loginPage);
            });

            var loginApiGroup = app.MapGroup("/login-api");
            loginApiGroup.MapPost("/", async (HttpContext context, IUserManager userManager) =>
            {
                string Email = context.Request.Form["Email"];
                string Password = context.Request.Form["Password"];

                if (!await userManager.IniUser(Email, Password))
                {
                    context.Response.Redirect("/auth/login?error=invalid_credentials");
                    return;
                }

                int id = await userManager.GetUserId(Email);
                string token = GenerateJwt(id.ToString(), Email);

                context.Response.Cookies.Append
                (
                    "auth_token",
                    token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        //Secure = !app.Environment.IsDevelopment(), //https
                        Secure = context.Request.IsHttps,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(2),
                        Path = "/"
                    }
                );
                context.Response.Redirect("/notes");
            });

            //---------------------------------------------------------------------------
            var resistrationGroup = app.MapGroup("/register");
            resistrationGroup.MapGet("/", async (HttpContext context) =>
            {
                await SendHtmlAsSync(context, _registorPage);
            });

            var registerApiGroup = app.MapGroup("/register-api");
            registerApiGroup.MapPost("/", async (HttpContext context, IUserManager userManager) => await userManager.AddUser(context));

            app.MapGet("/", async (HttpContext context) =>
            {
                if (context.User.Identity?.IsAuthenticated != true)
                    await SendHtmlAsSync(context, _index);
                else
                    context.Response.Redirect("/home");
            });

            app.Run();
        }

        private static async Task SendHtmlAsSync(HttpContext context, string pathPage)
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync("wwwroot/" + pathPage);
        }
        private static string GenerateJwt(string id, string email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Email, email)
            };

            var credentials = new SigningCredentials(
                AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private static async Task<IResult> NotesPage(string note = "")
        {
            string html = await File.ReadAllTextAsync("wwwroot/" + _notesPage);
            html = html.Replace("{{note}}", WebUtility.HtmlEncode(note));
            return Results.Content(html, "text/html; charset=utf-8");
        }
    }
}
