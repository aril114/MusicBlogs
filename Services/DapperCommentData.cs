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
                (@Content, @Title, @UserLogin)
                """;
            db.Execute(sqlQuery, newComment);
        }
    }

    public void Delete(Comment Comment)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = "DELETE FROM \"Comments\" WHERE id = @Id and \"id_Articles\"=@ArticleId";
            db.Execute(sqlQuery, Comment);
        }
    }

    public Comment? Get(string id, string userLogin)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<Comment>("""
                SELECT * FROM "Comments"
                WHERE id = @id AND "login_Users"=@userLogin
                """, new { id, userLogin }).FirstOrDefault();
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
                SET content = @Content, title = @Title, "login_Users" = @UserLogin
                WHERE id = @Id
                """;
            db.Execute(sqlQuery, Comment);
        }
    }
}
