using MusicBlogs.Models;

namespace MusicBlogs.Services;

public interface IArticleData
{
    int Add(string content, string title, string excerpt, string preview_img, string login_Users);
    void Delete(Article article);
    Article? Get(int id);
    IEnumerable<Article> Search(string? query, string[]? tags, bool searchInTitle = true, bool sortByDate = true, bool sortDesc = true);
    IEnumerable<Article> GetAll(bool sortByDate = true, bool desc = true);
    IEnumerable<Article> GetAllWithTags(string[] tags, bool sortByDate = true);
    IEnumerable<Article> GetAllForUser(string userLogin, bool sortByDate = true, bool desc = true);
    void Update(Article article);
}
