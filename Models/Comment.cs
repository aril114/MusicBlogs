namespace MusicBlogs.Models;

/// <summary>
/// Комментарий
/// </summary>
/// <param name="Id">ID, часть первичного ключа</param>
/// <param name="Content">Текст</param>
/// <param name="ArticleId">ID статьи, часть первичного ключа</param>
/// <param name="PublishedAt">Дата публикации</param>
/// <param name="UserLogin">Логин автора</param>
/// <param name="AnswerTo">ID комментария, на который данный отвечает</param>
public record Comment(int Id, string Content, int ArticleId, DateTime PublishedAt, string UserLogin, int AnswerTo);
