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

    public int Add(string content, string title, string tags, string excerpt, string preview_img, string login_Users)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                INSERT INTO "DraftArticles" (content, title, tags, excerpt, preview_img, "login_Users") VALUES
                (@content, @title, @tags, @excerpt, @preview_img, @login_Users)
                RETURNING id
                """;
            return db.Query<int>(sqlQuery, new { content, title, tags, excerpt, preview_img, login_Users }).First();
        }
    }

    public void Delete(DraftArticle draftArticle)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = "DELETE FROM \"DraftArticles\" WHERE id = @id and \"login_Users\"=@login_Users";
            db.Execute(sqlQuery, draftArticle);
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

    public IEnumerable<DraftArticle> GetAllBeingModerated()
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<DraftArticle>("""
                SELECT * FROM "DraftArticles"
                WHERE "is_being_moderated" = true
                """).ToList();
        }
    }

    public IEnumerable<DraftArticle> GetAllForUser(string userLogin, bool is_being_moderated = false)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<DraftArticle>("""
                SELECT * FROM "DraftArticles"
                WHERE "login_Users" = @userLogin
                AND "is_being_moderated" = @is_being_moderated
                ORDER BY id DESC
                """, new { userLogin, is_being_moderated }).ToList();
        }
    }

    public void Update(DraftArticle draftArticle)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                UPDATE "DraftArticles"
                SET content = @content, title = @title, tags = @tags, excerpt = @excerpt, preview_img = @preview_img, is_being_moderated = @is_being_moderated
                WHERE id = @id AND "login_Users" = @login_Users
                """;
            db.Execute(sqlQuery, draftArticle);
        }
    }
}
