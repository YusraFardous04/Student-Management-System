// This is the starting point of our entire app, the flow of the app starts from here.
// Each web app has to do three things:
// 1. Gather the tools (the database context, services, controllers, etc.).
// 2. Decide what happens to each incoming request (middleware pipeline).
// 3. Start listening for requests.

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Api.Data;
using StudentManagement.Api.models;
using StudentManagement.Api.Services;
using StudentManagement.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers with JSON ReferenceHandler settings to prevent object cycle issues
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Swagger documentation configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context registration
builder.Services.AddDbContext<StudentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection Service Lifetime Configurations (Scoped = 1 instance per HTTP Request)
builder.Services.AddScoped<IStudentServices, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IClassAssignmentService, ClassAssignmentService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddHttpClient<OllamaClient>();
builder.Services.AddScoped<IIndexingService, IndexingService>();
builder.Services.AddScoped<IRetrievalService, RetrievalService>();
builder.Services.AddScoped<IAnalyticsRetrievalService, AnalyticsRetrievalService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddSingleton<ChatMemoryStore>();
// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowRazorPages", policy =>
        policy.WithOrigins("https://localhost:5083", "http://localhost:5083")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowRazorPages");
app.UseAuthorization();

// Route Controller Mapping with Exception Diagnostic Handling
try
{
    app.MapControllers();
}
catch (System.Reflection.ReflectionTypeLoadException ex)
{
    foreach (var inner in ex.LoaderExceptions)
    {
        if (inner != null)
        {
            Console.WriteLine(inner.Message);
        }
    }
}


app.Run(); 