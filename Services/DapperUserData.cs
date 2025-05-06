using Dapper;
using MusicBlogs.Models;
using Npgsql;
using System.Data;

namespace MusicBlogs.Services;

public class DapperUserData : IUserData
{
    /// <summary>
    /// Строка подключения к БД
    /// </summary>
    private string _cn;

    public DapperUserData(IConfiguration configuration)
    {
        _cn = configuration.GetSection("ConnectionStrings")["DefaultConnection"];
    }

    public void Add(string login, string password)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                INSERT INTO "Users" (login, password) VALUES
                (@login, @password)
                """;
            db.Execute(sqlQuery, new { login, password });
        }
    }

    public void Delete(User user)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = "DELETE FROM \"Users\" where login = @login";
            db.Execute(sqlQuery, new { user.login });
        }
    }

    public User? Get(string login)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<User>("SELECT * FROM \"Users\" WHERE login = @login", new { login })
                .FirstOrDefault();
        }
    }

    public IEnumerable<User> GetAll()
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<User>("SELECT * FROM \"Users\"").ToList();
        }
    }

    public void Update(User user)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                UPDATE "Users"
                SET created_at = @created_at, contacts = @contacts, about = @about,
                pasword = @pasword, is_moderator = @is_moderator, is_banned = @is_banned, ban_reason = @ban_reason
                WHERE login = @login
                """;
            db.Execute(sqlQuery, user);
        }
    }

    public void Ban(string login, string reason)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                UPDATE "Users"
                SET is_banned = true, ban_reason = @reason
                WHERE login = @login
                """;
            db.Execute(sqlQuery, new { login, reason });
        }
    }

    public void Unban(string login)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                UPDATE "Users"
                SET is_banned = false, ban_reason = NULL
                WHERE login = @login
                """;
            db.Execute(sqlQuery, new { login });
        }
    }
}
