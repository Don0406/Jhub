using Microsoft.EntityFrameworkCore;
using _UPDATED__FrontEnd_Capstone.Models;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the Database Context using the official Oracle MySQL driver
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySQL(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Global PH culture for money formatting
var phCulture = new CultureInfo("en-PH");
phCulture.NumberFormat.CurrencySymbol = "₱";
phCulture.NumberFormat.CurrencyPositivePattern = 0; // ₱1,234.56
phCulture.NumberFormat.CurrencyNegativePattern = 1; // -₱1,234.56
CultureInfo.DefaultThreadCurrentCulture = phCulture;
CultureInfo.DefaultThreadCurrentUICulture = phCulture;

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Boot into guest tour discovery — no login required
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Booking}/{action=TourDiscovery}/{id?}");

app.Run();
