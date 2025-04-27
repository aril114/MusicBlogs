using MusicBlogs.Models;

namespace MusicBlogs.Services;

public interface IArticleData
{
    void Add(string content, string title, string login_Users);
    void Delete(Article article);
    Article? Get(int id);
    IEnumerable<Article> Search(string query, bool searchInTitle = true, bool sortByDate = true, bool sortDesc = true);
    IEnumerable<Article> GetAll(bool sortByDate = true);
    IEnumerable<Article> GetAllForUser(string userLogin);
    void Update(Article article);
}
