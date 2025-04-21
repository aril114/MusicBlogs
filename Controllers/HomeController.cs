using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MusicBlogs.Models;
using MusicBlogs.Services;

namespace MusicBlogs.Controllers;

public class HomeController : Controller
{
    const string SessionLogin = "login";
    const string SessionPassword = "password";

    private readonly ILogger<HomeController> _logger;
    private IArticleData _articles;

    public HomeController(ILogger<HomeController> logger, IArticleData articleData)
    {
        _logger = logger;
        _articles = articleData;
    }

    public IActionResult Index()
    {
        var model = _articles.GetAll();
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
