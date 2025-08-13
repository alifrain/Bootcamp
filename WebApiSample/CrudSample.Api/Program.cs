using CrudSample.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Basic services ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "CrudSample API", Version = "v1" });
    // kalau nanti pakai JWT, definisikan security scheme di sini
});

// --- Register AppDbContext (Fluent API configs are in the DbContext assembly) ---
// Gunakan SQLite untuk development cepat; ganti ke UseSqlServer(...) jika perlu.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
// Repositories & UoW
builder.Services.AddScoped(typeof(CrudSample.Api.Repositories.Interfaces.IRepository<>),
                           typeof(CrudSample.Api.Repositories.Implementations.EfRepository<>));
builder.Services.AddScoped<CrudSample.Api.Repositories.Interfaces.IEmployeeRepository,
                           CrudSample.Api.Repositories.Implementations.EmployeeRepository>();
builder.Services.AddScoped<CrudSample.Api.Repositories.Implementations.IUnitOfWork,
                           CrudSample.Api.Repositories.Implementations.UnitOfWork>();

// Services
builder.Services.AddScoped<CrudSample.Api.Services.Interfaces.IEmployeeService,
                           CrudSample.Api.Services.Implementations.EmployeeService>();
var app = builder.Build();

// --- Middleware pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CrudSample API v1"));
}

app.UseAuthorization();

app.MapControllers();
app.Run();
