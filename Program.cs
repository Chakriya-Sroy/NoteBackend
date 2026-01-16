using NoteBackend.Data;
using NoteBackend.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// IMPORTANT: Register Dapper Context and Repository
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();

// Add CORS for Vue frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // Vue default ports
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowVueFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();