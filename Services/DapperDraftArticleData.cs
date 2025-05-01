using Dapper;
using MusicBlogs.Models;
using Npgsql;
using System.Data;

namespace MusicBlogs.Services;

public class DapperDraftArticleData : IDraftArticleData
{
    /// <summary>
    /// Строка подключения к БД
    /// </summary>
    private string _cn;

    public DapperDraftArticleData(IConfiguration configuration)
    {
        _cn = configuration.GetSection("ConnectionStrings")["DefaultConnection"];
    }

    public void Add(string content, string title, string tags, string login_Users)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                INSERT INTO "DraftArticles" (content, title, tags, "login_Users") VALUES
                (@content, @title, @tags, @login_Users)
                """;
            db.Execute(sqlQuery, new { content, title, tags, login_Users });
        }
    }

    public void Delete(DraftArticle DraftArticle)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = "DELETE FROM \"DraftArticles\" WHERE id = @id and \"login_Users\"=@login_Users";
            db.Execute(sqlQuery, DraftArticle);
        }
    }

    public DraftArticle? Get(int id, string userLogin)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<DraftArticle>("""
                SELECT * FROM "DraftArticles"
                WHERE id = @id AND "login_Users" = @userLogin
                """, new { id, userLogin }).FirstOrDefault();
        }
    }

    public IEnumerable<DraftArticle> GetAll()
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<DraftArticle>("""
                SELECT * FROM "DraftArticles"
                """).ToList();
                
        }
    }

    public IEnumerable<DraftArticle> GetAllForUser(string userLogin)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<DraftArticle>("""
                SELECT * FROM "DraftArticles"
                WHERE "login_Users" = @userLogin
                ORDER BY id DESC
                """, new { userLogin }).ToList();
        }
    }

    public void Update(DraftArticle DraftArticle)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                UPDATE "DraftArticles"
                SET content = @content, title = @title, tags = @tags
                WHERE id = @id AND "login_Users" = @login_Users
                """;
            db.Execute(sqlQuery, DraftArticle);
        }
    }
}
