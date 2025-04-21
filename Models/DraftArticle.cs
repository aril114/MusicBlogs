namespace MusicBlogs.Models;

/// <summary>
/// 
/// </summary>
/// <param name="id">ID, часть первичного ключа</param>
/// <param name="content">Текст</param>
/// <param name="title">Заголовок</param>
/// <param name="login_Users">Логин автора, часть первичного ключа</param>
public record DraftArticle(int id, string content, string title, string login_Users);
