using MusicBlogs.Models;

namespace MusicBlogs.Services;

public interface IUserData
{
    void Add(User newUser);
    void Delete(User user);
    User? Get(string login);
    IEnumerable<User> GetAll();
    void Update(User user);
}