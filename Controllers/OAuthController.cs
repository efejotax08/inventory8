using Google.Apis.Auth.OAuth2;
using inventory8.DatabaseContext;
using inventory8.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace inventory8.Controllers
{
    [ApiController]
    [Route("api/oauth")]
    public class OAuthController : ControllerBase
    {
        private readonly InventoryContext _context;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _callbackUrl;
        private readonly string _jwtKey;
        public OAuthController(IMemoryCache cache, IHttpClientFactory factory, IConfiguration config)
        {
            _httpClient = factory.CreateClient();
            _cache = cache;
            _jwtKey = config["Jwt:Key"];
            _clientId = config["CleverCloud:ClientId"];
            _clientSecret = config["CleverCloud:ClientSecret"];
            _callbackUrl = "https://app-aa670142-e807-4859-be5b-19fad2323953.cleverapps.io/api/oauth/callback"; // o también podrías sacarlo de config si quieres
        }
        [HttpPost("request_token")]
        public async Task<IActionResult> RequestToken()
        {
           // var consumerKey = "Iq8B1YKaUU88OJXoAo92ooLC4pX06c";
            //var consumerSecret = "OF0YPVYjvfJJk6uQjUYLRbRx1U1pfG";
           // var callbackUrl = "https://app-aa670142-e807-4859-be5b-19fad2323953.cleverapps.io/api/oauth/callback";

            var oauthNonce = OAuth1Helper.GenerateNonce();
            var oauthTimestamp = OAuth1Helper.GenerateTimestamp();
            var oauthVersion = "1.0";
            var oauthSignatureMethod = "PLAINTEXT";

            // PLAINTEXT signature
            var oauthSignature = $"{OAuth1Helper.UrlEncode(_clientSecret)}&";

            var authHeader = $"OAuth " +
         $"oauth_callback=\"{OAuth1Helper.UrlEncode(_callbackUrl)}\", " +
         $"oauth_consumer_key=\"{OAuth1Helper.UrlEncode(_clientId)}\", " +
         $"oauth_nonce=\"{OAuth1Helper.UrlEncode(oauthNonce)}\", " +
         $"oauth_signature=\"{OAuth1Helper.UrlEncode(oauthSignature)}\", " +
         $"oauth_signature_method=\"{OAuth1Helper.UrlEncode(oauthSignatureMethod)}\", " +
         $"oauth_timestamp=\"{OAuth1Helper.UrlEncode(oauthTimestamp)}\", " +
         $"oauth_version=\"{OAuth1Helper.UrlEncode(oauthVersion)}\"";

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.clever-cloud.com/v2/oauth/request_token");
            request.Headers.Add("Authorization", authHeader);
            request.Content = new StringContent(""); // El cuerpo puede estar vacío, ya que todo va en el header
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, content);
            }
            var queryParams = HttpUtility.ParseQueryString(content);
            var token = queryParams["oauth_token"];
            var secret = queryParams["oauth_token_secret"];
            _cache.Set("oauth:req:" + token, secret, TimeSpan.FromMinutes(20));
            return Ok(new
            {
                message = "Token generado",
                token,
                secret,
                authUrl = $"https://api.clever-cloud.com/v2/oauth/authorize?oauth_token={token}"
            });
            //return Ok(content);
        }


        [HttpPost("access-token")]
        public async Task<IActionResult> AccessToken([FromQuery] string token, [FromQuery] string verifier)
        {
            var consumerKey = "Iq8B1YKaUU88OJXoAo92ooLC4pX06c";
            var consumerSecret = "OF0YPVYjvfJJk6uQjUYLRbRx1U1pfG";
            var tokenSecret = ""; // Solo si Clever te lo devolvió antes (a veces no lo hacen en request_token, revisar)

            var requestUrl = "https://api.clever-cloud.com/v2/oauth/access_token";

            var oauthNonce = OAuth1Helper.GenerateNonce();
            var oauthTimestamp = OAuth1Helper.GenerateTimestamp();
            var oauthSignatureMethod = "PLAINTEXT";
            var oauthVersion = "1.0";

            var signature = $"{OAuth1Helper.UrlEncode(consumerSecret)}&{OAuth1Helper.UrlEncode(tokenSecret)}";

            var authHeader = $"OAuth " +
                $"oauth_consumer_key=\"{OAuth1Helper.UrlEncode(consumerKey)}\", " +
                $"oauth_token=\"{OAuth1Helper.UrlEncode(token)}\", " +
                $"oauth_verifier=\"{OAuth1Helper.UrlEncode(verifier)}\", " +
                $"oauth_signature_method=\"{OAuth1Helper.UrlEncode(oauthSignatureMethod)}\", " +
                $"oauth_timestamp=\"{OAuth1Helper.UrlEncode(oauthTimestamp)}\", " +
                $"oauth_nonce=\"{OAuth1Helper.UrlEncode(oauthNonce)}\", " +
                $"oauth_version=\"{OAuth1Helper.UrlEncode(oauthVersion)}\", " +
                $"oauth_signature=\"{OAuth1Helper.UrlEncode(signature)}\"";

            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Headers.Add("Authorization", authHeader);
            request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, content);

            return Ok(content); // Esto contendrá oauth_token & oauth_token_secret del usuario
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string oauth_token, [FromQuery] string oauth_verifier)
        {
            // 1. Recupera el oauth_token_secret que guardaste cuando hiciste el request_token
            if (!_cache.TryGetValue("oauth:req:" + oauth_token, out string tokenSecret))
                return BadRequest("Token secret not found");


            var url = "https://api.clever-cloud.com/v2/oauth/access_token";

            var oauthNonce = OAuth1Helper.GenerateNonce();
            var oauthTimestamp = OAuth1Helper.GenerateTimestamp();
            var oauthVersion = "1.0";
            var oauthSignatureMethod = "PLAINTEXT";

            // PLAINTEXT signature
            var oauthSignature = $"{OAuth1Helper.UrlEncode(_clientSecret)}&{OAuth1Helper.UrlEncode(tokenSecret)}";

            var authHeader = $"OAuth " +
                $"oauth_consumer_key=\"{OAuth1Helper.UrlEncode(_clientId)}\", " +
                $"oauth_token=\"{OAuth1Helper.UrlEncode(oauth_token)}\", " +
                $"oauth_verifier=\"{OAuth1Helper.UrlEncode(oauth_verifier)}\", " +
                $"oauth_signature_method=\"{OAuth1Helper.UrlEncode(oauthSignatureMethod)}\", " +
                $"oauth_signature=\"{OAuth1Helper.UrlEncode(oauthSignature)}\", " +
                $"oauth_timestamp=\"{OAuth1Helper.UrlEncode(oauthTimestamp)}\", " +
                $"oauth_nonce=\"{OAuth1Helper.UrlEncode(oauthNonce)}\", " +
                $"oauth_version=\"{OAuth1Helper.UrlEncode(oauthVersion)}\"";

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", authHeader);
            request.Content = new StringContent(""); // El cuerpo puede estar vacío, ya que todo va en el header
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, content);

            var query = System.Web.HttpUtility.ParseQueryString(content);
            var accessToken = query["oauth_token"];
            var accessTokenSecret = query["oauth_token_secret"];

             url = "https://api.clever-cloud.com/v2/self";

             oauthNonce = OAuth1Helper.GenerateNonce();
             oauthTimestamp = OAuth1Helper.GenerateTimestamp();
             oauthVersion = "1.0";
             oauthSignatureMethod = "PLAINTEXT";

             oauthSignature = $"{OAuth1Helper.UrlEncode(_clientSecret)}&{OAuth1Helper.UrlEncode(accessTokenSecret)}";

             authHeader = $"OAuth " +
                $"oauth_consumer_key=\"{OAuth1Helper.UrlEncode(_clientId)}\", " +
                $"oauth_token=\"{OAuth1Helper.UrlEncode(accessToken)}\", " +
                $"oauth_signature_method=\"{OAuth1Helper.UrlEncode(oauthSignatureMethod)}\", " +
                $"oauth_signature=\"{OAuth1Helper.UrlEncode(oauthSignature)}\", " +
                $"oauth_timestamp=\"{OAuth1Helper.UrlEncode(oauthTimestamp)}\", " +
                $"oauth_nonce=\"{OAuth1Helper.UrlEncode(oauthNonce)}\", " +
                $"oauth_version=\"{OAuth1Helper.UrlEncode(oauthVersion)}\"";

             request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", authHeader);

             response = await _httpClient.SendAsync(request);
             content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, content);

            var userJson = JsonDocument.Parse(content);
            var uniqueIdentifier = userJson.RootElement.GetProperty("id").GetString();
            var name = userJson.RootElement.GetProperty("name").GetString();

            // Paso 5: Generar un JWT para el frontend
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("cc_token", accessToken),
                new Claim("cc_secret", accessTokenSecret),
                new Claim(ClaimTypes.NameIdentifier, "clever_user"), // o algún ID local si lo vinculas
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(jwt);



            //Buscar en la base de datos
           /*  var user = await _context.Users.FirstOrDefaultAsync(u => u.UniqueIdentifier == uniqueIdentifier);

           if (user == null)*/
           // return Unauthorized("Usuario no registrado");

            // Opcional: Generar JWT o establecer sesión
            return Ok(new
            {
                userJson,
                uniqueIdentifier,
                name,
              //  user,
                jwt = jwtToken
            });


              // Buscar en la base de datos
              // var user = await _context.Users.FirstOrDefaultAsync(u => u.UniqueIdentifier == uniqueIdentifier
        }
        
        [HttpGet("api/oauth/userinfo")]
        private async Task<IActionResult> GetUserInfoF([FromQuery] string accessToken, [FromQuery] string accessTokenSecret)
        {
            var url = "https://api.clever-cloud.com/v2/self";

            var oauthNonce = OAuth1Helper.GenerateNonce();
            var oauthTimestamp = OAuth1Helper.GenerateTimestamp();
            var oauthVersion = "1.0";
            var oauthSignatureMethod = "PLAINTEXT";

            var oauthSignature = $"{OAuth1Helper.UrlEncode(_clientSecret)}&{OAuth1Helper.UrlEncode(accessTokenSecret)}";

            var authHeader = $"OAuth " +
                $"oauth_consumer_key=\"{OAuth1Helper.UrlEncode(_clientId)}\", " +
                $"oauth_token=\"{OAuth1Helper.UrlEncode(accessToken)}\", " +
                $"oauth_signature_method=\"{OAuth1Helper.UrlEncode(oauthSignatureMethod)}\", " +
                $"oauth_signature=\"{OAuth1Helper.UrlEncode(oauthSignature)}\", " +
                $"oauth_timestamp=\"{OAuth1Helper.UrlEncode(oauthTimestamp)}\", " +
                $"oauth_nonce=\"{OAuth1Helper.UrlEncode(oauthNonce)}\", " +
                $"oauth_version=\"{OAuth1Helper.UrlEncode(oauthVersion)}\"";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", authHeader);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, content);

            var userJson = JsonDocument.Parse(content);
            var uniqueIdentifier = userJson.RootElement.GetProperty("id").GetString();
            var name = userJson.RootElement.GetProperty("name").GetString();

            // Buscar en la base de datos
           // var user = await _context.Users.FirstOrDefaultAsync(u => u.UniqueIdentifier == uniqueIdentifier);

           // if (user == null)
             //   return Unauthorized("Usuario no registrado");

            // Opcional: Generar JWT o establecer sesión
            return Ok(new
            {
                userJson,
                uniqueIdentifier,
                Token = accessToken,
                name,
                TokenSecret = accessTokenSecret
            });
        }



    }
}
