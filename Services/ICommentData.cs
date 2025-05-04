using MusicBlogs.Models;

namespace MusicBlogs.Services;

public interface ICommentData
{
    void Add(string content, int id_Articles, string login_Users, int? answer_to = null);
    void Delete(Comment Comment);
    Comment? Get(int id, int id_Articles);
    IEnumerable<Comment> GetAll();
    IEnumerable<Comment> GetAllForArticle(int articleId);
    IEnumerable<Comment> GetAllForUser(string userLogin);
    void Update(Comment Comment);
}
