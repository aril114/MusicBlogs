using Microsoft.AspNetCore.Mvc;
using MusicBlogs.Services;
using System.Text.RegularExpressions;

namespace MusicBlogs.Controllers;

public class SearchController : Controller
{
    private IArticleData _articles;

    public SearchController(IArticleData articleData)
    {
        _articles = articleData;
    }

    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Q(string? query, string? tags, string searchIn, string sortBy, string ascDesc)
    {
        bool searchInTitle = searchIn == "title" ? true : false;

        bool sortByDate = sortBy == "date" ? true : false;

        bool desc = ascDesc == "desc" ? true : false;

        string[]? splittedTags = null;

        if (tags != null)
        {
            splittedTags = Regex.Split(tags, @"\s*,\s*");
        }

        var model = _articles.Search(query, splittedTags, searchInTitle, sortByDate, desc);

        return View(model);
    }

    public ActionResult ByTag(string name)
    {
        var model = _articles.GetAllWithTags([name]);
        return View("Q", model);
    }
}
