using Microsoft.AspNetCore.Authentication.Cookies;
using MusicBlogs.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ModlogService>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/logreg");

builder.Services.AddSingleton<IArticleData, DapperArticleData>();
builder.Services.AddSingleton<IDraftArticleData, DapperDraftArticleData>();
builder.Services.AddSingleton<ICommentData, DapperCommentData>();
builder.Services.AddSingleton<IUserData, DapperUserData>();
builder.Services.AddSingleton<ILikeData, DapperLikeData>();
builder.Services.AddSingleton<ITagData, DapperTagData>();
builder.Services.AddSingleton<IBookmarkData, DapperBookmarkData>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
