using Microsoft.AspNetCore.Mvc;
using MusicBlogs.Models;
using MusicBlogs.Services;
using System.Text.RegularExpressions;

namespace MusicBlogs.Controllers;

public class SearchController : Controller
{
    private IArticleData _articles;
    private const int _articlesPerPage = 6;

    public SearchController(IArticleData articleData)
    {
        _articles = articleData;
    }

    public ActionResult Index()
    {
        return View();
    }

    public ActionResult Q(string? query, string? tags, string searchIn, string sortBy, string ascDesc, int? page)
    {
        bool searchInTitle = searchIn == "title" ? true : false;

        bool sortByDate = sortBy == "rating" ? false : true;

        bool desc = ascDesc == "asc" ? false : true;

        string[]? splittedTags = null;

        if (tags != null)
        {
            splittedTags = Regex.Split(tags, @"\s*,\s*");
        }

        IEnumerable<Article> model = _articles.Search(query, splittedTags, searchInTitle, sortByDate, desc);

        PagingInfo p = new PagingInfo()
        {
            CurrentPage = page ?? 1,
            ItemsPerPage = _articlesPerPage,
            TotalItems = model.Count()
        };

        ViewBag.PageInfo = p;

        // Надо сохранить параметры поиска, чтобы в представлении добавить их ссылкам
        ViewBag.query = query;
        ViewBag.tags = tags;
        ViewBag.searchIn = searchIn;
        ViewBag.sortBy = sortBy;
        ViewBag.ascDesc = ascDesc;

        ViewBag.searchInTitle = searchInTitle;

        model = model
            .Skip(p.ItemsPerPage * (p.CurrentPage - 1))
            .Take(p.ItemsPerPage);

        return View(model);
    }
}
