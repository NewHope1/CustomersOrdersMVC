using CustomersOrdersMVC.Data;
using Microsoft.EntityFrameworkCore; // Add this using directive for 'UseSqlServer' extension method  

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});


// Add Kendo UI services to the services container.
builder.Services.AddKendo();

// Add services to the container.  
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CustomersOrdersContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
   pattern: "{controller=Home}/{action=Edit}/{id?}")
   .WithStaticAssets();

app.Run();
