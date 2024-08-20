using FEALVES.AspNetMVCCore.Boilerpate.Data;
using FEALVES.AspNetMVCCore.Boilerpate.Models;
using FEALVES.AspNetMVCCore.Boilerpate.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Configure services
Startup.ConfigureServices(builder.Services, builder.Configuration);

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
Startup.ConfigureMiddleware(app);

// Ensure the database is created, apply migrations, and create roles
await Startup.InitializeDatabase(app);

// Run the application
app.Run();