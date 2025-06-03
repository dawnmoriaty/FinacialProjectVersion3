using FinacialProjectVersion3.Data;
using FinacialProjectVersion3.Repository.Impl;
using FinacialProjectVersion3.Repository;
using Microsoft.EntityFrameworkCore;
using FinacialProjectVersion3.Services.Impl;
using FinacialProjectVersion3.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký sử dụng repository
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // THÊM DÒNG NÀY

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// TẠO THỦ MỤC IMG SAU KHI APP ĐÃ ĐƯỢC BUILD
try
{
    var imgDirectory = Path.Combine(app.Environment.WebRootPath ?? "wwwroot", "img");
    if (!Directory.Exists(imgDirectory))
    {
        Directory.CreateDirectory(imgDirectory);
    }
}
catch (Exception ex)
{
    // Log lỗi nếu cần
    Console.WriteLine($"Error creating img directory: {ex.Message}");
}

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();