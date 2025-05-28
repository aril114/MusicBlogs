using Dapper;
using MusicBlogs.Models;
using Npgsql;
using System.Data;

namespace MusicBlogs.Services;

public class DapperBookmarkData : IBookmarkData
{
    /// <summary>
    /// Строка подключения к БД
    /// </summary>
    private string _cn;

    public DapperBookmarkData(IConfiguration configuration)
    {
        _cn = configuration.GetSection("ConnectionStrings")["DefaultConnection"];
    }

    public void Add(string login_Users, int id_Articles)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            string sqlQuery = """
                INSERT INTO "Bookmarks" ("login_Users", "id_Articles")
                VALUES (@login_Users, @id_Articles)
                """;

            db.Execute(sqlQuery, new { login_Users, id_Articles });
        }
    }

    public void Delete(Bookmark bookmark)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
            DELETE FROM "Bookmarks"
            WHERE "login_Users" = @login_Users AND "id_Articles" = @id_Articles
            """;

            db.Execute(sqlQuery, bookmark);
        }
    }

    public Bookmark? Get(string login_Users, int id_Articles)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<Bookmark>("""
                SELECT * FROM "Bookmarks"
                WHERE "login_Users" = @login_Users AND "id_Articles" = @id_Articles
                """, new { login_Users, id_Articles })
                .FirstOrDefault();
        }
    }

    public IEnumerable<Bookmark> GetAllForUser(string login_Users)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<Bookmark>($"""
                SELECT * FROM "Bookmarks"
                WHERE "login_Users" = @login_Users
                ORDER BY "created_at" DESC
                """, new { login_Users }).ToList();
        }
    }
}
