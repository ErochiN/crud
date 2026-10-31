namespace CRDUProject
{
    public class Program
    {
        private static string _index = "index.html";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.Run();

            //app.MapGet("/", async (HttpContext context) => 
            //{
            //    Console.WriteLine(context.Request.Path);
            //    await SendHtmlAsync(context, _index);
            //});

            //app.MapGet("/css/{fileName}", async (HttpContext context, string fileName) => 
            //{ 
            //    Console.WriteLine(context.Request.Path);
            //    await SendCssAsync(context, fileName);
            //});

            //app.MapGet("/img/{fileName}", async (HttpContext context, string fileName) =>
            //{
            //    Console.WriteLine(context.Request.Path);
            //    await SendImgAsync(context, fileName);
            //});

            //app.MapGet("/fonts/{fileName}", async (HttpContext context, string fileName) =>
            //{
            //    Console.WriteLine(context.Request.Path);
            //    await SendFontAsync(context, fileName);
            //});

            //app.MapGet("/icons/{fileName}", async (HttpContext context, string fileName) =>
            //{
            //    Console.WriteLine(context.Request.Path);
            //    await SendIconsAsync(context, fileName);
            //});

            //app.Run();
        }

        private static async Task SendHtmlAsync(HttpContext context, string pathPage)
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync("wwwroot/" + pathPage); 
        }
        private static async Task SendCssAsync(HttpContext context, string pathPage)
        {
            context.Response.ContentType = "text/css";
            await context.Response.SendFileAsync("wwwroot/css/" + pathPage);
        }
        private static async Task SendImgAsync(HttpContext context, string pathPage)
        {
            string extension = Path.GetExtension(pathPage).ToLower();

            context.Response.ContentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                ".webp" => "image/webp",
                ".ico" => "image/x-icon",
                ".webmanifest" => "application/manifest+json",
                _ => "application/octet-stream"
            };

            await context.Response.SendFileAsync("wwwroot/img/" + pathPage);
        }
        private static async Task SendFontAsync(HttpContext context, string pathPage)
        {
            context.Response.ContentType = "font/ttf";
            await context.Response.SendFileAsync("wwwroot/fonts/" + pathPage);
        }
        private static async Task SendIconsAsync(HttpContext context, string pathPage)
        {
            string extension = Path.GetExtension(pathPage).ToLower();

            context.Response.ContentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                ".webp" => "image/webp",
                ".ico" => "image/x-icon",
                ".webmanifest" => "application/manifest+json",
                _ => "application/octet-stream"
            };

            await context.Response.SendFileAsync("wwwroot/icons/" + pathPage);
        }
    }
}
