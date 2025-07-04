using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace inventory8.Helpers
{
    public static class OAuth1Helper
    {
        public static string GenerateNonce() => Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("+", "").Replace("/", "").Replace("=", "");

        public static string GenerateTimestamp() => ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();

        public static string UrlEncode(string value) => HttpUtility.UrlEncode(value).Replace("+", "%20").Replace("*", "%2A").Replace("%7E", "~");

        public static string GenerateSignature(string httpMethod, string url, string parameters, string consumerSecret, string tokenSecret = "")
        {
            string signatureBase = $"{httpMethod.ToUpper()}&{UrlEncode(url)}&{UrlEncode(parameters)}";
            string key = $"{UrlEncode(consumerSecret)}&{UrlEncode(tokenSecret)}";

            using var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(key));
            var hash = hasher.ComputeHash(Encoding.ASCII.GetBytes(signatureBase));
            return Convert.ToBase64String(hash);
        }
    }
}
