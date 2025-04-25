using MusicBlogs.Models;

namespace MusicBlogs.Services;

public interface IDraftArticleData
{
    void Add(string content, string title, string login_Users);
    void Delete(DraftArticle DraftArticle);
    DraftArticle? Get(int id, string userLogin);
    IEnumerable<DraftArticle> GetAll();
    IEnumerable<DraftArticle> GetAllForUser(string userLogin);
    void Update(DraftArticle draftArticle);
}