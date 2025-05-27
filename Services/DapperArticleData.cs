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

    public int Add(string content, string title, string excerpt, string preview_img, string login_Users)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            string sqlQuery = """
                INSERT INTO "Articles" (content, title, excerpt, preview_img, "login_Users")
                VALUES (@content, @title, @excerpt, @preview_img, @login_Users)
                RETURNING id
                """;

            return db.Query<int>(sqlQuery, new { content, title, excerpt, preview_img, login_Users }).First();
        }
    }

    public void Delete(Article article)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = "DELETE FROM \"Articles\" where id = @id";
            db.Execute(sqlQuery, new { article.id });
        }
    }

    public Article? Get(int id)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<Article>("SELECT * FROM \"Articles\" WHERE id = @id", new { id })
                .FirstOrDefault();
        }
    }

    public IEnumerable<Article> GetAll(bool sortByDate = true, bool desc = true)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            string sortBy = sortByDate ? "published_at" : "rating";

            string ascDesc = desc ? "DESC" : "ASC";

            return db.Query<Article>($"SELECT * FROM \"Articles\" ORDER BY {sortBy} {ascDesc}").ToList();
        }
    }

    public IEnumerable<Article> GetAllForUser(string userLogin, bool sortByDate = true, bool desc = true)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            string sortBy = sortByDate ? "published_at" : "rating";

            string ascDesc = desc ? "DESC" : "ASC";

            return db.Query<Article>($"""
                SELECT * FROM "Articles"
                WHERE "login_Users" = @userLogin
                ORDER BY {sortBy} {ascDesc}
                """, new { userLogin }).ToList();

        }
    }

    // 
    public IEnumerable<Article> GetAllWithTags(string[] tags, bool sortByDate = true)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            string sortBy = sortByDate ? "published_at" : "rating";

            int tagCount = tags.Length;

            if (tagCount == 0)
            {
                return Enumerable.Empty<Article>();
            }

            string sqlQuery = """
                SELECT a.*
                FROM "Articles" a
                INNER JOIN "Tags" t ON a.id = t."id_Articles"
                WHERE t.name = ANY(@tags)
                GROUP BY a.id
                HAVING COUNT(t.name) = @tagCount
                ORDER BY @sortBy
                """;

            return db.Query<Article>(sqlQuery, new
            {
                tags,
                tagCount,
                sortBy
            });
        }
    }

    public IEnumerable<Article> Search(string? query, string[]? tags, bool searchInTitle = true, bool sortByDate = true, bool sortDesc = true)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            string searchIn = searchInTitle ? "title" : "content";

            string sortBy = sortByDate ? "published_at" : "rating";

            string sortOrder = sortDesc ? "DESC" : "ASC";

            var builder = new SqlBuilder();
            var template = builder.AddTemplate("""
                SELECT a.*
                FROM "Articles" a
                /**innerjoin**/
                /**where**/
                /**groupby**/
                /**having**/
                /**orderby**/
                """);

            if (tags != null && tags.Length > 0)
            {
                builder.InnerJoin("""
                    "Tags" t ON a.id = t."id_Articles"
                    """);
                builder.Where("t.name = ANY(@tags)", new { tags });
                builder.GroupBy("a.id");
                builder.Having("COUNT(t.name) = @tagCount", new { tagCount = tags.Length });
            }

            if (!string.IsNullOrEmpty(query))
            {
                builder.Where($"{searchIn} ~* @query", new { query });
            }

            builder.OrderBy($"{sortBy} {sortOrder}");

            return db.Query<Article>(template.RawSql, template.Parameters);
        }
    }

    public void Update(Article article)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                UPDATE "Articles" SET content = @content, title = @title, 
                "login_Users" = @login_Users, rating = @rating, published_at = @published_at
                WHERE id = @id
                """;
            db.Execute(sqlQuery, article);
        }
    }
}
