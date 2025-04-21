using Microsoft.AspNetCore.Mvc;
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
}
