using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System;

using inventory8.Helpers;

namespace inventory8.Controllers
{
    [ApiController]
    [Route("api/oauth")]
    public class OAuthController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public OAuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        [HttpPost("request_token")]
        public async Task<IActionResult> RequestToken()
        {
            var consumerKey = "Iq8B1YKaUU88OJXoAo92ooLC4pX06c";
            var consumerSecret = "OF0YPVYjvfJJk6uQjUYLRbRx1U1pfG";
            var callbackUrl = "https://app-aa670142-e807-4859-be5b-19fad2323953.cleverapps.io/Callback";

            var oauthNonce = OAuth1Helper.GenerateNonce();
            var oauthTimestamp = OAuth1Helper.GenerateTimestamp();
            var oauthVersion = "1.0";
            var oauthSignatureMethod = "PLAINTEXT";

            // PLAINTEXT signature
            var oauthSignature = $"{OAuth1Helper.UrlEncode(consumerSecret)}&"; // No token_secret aún

            var authHeader = $"OAuth " +
                $"oauth_callback=\"{OAuth1Helper.UrlEncode(callbackUrl)}\", " +
                $"oauth_consumer_key=\"{OAuth1Helper.UrlEncode(consumerKey)}\", " +
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

            return Ok(content);
        }

    }
}
