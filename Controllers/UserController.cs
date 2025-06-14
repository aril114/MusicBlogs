﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicBlogs.Models;
using MusicBlogs.Services;

namespace MusicBlogs.Controllers;

public class UserController : Controller
{
    private IUserData _users;
    private IArticleData _articles;
    private ICommentData _comments;
    private ModlogService _logger;

    private const int _articlesPerPage = 6;
    private const int _userCommentsPerPage = 20;

    public UserController(IUserData userData, IArticleData articleData,
        ICommentData commentData, ModlogService logger)
    {
        _users = userData;
        _articles = articleData;
        _comments = commentData;
        _logger = logger;
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
    public IActionResult Comments(string username, int page = 1)
    {
        if (_users.Get(username) == null)
        {
            return NotFound();
        }

        IEnumerable<Comment> model = _comments.GetAllForUser(username);

        PagingInfo p = new PagingInfo()
        {
            CurrentPage = page,
            ItemsPerPage = _userCommentsPerPage,
            TotalItems = model.Count()
        };

        ViewBag.PageInfo = p;

        model = model
            .Skip(p.ItemsPerPage * (p.CurrentPage - 1))
            .Take(p.ItemsPerPage)
            .ToList();

        ViewBag.username = username;

        return View(model);
    }

    /// <summary>
    /// Статьи пользователя
    /// </summary>
    /// <param name="username">Имя автора статей</param>
    /// <param name="page">Номер просматриваемой страницы</param>
    /// <param name="sortBy">Поле, по которому производится сортировка. Date или rating</param>
    /// <param name="ascDesc">Произвести сортировку по убыванию (desc) или возрастанию (asc)</param>
    /// <returns></returns>
    [Route("/u/{username}/articles")]
    public IActionResult Articles(string username, int page = 1, string sortBy = "date", string ascDesc = "desc")
    {
        if (_users.Get(username) == null)
        {
            return NotFound();
        }

        bool desc = ascDesc == "desc";
        bool sortByDate = sortBy == "date";

        IEnumerable<Article> model = _articles.GetAllForUser(username, sortByDate, desc);

        PagingInfo p = new PagingInfo()
        {
            CurrentPage = page,
            ItemsPerPage = _articlesPerPage,
            TotalItems = model.Count()
        };

        ViewBag.PageInfo = p;

        model = model
            .Skip(p.ItemsPerPage * (p.CurrentPage - 1))
            .Take(p.ItemsPerPage);


        ViewBag.username = username;
        ViewBag.sortBy = sortBy;
        ViewBag.ascDesc = ascDesc;

        return View(model);
    }

    [Authorize]
    [HttpPost]
    public ActionResult EditAboutInfo(string aboutInfo)
    {
        var user = _users.Get(User.Identity.Name) with { about = aboutInfo };

        _users.Update(user);

        return Ok("Информация о себе изменена");
    }

    [Authorize]
    [HttpPost]
    public ActionResult EditContacts(string contacts)
    {
        var user = _users.Get(User.Identity.Name) with { contacts = contacts };

        _users.Update(user);

        return Ok("Контакты изменены");
    }

    [Authorize]
    [HttpPost]
    public ActionResult Ban(string username, string reason)
    {
        var userBanning = _users.Get(User.Identity.Name);

        if (!userBanning.is_moderator)
        {
            return Unauthorized();
        }

        var toBeBanned = _users.Get(username);

        if (toBeBanned == null || toBeBanned.is_moderator)
        {
            return BadRequest();
        }

        _users.Ban(username, reason);

        _logger.LogAction("Бан", userBanning.login, $"Забаненный: {toBeBanned.login}, причина: {reason}");

        return Ok($"{username} забанен");
    }

    [Authorize]
    [HttpPost]
    public ActionResult Unban(string username)
    {
        var userUnbanning = _users.Get(User.Identity.Name);

        if (!userUnbanning.is_moderator)
        {
            return Unauthorized();
        }

        var toBeUnbanned = _users.Get(username);

        if (toBeUnbanned == null || toBeUnbanned.is_moderator)
        {
            return BadRequest();
        }

        _users.Unban(username);

        _logger.LogAction("Разбан", userUnbanning.login, $"Разбаненный: {toBeUnbanned.login}");

        return Ok($"{username} разбанен");
    }
}
