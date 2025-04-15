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

    public void Add(User newUser)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                INSERT INTO "Users" (login, contacts, about, password) VALUES
                (@Login, @Contacts, @About, @Password)
                """;
            db.Execute(sqlQuery, newUser);
        }
    }

    public void Delete(User user)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = "DELETE FROM \"Users\" where login = @Login";
            db.Execute(sqlQuery, new { user.Login });
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
                SET created_at = @CreatedAt, contacts = @Contacts, about = @About,
                pasword = @Password, is_moderator = @IsModerator 
                WHERE login = @Login
                """;
            db.Execute(sqlQuery, user);
        }
    }
}
