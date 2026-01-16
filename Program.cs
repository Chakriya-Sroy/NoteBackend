using NoteBackend.Data;
using NoteBackend.Repositories;
using NoteBackend.Helpers;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SETUP SERVICES ---
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();

// Allow Vue Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

var connectionString = app.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString))
{
    await DbHelper.SetupDatabase(connectionString);
}
// --- 3. CONFIGURE HTTP PIPELINE ---
app.UseCors("AllowVue");
app.UseAuthorization();
app.MapControllers();
app.Run();
