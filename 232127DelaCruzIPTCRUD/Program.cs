using _232127DelaCruzIPTCRUD.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

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

// Quick startup check for DB connectivity and helpful guidance when it fails
try
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrWhiteSpace(connStr))
    {
        try
        {
            using var testConn = new SqlConnection(connStr);
            testConn.Open();
            logger.LogInformation("Database connection successful.");
            testConn.Close();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to connect to database using connection string from configuration. If you are running in a container, 'localhost' refers to the container, not your host machine. Use SQL authentication and point to the host (for example 'host.docker.internal' or an IP), then set the connection string in an environment variable 'ConnectionStrings__DefaultConnection'.");
        }
    }
}
catch
{
    // ignore logging errors during startup diagnostics
}

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
