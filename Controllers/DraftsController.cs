﻿using Markdig;
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

    // GET: MyArticlesController
    public ActionResult Index()
    {
        var model = _draftArticleData.GetAllForUser(User.Identity.Name);
        return View(model);
    }

    // GET: MyArticlesController/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: MyArticlesController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(string title, string text, string tags, string excerpt, string preview_img)
    {
        _draftArticleData.Add(text, title, tags, excerpt, preview_img, User.Identity.Name);
        return RedirectToAction("Index");
    }

    // GET: MyArticlesController/Edit/5
    public ActionResult Edit(int id)
    {
        var model = _draftArticleData.Get(id, User.Identity.Name);
        return View(model);
    }

    // POST: MyArticlesController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, string title, string text, string tags, string excerpt, string previewImg)
    {
        var oldDraft = _draftArticleData.Get(id, User.Identity.Name);

        if (oldDraft == null)
        {
            return BadRequest();
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

    // Публикует статью. Использует для создания новой
    // статьи данные формы, а не данные черновика из БД
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Publish(int? id, string title, string text, string tags, string excerpt, string previewImg)
    {
        var userPublishing = _userData.Get(User.Identity.Name);

        if (userPublishing.is_banned)
        {
            return Unauthorized($"Вы забанены. Причина бана: {userPublishing.ban_reason}");
        }

        var mdPipeline = new MarkdownPipelineBuilder()
            .UseBootstrap()
            .UseSoftlineBreakAsHardlineBreak()
            .UseAdvancedExtensions()
            .Build();

        string htmledContent = Markdown.ToHtml(text, mdPipeline);

        DraftArticle? draft = null;

        // Публикуется черновик, для которого в БД уже есть запись
        if (id != null)
        {
            draft = _draftArticleData.Get(id.Value, User.Identity.Name);

            if (draft == null)
            {
                return BadRequest();
            }
        }

        // Либо создается новая статья, которая и публикуется
        int articleId = _articleData.Add(htmledContent, title, excerpt, previewImg, User.Identity.Name);

        // Удаление черновика, если публикуется он
        if (draft != null)
        {
            _draftArticleData.Delete(draft);
        }

        // Добавление тегов
        if (!string.IsNullOrWhiteSpace(tags))
        {
            string[] splittedTags = Regex.Split(tags, @"\s*,\s*");

            foreach (string tag in splittedTags)
            {
                _tagData.Add(tag, articleId);
            }
        }

        return RedirectToAction("Index", "Article", new { id = articleId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult PublishById(int id)
    {
        var userPublishing = _userData.Get(User.Identity.Name);

        if (userPublishing.is_banned)
        {
            return Unauthorized($"Вы забанены. Причина бана: {userPublishing.ban_reason}");
        }

        var mdPipeline = new MarkdownPipelineBuilder()
            .UseBootstrap()
            .UseSoftlineBreakAsHardlineBreak()
            .UseAdvancedExtensions()
            .Build();

        var draft = _draftArticleData.Get(id, User.Identity.Name);

        if (draft == null)
        {
            return BadRequest();
        }

        string excerpt = draft.excerpt;

        string previewImg = draft.preview_img;

        string htmledContent = Markdown.ToHtml(draft.content, mdPipeline);

        // Добавление статьи в БД
        int articleId = _articleData.Add(htmledContent, draft.title, excerpt, previewImg, User.Identity.Name);

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
