using Microsoft.AspNetCore.Identity.Data;

namespace MusicBlogs.Models;

public record User(string Login, DateTime CreatedAt, string Contacts,
    string About, string Password, bool IsModerator);