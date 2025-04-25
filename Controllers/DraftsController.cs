using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicBlogs.Services;

namespace MusicBlogs.Controllers;

[Authorize]
public class DraftsController : Controller
{
    private IArticleData _articleData;
    private IDraftArticleData _draftArticleData;
    public DraftsController(IArticleData articleData, IDraftArticleData draftArticleData)
    {
        _articleData = articleData;
        _draftArticleData = draftArticleData;
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
    public ActionResult Create(string title, string text)
    {
        _draftArticleData.Add(text, title, User.Identity.Name);
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
    public ActionResult Edit(int id, string title, string text)
    {
        var oldDraft = _draftArticleData.Get(id, User.Identity.Name);

        if (oldDraft == null)
        {
            return BadRequest();
        }

        var newDraft = oldDraft with { title = title, content = text };

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
    public ActionResult Publish(int? id, string title, string text)
    {
        var mdPipeline = new MarkdownPipelineBuilder().UseBootstrap().Build();

        if (id != null)
        {
            var draft = _draftArticleData.Get(id.Value, User.Identity.Name);

            if (draft == null)
            {
                return BadRequest();
            }

            string htmledContent = Markdown.ToHtml(text, mdPipeline);

            _articleData.Add(htmledContent, title, User.Identity.Name);

            _draftArticleData.Delete(draft);
        }

        else
        {
            string htmledContent = Markdown.ToHtml(text, mdPipeline);

            _articleData.Add(htmledContent, title, User.Identity.Name);
        }

        return Ok("Ваша статья опубликована");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult PublishById(int id)
    {
        var mdPipeline = new MarkdownPipelineBuilder().UseBootstrap().Build();

        var draft = _draftArticleData.Get(id, User.Identity.Name);

        if (draft == null)
        {
            return BadRequest();
        }

        string htmledContent = Markdown.ToHtml(draft.content, mdPipeline);

        _articleData.Add(htmledContent, draft.title, User.Identity.Name);

        _draftArticleData.Delete(draft);

        return Ok("Ваша статья опубликована");
    }
}
