using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicBlogs.Models;
using MusicBlogs.Services;
using MusicBlogs.ViewModels;

namespace MusicBlogs.Controllers;

public class ArticleController : Controller
{
    private IArticleData _articles;
    private ICommentData _comments;

    public ArticleController(IArticleData articleData, ICommentData commentData)
    {
        _articles = articleData;
        _comments = commentData;
    }

    [Route("/a/{id}")]
    public IActionResult Index(string id)
    {
        var article = _articles.Get(int.Parse(id));

        if (article == null)
        {
            return NotFound();
        }

        var comments = _comments.GetAllForArticle(int.Parse(id));

        var model = new ArticleViewModel(
            article.id,
            article.published_at,
            article.rating,
            article.content,
            article.title,
            article.login_Users,
            comments);

        return View(model);
    }

    [Route("/a/{id}/rating")]
    public IActionResult GetRating(int id)
    {
        var article = _articles.Get(id);

        if (article == null)
        {
            return BadRequest();
        }

        return new ObjectResult(article.rating);
    }

    [Authorize]
    [Route("/a/{id}/like")]
    public IActionResult Like(int id)
    {
        var article = _articles.Get(id);

        if (article == null)
        {
            return BadRequest();
        }

        int newRating = article.rating + 1;

        var newArticle = article with { rating = newRating };

        _articles.Update(newArticle);

        return new ObjectResult(newRating);
    }

    [Authorize]
    [Route("/a/{id}/dislike")]
    public IActionResult Dislike(int id)
    {
        var article = _articles.Get(id);

        if (article == null)
        {
            return BadRequest();
        }

        int newRating = article.rating - 1;

        var newArticle = article with { rating = newRating };

        _articles.Update(newArticle);

        return new ObjectResult(newRating);
    }

    [Route("/a/{id}/comments")]
    public IActionResult GetComments(int id)
    {
        var comments = _comments.GetAllForArticle(id);

        return new ObjectResult(comments);
    }

    [Authorize]
    [HttpPost]
    public IActionResult AddComment(int articleId, string text, int? answerTo)
    {
        string author = User.Identity.Name;
        _comments.Add(text, articleId, author, answerTo);
        return RedirectToAction("Index", new { id = articleId });
    }
}
