using Dapper;
using MusicBlogs.Models;
using Npgsql;
using System.Data;

namespace MusicBlogs.Services;

public class DapperArticleData : IArticleData
{
    /// <summary>
    /// Строка подключения к БД
    /// </summary>
    private string _cn;

    public DapperArticleData(IConfiguration configuration)
    {
        _cn = configuration.GetSection("ConnectionStrings")["DefaultConnection"];
    }

    public void Add(Article newArticle)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                INSERT INTO Articles (content, title, login_Users, rating) VALUES
                (@Content, @Title, @UserLogin, @Rating)
                """;
            db.Execute(sqlQuery, newArticle);
        }
    }

    public void Delete(Article article)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = "DELETE FROM Articles where id = @Id";
            db.Execute(sqlQuery, new { article.Id });
        }
    }

    public Article? Get(int id)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<Article>("SELECT * FROM Articles WHERE id = @id", new { id })
                .FirstOrDefault();
        }
    }

    public IEnumerable<Article> GetAll()
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<Article>("SELECT * FROM Articles").ToList();
        }
    }

    public void Update(Article article)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                UPDATE Articles SET content = @Content, title = @Title, 
                login_Users = @UserLogin, rating = @Rating, published_at = @PublishedAt
                WHERE id = @Id
                """;
            db.Execute(sqlQuery, article);
        }
    }
}
