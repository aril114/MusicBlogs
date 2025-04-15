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
                (@Content, @Title, @UserLogin)
                """;
            db.Execute(sqlQuery, newDraftArticle);
        }
    }

    public void Delete(DraftArticle DraftArticle)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = "DELETE FROM \"DraftArticles\" WHERE id = @Id and \"login_Users\"=@UserLogin";
            db.Execute(sqlQuery, DraftArticle);
        }
    }

    public DraftArticle? Get(string id, string userLogin)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<DraftArticle>("""
                SELECT * FROM "DraftArticles"
                WHERE id = @id AND "login_Users"=@userLogin
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
                SET content = @Content, title = @Title
                WHERE id = @Id AND "login_Users" = @UserLogin
                """;
            db.Execute(sqlQuery, DraftArticle);
        }
    }
}
