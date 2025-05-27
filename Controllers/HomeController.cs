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

    //public IActionResult Index(int page = 1, string sortBy = "date", string ascDesc = "desc")
    //{
    //    bool sortByDate = sortBy == "date";
    //    bool desc = ascDesc == "desc";

    //    var model = _articles.GetAll(sortByDate, desc);
    //    PagingInfo p = new PagingInfo()
    //    {
    //        CurrentPage = page,
    //        ItemsPerPage = _articlesPerPage,
    //        TotalItems = model.Count()
    //    };

    //    ViewBag.PageInfo = p;

    //    model = model
    //        .Skip(p.ItemsPerPage * (p.CurrentPage - 1))
    //        .Take(p.ItemsPerPage)
    //        .ToList();

    //    ViewBag.sortBy = sortBy;
    //    ViewBag.ascDesc = ascDesc;

    //    return View(model);
    //}

    public IActionResult Index()
    {
        ViewBag.lastArticles = _articles.GetAll(sortByDate: true).Take(5);

        ViewBag.popularArticles = _articles.GetAll(sortByDate: false).Take(5);

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
