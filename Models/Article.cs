namespace MusicBlogs.Models;

/// <summary>
/// Опубликованная статья
/// </summary>
/// <param name="Id">ID, первичный ключ</param>
/// <param name="PublishedAt">Дата публикации</param>
/// <param name="rating">Рейтинг</param>
/// <param name="Content">Текст в формате HTML</param>
/// <param name="Title">Заголовок</param>
/// <param name="UserLogin">Логин автора</param>
public record Article(int Id, DateTime PublishedAt, int rating, string Content, string Title, string UserLogin);
