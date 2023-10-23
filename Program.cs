using CarRent.Areas.Identity.Data;
using CarRent.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ��������� ����� ���������� � ������������ �� ��������� ��������� ���� ����� �� ������
var connectionString = builder.Configuration.GetConnectionString("CarRentDbContextConnection") ?? throw new InvalidOperationException("Connection string 'CarRentDbContextConnection' not found.");
builder.Services.AddDbContext<CarRentDbContext>(options => options.UseSqlServer(connectionString));

// ��������� ���������� ����������� �� ������
builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<CarRentDbContext>();

// ��������� MVC ���������� �� Razor ������� � ������
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ��������� ��������� ���� ����� ��������� �� ������
builder.Services.AddDbContext<CarDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CarDbContext") ?? throw new InvalidOperationException("Connection string 'CarDbContext' not found.")));

// ��������� ��'���� ��������
var app = builder.Build();

// ������������ ������� ������� ������
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

// ������������ �������� ��� ���������� �� Razor �������
app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// ������ ��������
app.Run();