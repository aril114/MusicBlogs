using Microsoft.AspNetCore.Mvc;
using MusicBlogs.Services;

namespace MusicBlogs.Controllers;

public class SearchController : Controller
{
    private IArticleData _articles;
    private IUserData _users;

    public SearchController(IArticleData articleData, IUserData userData)
    {
        _articles = articleData;
        _users = userData;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Q(string query, string searchIn, string sortBy, string ascDesc)
    {
        bool searchInTitle = searchIn == "title" ? true : false;

        bool sortByDate = sortBy == "date" ? true : false;

        bool desc = ascDesc == "desc" ? true : false;

        var model = _articles.Search(query, searchInTitle, sortByDate, desc);

        return View(model);
    }
}
