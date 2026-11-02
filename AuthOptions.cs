using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CRDUProject
{
    internal class AuthOptions
    {
        public const string ISSUER = "se6350235";
        public const string AUDIENCE = "MyAuthClient";
        public static readonly string KEY = GetKey();
        public const int LIFETIME = 60;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        }

        public static string GetKey()
        {

            string? resultKey;
#if !DEBUG
            string? key = Environment.GetEnvironmentVariable("JWT_KEY");
            resultKey = key;
#endif
#if DEBUG
            using (FileStream fs = new FileStream(".key", FileMode.Open))
            {
                using (StreamReader stream = new StreamReader(fs))
                {
                    resultKey = stream.ReadToEnd();
                }
            }
#endif
            resultKey = resultKey?.Trim();
            if (string.IsNullOrEmpty(resultKey) || resultKey.Length < 32)
                throw new InvalidOperationException(
                    "JWT key is missing or shorter than 32 characters.");

            return resultKey;
        }
    }
}