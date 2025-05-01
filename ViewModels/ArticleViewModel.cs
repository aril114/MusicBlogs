using MusicBlogs.Models;

namespace MusicBlogs.ViewModels;

public record ArticleViewModel(int id,
    DateTime published_at,
    int rating,
    string content,
    string title,
    string author,
    IEnumerable<Comment> comments,
    IEnumerable<string> tags);