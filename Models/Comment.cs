namespace MusicBlogs.Models;

/// <summary>
/// Комментарий
/// </summary>
/// <param name="id">ID, часть первичного ключа</param>
/// <param name="content">Текст</param>
/// <param name="id_Articles">ID статьи, часть первичного ключа</param>
/// <param name="published_at">Дата публикации</param>
/// <param name="login_Users">Логин автора</param>
/// <param name="answer_to">ID комментария, на который данный отвечает</param>
public record Comment(int id, string content, int id_Articles, DateTime published_at, string login_Users, int? answer_to);