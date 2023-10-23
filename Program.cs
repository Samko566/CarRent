using CarRent.Areas.Identity.Data;
using CarRent.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Отримання рядка підключення з конфігурації та додавання контексту бази даних до сервісів
var connectionString = builder.Configuration.GetConnectionString("CarRentDbContextConnection") ?? throw new InvalidOperationException("Connection string 'CarRentDbContextConnection' not found.");
builder.Services.AddDbContext<CarRentDbContext>(options => options.UseSqlServer(connectionString));

// Додавання стандартної авторизації до сервісів
builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<CarRentDbContext>();

// Додавання MVC контролерів та Razor сторінок у сервіси
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Додавання контексту бази даних автомобілів до сервісів
builder.Services.AddDbContext<CarDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CarDbContext") ?? throw new InvalidOperationException("Connection string 'CarDbContext' not found.")));

// Створення об'єкта програми
var app = builder.Build();

// Налаштування конвеєра обробки запитів
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Налаштування маршрутів для контролерів та Razor сторінок
app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Запуск програми
app.Run();