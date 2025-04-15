using MusicBlogs.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IArticleData, DapperArticleData>();
builder.Services.AddSingleton<IDraftArticleData, DapperDraftArticleData>();
builder.Services.AddSingleton<ICommentData, DapperCommentData>();
builder.Services.AddSingleton<IUserData, DapperUserData>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
