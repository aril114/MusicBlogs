using Microsoft.AspNetCore.Identity.Data;

namespace MusicBlogs.Models;

/// <summary>
/// Зарегистрированный пользователь
/// </summary>
/// <param name="login">Логин</param>
/// <param name="created_at">Дата и время создания</param>
/// <param name="contacts">Контакты</param>
/// <param name="about">Текст о себе</param>
/// <param name="password">Пароль</param>
/// <param name="is_moderator">Является ли модератором</param>
public record User(string login, DateTime created_at, string contacts,
    string about, string password, bool is_moderator, bool is_banned, string ban_reason);