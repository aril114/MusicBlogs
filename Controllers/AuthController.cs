using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicBlogs.Models;
using MusicBlogs.Services;
using System.Security.Claims;

namespace MusicBlogs.Controllers;

public class AuthController : Controller
{
    private IUserData _users;
    public AuthController(IUserData userData) 
    {
        _users = userData;
    }

    [Route("/logreg")]
    public IActionResult Logreg()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string login, string password)
    {
        User? user = _users.Get(login);

        if (user == null || user.password != password)
        {
            return Unauthorized();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.login)
        };

        var claimsIdentity = new ClaimsIdentity(claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        return SignIn(new ClaimsPrincipal(claimsIdentity),
            CookieAuthenticationDefaults.AuthenticationScheme);

        //return new ObjectResult(User);

        //return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Register(string login, string password)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, login)
        };

        var claimsIdentity = new ClaimsIdentity(claims,
            CookieAuthenticationDefaults.AuthenticationScheme);
        
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public IActionResult Logout()
    {
        SignOut(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
