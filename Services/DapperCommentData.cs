using Dapper;
using MusicBlogs.Models;
using Npgsql;
using System.Data;

namespace MusicBlogs.Services;

public class DapperCommentData : ICommentData
{
    /// <summary>
    /// Строка подключения к БД
    /// </summary>
    private string _cn;

    public DapperCommentData(IConfiguration configuration)
    {
        _cn = configuration.GetSection("ConnectionStrings")["DefaultConnection"];
    }

    public void Add(Comment newComment)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                INSERT INTO "Comments" (content, title, "login_Users") VALUES
                (@content, @title, @login_Users)
                """;
            db.Execute(sqlQuery, newComment);
        }
    }

    public void Delete(Comment Comment)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = "DELETE FROM \"Comments\" WHERE id = @id and \"id_Articles\"=@id_Articles";
            db.Execute(sqlQuery, Comment);
        }
    }

    public Comment? Get(int id, string login_Users)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<Comment>("""
                SELECT * FROM "Comments"
                WHERE id = @id AND "login_Users"=@login_Users
                """, new { id, login_Users }).FirstOrDefault();
        }
    }

    public IEnumerable<Comment> GetAll()
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<Comment>("SELECT * FROM \"Comments\"").ToList();
        }
    }

    public void Update(Comment Comment)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                UPDATE "Comments"
                SET content = @content, title = @title, "login_Users" = @login_Users
                WHERE id = @id
                """;
            db.Execute(sqlQuery, Comment);
        }
    }

    public IEnumerable<Comment> GetAllForArticle(int articleId)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            string sqlQuery = """
                SELECT * FROM "Comments"
                WHERE "id_Articles" = @articleId
                ORDER BY id
                """;

            return db.Query<Comment>(sqlQuery, new { articleId }).ToList();
        }
    }

    public IEnumerable<Comment> GetAllForUser(string userLogin)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            string sqlQuery = """
                SELECT * FROM "Comments"
                WHERE "login_Users" = @userLogin
                ORDER BY "published_at"
                """;

            return db.Query<Comment>(sqlQuery, new { userLogin }).ToList();
        }
    }
}
