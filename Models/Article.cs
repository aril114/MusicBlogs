namespace MusicBlogs.Models;

public record Article(int Id, DateTime PublishedAt, int rating, string Content, string Title, string UserLogin);
