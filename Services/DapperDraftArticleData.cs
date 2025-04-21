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

    public void Add(DraftArticle newDraftArticle)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                INSERT INTO "DraftArticles" (content, title, "login_Users") VALUES
                (@content, @title, @login_Users)
                """;
            db.Execute(sqlQuery, newDraftArticle);
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

    public DraftArticle? Get(string id, string userLogin)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<DraftArticle>("""
                SELECT * FROM "DraftArticles"
                WHERE id = @id AND "login_Users"=@login_Users
                """, new { id, userLogin }).FirstOrDefault();
        }
    }

    public IEnumerable<DraftArticle> GetAll()
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<DraftArticle>("SELECT * FROM \"DraftArticles\"").ToList();
        }
    }

    public void Update(DraftArticle DraftArticle)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                UPDATE "DraftArticles"
                SET content = @content, title = @title
                WHERE id = @id AND "login_Users" = @login_Users
                """;
            db.Execute(sqlQuery, DraftArticle);
        }
    }
}
