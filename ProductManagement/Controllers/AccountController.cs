using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


public class AccountController : Controller
{
    private readonly HttpClient _httpClient;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5014"); // API Base URL
    }

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var requestBody = new { username, password };
        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/v1/Auth/login", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JwtResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Token != null)
            {
                Response.Cookies.Append("AuthToken", result.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Ensure security in HTTPS
                    SameSite = SameSiteMode.Strict // Prevent CSRF attacks
                });

                return RedirectToAction("Index", "Product");
            }
        }

        ViewBag.Error = "Invalid email or password";
        return View();
    }
   
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken");
        return RedirectToAction("Login");
    }

    public IActionResult Register()
    {
        return View();
    }

   
    public async Task<IActionResult> RegisterInput(string name, string email,string username, string password)
    {
        var user = new
        {
            name = name,
            username = username,
            email = email,
            isActive = true,
            password = password
        };

        var json = JsonSerializer.Serialize(user);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/v1/RefUser", content);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }

        ViewData["Error"] = "Registration failed. Please try again.";
        return View();
    }
}

public class JwtResponse
{
    public string Token { get; set; }
}
