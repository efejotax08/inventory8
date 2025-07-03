using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace inventory8.Helpers
{
    public static class OAuth1Helper
    {
        public static string GenerateNonce()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static string GenerateTimestamp()
        {
            var secondsSinceEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return secondsSinceEpoch.ToString();
        }

        public static string UrlEncode(string value)
        {
            return Uri.EscapeDataString(value)
                .Replace("+", "%20")
                .Replace("*", "%2A")
                .Replace("%7E", "~");
        }

        public static string GenerateSignature(string method, string url, SortedDictionary<string, string> parameters, string consumerSecret, string tokenSecret = "")
        {
            var parameterString = string.Join("&",
                parameters
                    .OrderBy(kvp => kvp.Key)
                    .Select(kvp => $"{UrlEncode(kvp.Key)}={UrlEncode(kvp.Value)}"));

            var signatureBase = string.Join("&", new[]
            {
            method.ToUpperInvariant(),
            UrlEncode(url),
            UrlEncode(parameterString)
        });

            var signingKey = $"{UrlEncode(consumerSecret)}&{UrlEncode(tokenSecret)}";

            using var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(signingKey));
            var hash = hasher.ComputeHash(Encoding.ASCII.GetBytes(signatureBase));
            return Convert.ToBase64String(hash);
        }

        public static string BuildAuthorizationHeader(SortedDictionary<string, string> parameters, string signature)
        {
            var headerParams = new SortedDictionary<string, string>(parameters)
        {
            { "oauth_signature", signature }
        };

            var header = "OAuth " + string.Join(", ",
                headerParams
                    .Where(kvp => kvp.Key.StartsWith("oauth_"))
                    .Select(kvp => $"{UrlEncode(kvp.Key)}=\"{UrlEncode(kvp.Value)}\""));

            return header;
        }
    }
}
