using Microsoft.EntityFrameworkCore;
using RetroGaming.API.Middleware;
using RetroGaming.BLL.Mappings;
using RetroGaming.BLL.Services;
using RetroGaming.BLL.Services.Interfaces;
using RetroGaming.DAL.Context;
using RetroGaming.DAL.Repositories;
using RetroGaming.DAL.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── Database ────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ── AutoMapper ──────────────────────────────────────────────
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ── Repositories (DAL) ──────────────────────────────────────
builder.Services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
builder.Services.AddScoped<IConsoleRepository, ConsoleRepository>();

// ── Services (BLL) ──────────────────────────────────────────
builder.Services.AddScoped<IManufacturerService, ManufacturerService>();
builder.Services.AddScoped<IConsoleService, ConsoleService>();

// ── Controllers + Swagger ───────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "RetroGaming API", Version = "v1" });
});

// ── CORS ────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueDev", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174",
                "http://localhost:5175",
                "http://localhost:5176")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ── Middleware pipeline ──────────────────────────────────────
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowVueDev");
app.UseAuthorization();
app.MapControllers();

app.Run();