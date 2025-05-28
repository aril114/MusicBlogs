using MusicBlogs.Models;

namespace MusicBlogs.Services;

public interface IBookmarkData
{
    void Add(string login_Users, int id_Articles);
    void Delete(Bookmark bookmark);
    Bookmark? Get(string login_Users, int id_Articles);
    IEnumerable<Bookmark> GetAllForUser(string login_Users);
}