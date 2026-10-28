using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UTB.Utulek.Application.Abstraction;
using UTB.Utulek.Application.Implementation;
using UTB.Utulek.Domain.Repositories;
using UTB.Utulek.Infrastructure.Database;
using UTB.Utulek.Infrastructure.Identity;
using UTB.Utulek.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("MySQL");
var serverVersion = ServerVersion.AutoDetect(connectionString);
builder.Services.AddDbContext<UtulekDbContext>
    (optionBuilder => optionBuilder.UseMySql(connectionString, serverVersion));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<UtulekDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
builder.Services.AddScoped<ISpeciesRepository, SpeciesRepository>();

builder.Services.AddScoped<IAnimalAppService, AnimalAppService>();
builder.Services.AddScoped<ISpeciesAppService, SpeciesAppService>();

var app = builder.Build();

// Automatické použití migrací a seed rolí Admin, Manager, Zakaznik.
using (var scope = app.Services.CreateScope())
{
    await DatabaseInitializer.InitializeAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
