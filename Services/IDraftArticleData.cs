using MusicBlogs.Models;

namespace MusicBlogs.Services;

public interface IDraftArticleData
{
    void Add(DraftArticle newDraftArticle);
    void Delete(DraftArticle DraftArticle);
    DraftArticle? Get(string id, string userLogin);
    IEnumerable<DraftArticle> GetAll();
    void Update(DraftArticle draftArticle);
}