﻿using Dapper;
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

    public void Add(string content, int id_Articles, string login_Users, int? answer_to = null)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            var sqlQuery = """
                INSERT INTO "Comments" (content, "id_Articles", "login_Users", answer_to) VALUES
                (@content, @id_Articles, @login_Users, @answer_to)
                """;
            db.Execute(sqlQuery, new { content, id_Articles, login_Users, answer_to });
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

    public Comment? Get(int id, int id_Articles)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            return db.Query<Comment>("""
                SELECT * FROM "Comments"
                WHERE id = @id AND "id_Articles"=@id_Articles
                """, new { id, id_Articles }).FirstOrDefault();
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
                SET content = @content, title = @title, "login_Users" = @login_Users, answer_to = @answer_to
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
                ORDER BY "published_at" DESC
                """;

            return db.Query<Comment>(sqlQuery, new { userLogin }).ToList();
        }
    }

    public IEnumerable<Comment> GetAnswers(int parentCommentId, int id_Articles)
    {
        using (IDbConnection db = new NpgsqlConnection(_cn))
        {
            string sqlQuery = """
                SELECT * FROM "Comments"
                WHERE "answer_to" = @parentCommentId
                AND "id_Articles" = @id_Articles
                """;

            return db.Query<Comment>(sqlQuery, new { parentCommentId, id_Articles }).ToList();
        }
    }
}
