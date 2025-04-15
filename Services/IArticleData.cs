using MusicBlogs.Models;

namespace MusicBlogs.Services;

public interface IArticleData
{
    void Add(Article newArticle);
    void Delete(Article article);
    Article? Get(int id);
    IEnumerable<Article> GetAll();
    void Update(Article article);
}
