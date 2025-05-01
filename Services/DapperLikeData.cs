using Dapper;
using MusicBlogs.Models;
using Npgsql;
using System.Data;

namespace MusicBlogs.Services;

public class DapperLikeData : ILikeData
{
    /// <summary>
    /// Строка подключения к БД
    /// </summary>
    private string _cn;

    public DapperLikeData(IConfiguration configuration)
    {
        _cn = configuration.GetSection("ConnectionStrings")["DefaultConnection"];
    }

    public void Add(string login_Users, int id_Articles)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                INSERT INTO "Likes" ("login_Users", "id_Articles") VALUES
                (@login_Users, @id_Articles)
                """;
            db.Execute(sqlQuery, new { login_Users, id_Articles });
        }
    }

    public void Delete(Like like)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                DELETE FROM "Likes"
                WHERE "login_Users" = @login_Users and "id_Articles" = @id_Articles
                """;
                
            db.Execute(sqlQuery, like);
        }
    }

    public Like? Get(string login_Users, int id_Articles)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<Like>("""
                SELECT * FROM "Likes"
                WHERE "login_Users" = @login_Users and "id_Articles" = @id_Articles
                """, new { login_Users, id_Articles }).FirstOrDefault();
        }
    }

    IEnumerable<Like> ILikeData.GetAllForUser(string login_Users)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<Like>("""
                SELECT * FROM "Likes"
                WHERE "login_Users" = @login_Users
                """, new { login_Users });
        }
    }
}
