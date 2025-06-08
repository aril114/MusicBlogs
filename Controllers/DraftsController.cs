using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicBlogs.Models;
using MusicBlogs.Services;
using System.Text.RegularExpressions;

namespace MusicBlogs.Controllers;

[Authorize]
public class DraftsController : Controller
{
    private IArticleData _articleData;
    private IDraftArticleData _draftArticleData;
    private ITagData _tagData;
    private IUserData _userData;

    public DraftsController(IArticleData articleData,
        IDraftArticleData draftArticleData, ITagData tagData, IUserData userData)
    {
        _articleData = articleData;
        _draftArticleData = draftArticleData;
        _tagData = tagData;
        _userData = userData;
    }

    public ActionResult Index()
    {
        var model = _draftArticleData.GetAllForUser(User.Identity.Name, false);
        return View(model);
    }

    public ActionResult MyPendingModeration()
    {
        var model = _draftArticleData.GetAllForUser(User.Identity.Name, true);
        return View(model);
    }

    public ActionResult ModerateArticles()
    {
        var username = User.Identity.Name;

        if (!_userData.Get(username).is_moderator)
        {
            return Unauthorized();
        }

        var model = _draftArticleData.GetAllBeingModerated();

        return View(model);
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(string title, string text, string tags, string excerpt, string preview_img)
    {
        _draftArticleData.Add(text, title, tags, excerpt, preview_img, User.Identity.Name);
        return RedirectToAction("Index");
    }

    public ActionResult Edit(int id, string? author = null)
    {
        author ??= User.Identity.Name;

        var model = _draftArticleData.Get(id, author);

        if (author != User.Identity.Name)
        {
            var user = _userData.Get(User.Identity.Name);

            if (!user.is_moderator)
            {
                return Unauthorized();
            }

            if (model == null || !model.is_being_moderated)
            {
                return BadRequest();
            }
        }

        else
        {
            if (model == null)
            {
                return NotFound();
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, string? author, string title, string text, string tags, string excerpt, string previewImg)
    {
        author ??= User.Identity.Name;

        var oldDraft = _draftArticleData.Get(id, author);

        if (author != User.Identity.Name)
        {
            var user = _userData.Get(User.Identity.Name);

            if (!user.is_moderator)
            {
                return Unauthorized();
            }

            if (oldDraft == null || !oldDraft.is_being_moderated)
            {
                return BadRequest();
            }
        }

        else
        {
            if (oldDraft == null)
            {
                return NotFound();
            }
        }

        var newDraft = oldDraft with { title = title, content = text, tags = tags, excerpt = excerpt, preview_img = previewImg };

        _draftArticleData.Update(newDraft);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id)
    {
        var draft = _draftArticleData.Get(id, User.Identity.Name);

        if (draft == null)
        {
            return BadRequest();
        }

        _draftArticleData.Delete(draft);

        return Ok("Черновик удален");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult SendToModeration(int id)
    {
        var authorLogin = User.Identity.Name;

        DraftArticle draft = _draftArticleData.Get(id, authorLogin);

        if (draft == null)
        {
            return BadRequest();
        }

        if (draft.is_being_moderated)
        {
            return BadRequest("Статья уже ожидает публикации");
        }

        _draftArticleData.Update(draft with { is_being_moderated = true });

        return Ok("Статья отправлена на публикацию");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult SendBackToAuthor(int id, string author)
    {
        var userSending = _userData.Get(User.Identity.Name);

        if (!userSending.is_moderator)
        {
            return Unauthorized();
        }

        var draft = _draftArticleData.Get(id, author);

        if (draft == null || !draft.is_being_moderated)
        {
            return BadRequest();
        }

        _draftArticleData.Update(draft with { is_being_moderated = false });

        return Ok("Черновик отправлен обратно автору");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult PublishNotExisting(string title, string text, string tags, string excerpt, string previewImg)
    {
        int id = _draftArticleData.Add(text, title, tags, excerpt, previewImg, User.Identity.Name);

        var userPublishing = _userData.Get(User.Identity.Name);

        if (userPublishing.is_moderator)
        {
            return Publish(id);
        }

        return SendToModeration(id);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Publish(int id, string? author = null)
    {
        var userPublishing = _userData.Get(User.Identity.Name);

        if (userPublishing.is_banned)
        {
            return Unauthorized($"Вы забанены. Причина бана: {userPublishing.ban_reason}");
        }

        if (!userPublishing.is_moderator)
        {
            return Unauthorized("Опубликовать статью может только модератор");
        }

        author ??= User.Identity.Name;

        var draft = _draftArticleData.Get(id, author);

        if (draft == null)
        {
            return BadRequest("Черновик статьи не найден");
        }

        if (userPublishing.login != author && !draft.is_being_moderated)
        {
            return BadRequest("Нельзя опубликовать статью, которая не была отправлена на модерацию.");
        }

        var mdPipeline = new MarkdownPipelineBuilder()
            .UseBootstrap()
            .UseSoftlineBreakAsHardlineBreak()
            .UseAdvancedExtensions()
            .Build();

        string excerpt = draft.excerpt;

        string previewImg = draft.preview_img;

        string htmledContent = Markdown.ToHtml(draft.content, mdPipeline);

        // Добавление статьи в БД
        int articleId = _articleData.Add(htmledContent, draft.title, excerpt, previewImg, draft.login_Users);

        // Теги
        if (!string.IsNullOrWhiteSpace(draft.tags))
        {
            string[] splittedTags = Regex.Split(draft.tags, @"\s*,\s*");

            foreach (string tag in splittedTags)
            {
                _tagData.Add(tag, articleId);
            }
        }

        _draftArticleData.Delete(draft);

        return RedirectToAction("Index", "Article", new { id = articleId });
    }
}
