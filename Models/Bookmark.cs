namespace MusicBlogs.Models;

/// <summary>
/// Закладка статьи, чтобы пользователь мог прочесть статью позже
/// </summary>
/// <param name="created_at">Дата создания закладки</param>
/// <param name="login_Users">Логин пользователя</param>
/// <param name="id_Articles">ID статьи</param>

public record Bookmark(DateTime created_at, string login_Users, int id_Articles);
