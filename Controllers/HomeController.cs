using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MusicBlogs.Models;
using MusicBlogs.Services;

namespace MusicBlogs.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private IArticleData _articles;
    private const int _articlesPerPage = 5;

    public HomeController(ILogger<HomeController> logger, IArticleData articleData)
    {
        _logger = logger;
        _articles = articleData;
    }

    public IActionResult Index(int? page)
    {
        var model = _articles.GetAll();

        PagingInfo p = new PagingInfo()
        {
            CurrentPage = page ?? 1,
            ItemsPerPage = _articlesPerPage,
            TotalItems = model.Count()
        };

        ViewBag.PageInfo = p;

        model = model
            .Skip(p.ItemsPerPage * (p.CurrentPage - 1))
            .Take(p.ItemsPerPage)
            .ToList();

        return View(model);
    }

    [Route("/bestrated")]
    public IActionResult BestRated(int? page)
    {
        int currentPage = page ?? 1;

        var model = _articles.GetAll(sortByDate: false);

        PagingInfo p = new PagingInfo()
        {
            CurrentPage = page ?? 1,
            ItemsPerPage = _articlesPerPage,
            TotalItems = model.Count()
        };

        ViewBag.PageInfo = p;

        model = model
            .Skip(p.ItemsPerPage * (p.CurrentPage - 1))
            .Take(p.ItemsPerPage)
            .ToList();

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
