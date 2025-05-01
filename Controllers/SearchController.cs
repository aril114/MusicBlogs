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

    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Q(string query, string searchIn, string sortBy, string ascDesc)
    {
        bool searchInTitle = searchIn == "title" ? true : false;

        bool sortByDate = sortBy == "date" ? true : false;

        bool desc = ascDesc == "desc" ? true : false;

        var model = _articles.Search(query, searchInTitle, sortByDate, desc);

        return View(model);
    }

    public ActionResult ByTag(string name)
    {
        var model = _articles.GetAllWithTags(new string[] { name });
        return View("Q", model);
    }
}
