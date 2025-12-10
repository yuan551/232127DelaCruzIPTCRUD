using _232127DelaCruzIPTCRUD.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register services for dependency injection
builder.Services.AddScoped<_232127DelaCruzUserService>();
builder.Services.AddScoped<_232127DelaCruzStudentService>();
builder.Services.AddScoped<_232127DelaCruzCourseService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=_232127DelaCruzUser}/{action=Login}/{id?}");

app.Run();
