using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;

namespace CrudApp.Controller
{
    [ApiController]
    [Route("api/")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AuthController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var azureAdB2CConfig = _configuration.GetSection("AzureAdB2C");

            var tokenEndpoint = $"{azureAdB2CConfig["Instance"]}{azureAdB2CConfig["Domain"]}/{azureAdB2CConfig["SignUpSignInPolicyId"]}/oauth2/v2.0/token";

            var postData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", azureAdB2CConfig["ClientId"]),
                new KeyValuePair<string, string>("client_secret", azureAdB2CConfig["ClientSecret"]),
                new KeyValuePair<string, string>("scope", azureAdB2CConfig["Scope"]),
                new KeyValuePair<string, string>("username", request.Username),
                new KeyValuePair<string, string>("password", request.Password),
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, postData);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(new { Message = "Authentication failed", Status = response.StatusCode, Details = error });
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

            // Extract the "access_token" field
            if (tokenResponse.TryGetProperty("access_token", out var accessToken))
            {
                return Ok(accessToken.GetString());
            }

            return BadRequest(new { Message = "Token not found in response" });
        }
    }


}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}