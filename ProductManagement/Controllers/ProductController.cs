using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

public class ProductController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IHttpClientFactory httpClientFactory, ILogger<ProductController> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5014"); 
        _logger = logger;
    }

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string sortColumn = "", bool sortDescending = false)
    {
        var token = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/Product/grid?pageNumber={pageNumber}&pageSize={pageSize}&sortColumn={sortColumn}&sortDescending={sortDescending}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API Error: {response.StatusCode}");
                ViewBag.Error = "Failed to load products.";
                return View(new ProductViewModel());
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ProductApiResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (apiResponse?.Data == null || apiResponse.Data.Items == null)
            {
                ViewBag.Error = "No products found.";
                return View(new ProductViewModel());
            }

            return View(new ProductViewModel
            {
                Products = apiResponse.Data.Items,
                PageNumber = apiResponse.Data.PageNumber,
                PageSize = apiResponse.Data.PageSize,
                TotalCount = apiResponse.Data.TotalCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            ViewBag.Error = "Error loading products.";
            return View(new ProductViewModel());
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken] // Prevent CSRF attacks
    public async Task<IActionResult> Delete(Guid guid)
    {
        var token = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.DeleteAsync($"/api/v1/Product/{guid}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Failed to delete product.";
        }
        else
        {
            TempData["Success"] = "Product deleted successfully!";
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Save(string name, string description, decimal price)
    {
        var token = Request.Cookies["AuthToken"]; 

        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var product = new
        {
            guid = (Guid?)null,
            name = name,
            description = description,
            price = price
        };

        var json = JsonSerializer.Serialize(product);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Attach the Authorization Header
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsync("/api/v1/Product", content);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "New product added!";
            return RedirectToAction("Index");
        }

        ViewData["Error"] = "Failed to add new product.";
        return View();
    }

    
    public async Task<IActionResult> Edit(Guid guid, string name, string description, decimal price)
    {
        var token = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var product = new
        {
            guid = guid,
            name = name,
            description = description,
            price = price
        };

        var json = JsonSerializer.Serialize(product);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.PostAsync("/api/v1/Product", content);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Product updated successfully!";
            return RedirectToAction("Index");
        }

        ViewData["Error"] = "Failed to update product.";
        return View("Edit", product);
    }

    public IActionResult Create()
    {
        return View();
    }
}

// ViewModel
public class ProductViewModel
{
    public List<Product> Products { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}

public class ProductApiResponse
{
    public HeaderObj HeaderObj { get; set; }
    public ProductData Data { get; set; }
}

public class HeaderObj
{
    public string ResponseTime { get; set; }
    public string StatusCode { get; set; }
    public string Message { get; set; }
}

public class ProductData
{
    public List<Product> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Guid { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}
