using FEALVES.AspNetMVCCore.Boilerpate.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext and Identity
builder.Services.AddSingleton<ApplicationDbContextFactory>();

builder.Services.AddScoped(serviceProvider =>
{
    var factory = serviceProvider.GetRequiredService<ApplicationDbContextFactory>();
    return factory.CreateDbContext();
});


builder.Services.AddControllersWithViews();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage(); // Detailed error pages for development
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();