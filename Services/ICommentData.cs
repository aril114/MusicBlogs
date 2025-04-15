using MusicBlogs.Models;

namespace MusicBlogs.Services;

public interface ICommentData
{
    void Add(Comment newComment);
    void Delete(Comment Comment);
    Comment? Get(string id, string userLogin);
    IEnumerable<Comment> GetAll();
    void Update(Comment Comment);
}
