using E_Administration.Data;
using E_Administration.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DemoDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DemoConnectionString")));

// Thêm dịch vụ xác thực bằng Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đường dẫn nếu chưa đăng nhập
        options.LogoutPath = "/Account/Logout"; // Đường dẫn đăng xuất
        options.AccessDeniedPath = "/Account/AccessDenied";
    });
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
    });

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



// Route cho các controller trong Area
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


// Route mặc định cho các controller bên ngoài Area
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



// Đặt trang mặc định là Home trong Area User
/*app.MapControllerRoute(
    name: "user_default",
    pattern: "",
    defaults: new { area = "User", controller = "Home", action = "Index" });*/


app.Run();
