using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicBlogs.Services;
using MusicBlogs.ViewModels;

namespace MusicBlogs.Controllers;

public class ArticleController : Controller
{
    private IArticleData _articles;
    private ICommentData _comments;
    private ITagData _tags;
    private ILikeData _likes;


    public ArticleController(IArticleData articleData, ICommentData commentData,
        ITagData tagData, ILikeData likeData)
    {
        _articles = articleData;
        _comments = commentData;
        _tags = tagData;
        _likes = likeData;
    }

    [Route("/a/{id}")]
    public IActionResult Index(int id)
    {
        var article = _articles.Get(id);

        if (article == null)
        {
            return NotFound();
        }

        var comments = _comments.GetAllForArticle(id);

        var dbTags = _tags.GetAllForArticle(id);

        string[] tags = dbTags.Select(t => t.name).ToArray();

        var model = new ArticleViewModel(
            article.id,
            article.published_at,
            article.rating,
            article.content,
            article.title,
            article.login_Users,
            comments,
            tags);

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
        var like = _likes.Get(User.Identity.Name, id);

        if (like != null)
        {
            return BadRequest();
        }

        _likes.Add(User.Identity.Name, id);

        int newRating = _articles.Get(id).rating;

        return new ObjectResult(newRating);
    }

    [Authorize]
    [Route("/a/{id}/dislike")]
    public IActionResult Dislike(int id)
    {
        var like = _likes.Get(User.Identity.Name, id);

        if (like == null)
        {
            return BadRequest();
        }

        _likes.Delete(like);

        int newRating = _articles.Get(id).rating;

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
