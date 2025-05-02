using Microsoft.AspNetCore.Mvc;
using MusicBlogs.Models;
using MusicBlogs.Services;

namespace MusicBlogs.Controllers;

public class UserController : Controller
{
    private IUserData _users;
    private IArticleData _articles;
    private ICommentData _comments;
    private const int _articlesPerPage = 5;
    private const int _userCommentsPerPage = 20;

    public UserController(IUserData userData, IArticleData articleData,
        ICommentData commentData)
    {
        _users = userData;
        _articles = articleData;
        _comments = commentData;
    }

    [Route("/u/{username}")]
    public IActionResult Index(string username)
    {
        var model = _users.Get(username);

        if (model == null)
        {
            return NotFound();
        }

        ViewBag.username = username;

        return View(model);
    }

    [Route("/u/{username}/comments")]
    public IActionResult Comments(string username, int? page)
    {
        if (_users.Get(username) == null)
        {
            return NotFound();
        }

        IEnumerable<Comment> model = _comments.GetAllForUser(username);

        PagingInfo p = new PagingInfo()
        {
            CurrentPage = page ?? 1,
            ItemsPerPage = _userCommentsPerPage,
            TotalItems = model.Count()
        };

        ViewBag.PageInfo = p;

        model = model
            .Skip(p.ItemsPerPage * (p.CurrentPage - 1))
            .Take(p.ItemsPerPage)
            .ToList();

        ViewBag.username = username;

        return View(model);
    }

    [Route("/u/{username}/articles")]
    public IActionResult Articles(string username, int? page)
    {
        if (_users.Get(username) == null)
        {
            return NotFound();
        }

        IEnumerable<Article> model = _articles.GetAllForUser(username);

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


        ViewBag.username = username;

        return View(model);
    }

    [Route("/u/{username}/articles/bestrated")]
    public IActionResult BestRatedArticles(string username, int? page)
    {
        if (_users.Get(username) == null)
        {
            return NotFound();
        }

        IEnumerable<Article> model = _articles.GetAllForUser(username, sortByDate: false);

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


        ViewBag.username = username;

        return View(model);
    }
}
