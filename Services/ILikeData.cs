using MusicBlogs.Models;

namespace MusicBlogs.Services;

public interface ILikeData
{
    void Add(string login_Users, int id_Articles);
    void Delete(Like like);
    Like? Get(string login_Users, int id_Articles);
    IEnumerable<Like> GetAllForUser(string login_Users);
}