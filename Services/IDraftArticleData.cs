using MusicBlogs.Models;

namespace MusicBlogs.Services;

public interface IDraftArticleData
{
    int Add(string content, string title, string tags, string excerpt, string preview_img, string login_Users);
    void Delete(DraftArticle draftArticle);
    DraftArticle? Get(int id, string userLogin);
    IEnumerable<DraftArticle> GetAllBeingModerated();
    IEnumerable<DraftArticle> GetAllForUser(string userLogin, bool being_moderated = false);
    void Update(DraftArticle draftArticle);
}