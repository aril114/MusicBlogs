using Microsoft.AspNetCore.Identity.Data;

namespace MusicBlogs.Models;

/// <summary>
/// Зарегистрированный пользователь
/// </summary>
/// <param name="Login">Логин</param>
/// <param name="CreatedAt">Дата и время создания</param>
/// <param name="Contacts">Контакты</param>
/// <param name="About">Текст о себе</param>
/// <param name="Password">Пароль</param>
/// <param name="IsModerator">Является ли модератором</param>
public record User(string Login, DateTime CreatedAt, string Contacts,
    string About, string Password, bool IsModerator);