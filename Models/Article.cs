﻿namespace MusicBlogs.Models;

/// <summary>
/// Опубликованная статья
/// </summary>
/// <param name="id">ID, первичный ключ</param>
/// <param name="published_at">Дата публикации</param>
/// <param name="rating">Рейтинг</param>
/// <param name="content">Текст в формате HTML</param>
/// <param name="title">Заголовок</param>
/// <param name="comment_count">Количество комментариев</param>
/// <param name="login_Users">Логин автора</param>
public record Article(int id, DateTime published_at, int rating, string content, string title, string excerpt, string preview_img, int comment_count, string login_Users);
