using Microsoft.AspNetCore.Mvc;
using MusicBlogs.Models;
using MusicBlogs.Services;

namespace MusicBlogs.Controllers;

public class UserController : Controller
{
    private IUserData _users;
    private IArticleData _articles;
    private ICommentData _comments;

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
    public IActionResult Comments(string username)
    {
        if (_users.Get(username) == null)
        {
            return NotFound();
        }

        IEnumerable<Comment> model = _comments.GetAllForUser(username);

        ViewBag.username = username;

        return View(model);
    }

    [Route("/u/{username}/articles")]
    public IActionResult Articles(string username)
    {
        if (_users.Get(username) == null)
        {
            return NotFound();
        }

        IEnumerable<Article> model = _articles.GetAllForUser(username);

        ViewBag.username = username;

        return View(model);
    }
}
