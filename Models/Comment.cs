namespace MusicBlogs.Models;

public record Comment(int Id, string Content, int ArticleId, DateTime PublishedAt, string UserLogin, int AnswerTo);
