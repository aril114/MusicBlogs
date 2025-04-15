namespace MusicBlogs.Models;

/// <summary>
/// 
/// </summary>
/// <param name="Id">ID, часть первичного ключа</param>
/// <param name="Content">Текст</param>
/// <param name="Title">Заголовок</param>
/// <param name="UserLogin">Логин автора, часть первичного ключа</param>
public record DraftArticle(int Id, string Content, string Title, string UserLogin);
