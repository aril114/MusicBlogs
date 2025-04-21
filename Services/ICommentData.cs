using MusicBlogs.Models;

namespace MusicBlogs.Services;

public interface ICommentData
{
    void Add(Comment newComment);
    void Delete(Comment Comment);
    Comment? Get(int id, string userLogin);
    IEnumerable<Comment> GetAll();
    IEnumerable<Comment> GetAllForArticle(int articleId);
    IEnumerable<Comment> GetAllForUser(string userLogin);
    void Update(Comment Comment);
}
