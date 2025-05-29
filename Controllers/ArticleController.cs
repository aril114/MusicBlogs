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
    private ITagData _tags;
    private ILikeData _likes;
    private IUserData _users;
    private ModlogService _logger;


    public ArticleController(IArticleData articleData, ICommentData commentData,
        ITagData tagData, ILikeData likeData, IUserData userData, ModlogService logger)
    {
        _articles = articleData;
        _comments = commentData;
        _tags = tagData;
        _likes = likeData;
        _users = userData;
        _logger = logger;
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

        int recommendedCount = 3;

        List<Article> recommendations = new List<Article>();
        HashSet<int> rIds = new HashSet<int>();
        rIds.Add(id);

        foreach (string tag in tags)
        {
            var tagArticles = _articles.Search(query: null, tags: [tag]);

            foreach (Article tagArticle in tagArticles)
            {
                if (!rIds.Contains(tagArticle.id))
                {
                    rIds.Add(tagArticle.id);
                    recommendations.Add(tagArticle);

                    if (recommendations.Count >= recommendedCount) break;
                }
            }

            if (recommendations.Count() >= recommendedCount) break;
        }

        ViewBag.recommendations = recommendations;

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

    /// <summary>
    /// Удаление статьи
    /// </summary>
    /// <param name="id">id статьи</param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [Route("/a/delete")]
    public ActionResult Delete(int id)
    {
        User userDeleting = _users.Get(User.Identity.Name);

        if (!userDeleting.is_moderator)
        {
            return Unauthorized();
        }

        Article article = _articles.Get(id);

        if (article == null)
        {
            return BadRequest();
        }

        _articles.Delete(article);

        _logger.LogAction("Удаление статьи", userDeleting.login, $"Автор: {article.login_Users}, тема: {article.title}");

        return Ok("Статья удалена");
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
        var userCommenting = _users.Get(User.Identity.Name);
        if (userCommenting.is_banned)
        {
            return Unauthorized($"Вы забанены. Причина бана: {userCommenting.ban_reason}");
        }

        string author = User.Identity.Name;
        _comments.Add(text, articleId, author, answerTo);
        return RedirectToAction("Index", new { id = articleId });
    }

    [Authorize]
    [HttpPost]
    public IActionResult DeleteComment(int articleId, int commentId)
    {
        Comment comment = _comments.Get(commentId, articleId);

        if (comment == null)
        {
            return BadRequest();
        }

        User userDeleting = _users.Get(User.Identity.Name);

        if (!(comment.login_Users == userDeleting.login || userDeleting.is_moderator))
        {
            return Unauthorized();
        }

        _comments.Delete(comment);

        if (comment.login_Users != userDeleting.login)
        {
            _logger.LogAction("Удаление комментария", userDeleting.login,
                $"Автор: {comment.login_Users}, текст: {comment.content}, ID статьи: {comment.id_Articles}");
        }

        return Ok("Комментарий удален");
    }
}
