using Dapper;
using MusicBlogs.Models;
using Npgsql;
using System.Data;

namespace MusicBlogs.Services;

public class DapperTagData : ITagData
{
    /// <summary>
    /// Строка подключения к БД
    /// </summary>
    private string _cn;

    public DapperTagData(IConfiguration configuration)
    {
        _cn = configuration.GetSection("ConnectionStrings")["DefaultConnection"];
    }

    public void Add(string name, int id_Articles)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                INSERT INTO "Tags" ("name", "id_Articles") VALUES
                (@name, @id_Articles)
                """;
            db.Execute(sqlQuery, new { name, id_Articles });
        }
    }

    public void Delete(Tag tag)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                DELETE FROM "Tags"
                WHERE "name" = @name and "id_Articles" = @id_Articles
                """;
                
            db.Execute(sqlQuery, tag);
        }
    }

    public Tag? Get(string name, int id_Articles)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<Tag>("""
                SELECT * FROM "Tags"
                WHERE "name" = @name and "id_Articles" = @id_Articles
                """, new { name, id_Articles }).FirstOrDefault();
        }
    }

    public IEnumerable<Tag> GetAllForArticle(int id_Articles)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<Tag>("""
                SELECT * FROM "Tags"
                WHERE "id_Articles" = @id_Articles
                """, new { id_Articles });
        }
    }
}
