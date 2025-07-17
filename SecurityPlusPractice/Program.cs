using Microsoft.EntityFrameworkCore;
using SecurityPlusPractice.Data;
using SecurityPlusPractice.Services;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext and SQL Server with trusted cert
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection"),
		sqlOptions => sqlOptions.EnableRetryOnFailure()
	));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ?? Ensure DB and tables are created, then seed
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<AppDbContext>();
	var env = services.GetRequiredService<IWebHostEnvironment>();

	// ? Create DB and tables if they don’t exist
	context.Database.EnsureCreated();

	// ? Seed data from text files
	Seeder.SeedData(context, env);
}

// Default pipeline
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
//using Microsoft.EntityFrameworkCore;
//using SecurityPlusPractice.Data;
//using SecurityPlusPractice.Services;

//var builder = WebApplication.CreateBuilder(args);

//// 1?? Register AppDbContext with SQL Server
//builder.Services.AddDbContext<AppDbContext>(options =>
//	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// 2?? Register MVC services
//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//// 3?? Seed the database on startup
//using (var scope = app.Services.CreateScope())
//{
//	var services = scope.ServiceProvider;

//	// ? This works ONLY if AppDbContext is registered above!
//	var context = services.GetRequiredService<AppDbContext>();
//	var env = services.GetRequiredService<IWebHostEnvironment>();
//	Seeder.SeedData(context, env);
//}

//// 4?? Request pipeline
//if (!app.Environment.IsDevelopment())
//{
//	app.UseExceptionHandler("/Home/Error");
//	app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();
//app.UseAuthorization();

//app.MapControllerRoute(
//	name: "default",
//	pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();