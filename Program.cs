using System.IO;
using Foodopia.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Use full project path for SQLite DB
var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Foodopia.db");

// ✅ Register DbContext
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite($"Data Source={dbPath}")
);

// ✅ Add MVC Controllers + Views
builder.Services.AddControllersWithViews();

// ✅ Add Session Service
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Add HttpContext Accessor (must be before builder.Build)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ✅ Seed Sample Admin + User Data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();



    // 🔸 Seed Test User
    if (!context.Users.Any())
    {
        context.Users.Add(new Foodopia.Models.Users
        {
            Name = "Demo User",
            User_Email = "user@foodopia.com",
            Password = "12345"
        });
        context.SaveChanges();
    }
}

// ✅ Middleware configuration
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ✅ Enable session before authorization
app.UseSession();

// ✅ Role-based route protection middleware
app.UseMiddleware<Foodopia.Middleware.RoleAccessMiddleware>();

app.UseAuthorization();

// ✅ Default route mapping
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
