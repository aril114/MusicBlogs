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

    [Route("/u/{login}")]
    public IActionResult Index(string login)
    {
        var model = _users.Get(login);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [Route("/u/{login}/comments")]
    public IActionResult Comments(string login)
    {
        if (_users.Get(login) == null)
        {
            return NotFound();
        }

        IEnumerable<Comment> model = _comments.GetAllForUser(login);
        return View(model);
    }

    [Route("/u/{login}/articles")]
    public IActionResult Articles(string login)
    {
        if (_users.Get(login) == null)
        {
            return NotFound();
        }

        IEnumerable<Article> model = _articles.GetAllForUser(login);
        return View(model);
    }
}
