using MusicBlogs.Models;

namespace MusicBlogs.Services;

public interface ITagData
{
    void Add(string name, int id_Articles);
    void Delete(Tag tag);
    Tag? Get(string name, int id_Articles);
    IEnumerable<Tag> GetAllForArticle(int id_Articles);
}