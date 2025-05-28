using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicBlogs.Models;
using MusicBlogs.Services;

namespace MusicBlogs.Controllers;

[Authorize]
public class BookmarksController : Controller
{
    private IArticleData _articleData;
    private IBookmarkData _bookmarkData;

    public BookmarksController(IArticleData articleData, IBookmarkData bookmarkData)
    {
        _articleData = articleData;
        _bookmarkData = bookmarkData;
    }

    public ActionResult Index()
    {
        var bookmarks = _bookmarkData.GetAllForUser(User.Identity.Name);

        var bookmarksAddedAt = from b in bookmarks
                               select b.created_at;

        var articles = from b in bookmarks
                       select _articleData.Get(b.id_Articles);

        return View(articles.Zip(bookmarksAddedAt));
    }

    [HttpPost]
    public ActionResult Add(int articleId)
    {

        Article article = _articleData.Get(articleId);

        if (article == null)
        {
            return BadRequest();
        }

        _bookmarkData.Add(User.Identity.Name, articleId);

        return Ok("Закладка добавлена");
    }

    [HttpPost]
    public ActionResult Delete(int articleId)
    {
        var bookmark = _bookmarkData.Get(User.Identity.Name, articleId);

        if (bookmark == null)
        {
            return BadRequest();
        }

        _bookmarkData.Delete(bookmark);

        return Ok("Закладка удалена");
    }
}
