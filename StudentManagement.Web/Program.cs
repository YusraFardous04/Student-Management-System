using StudentManagement.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Razor Pages
builder.Services.AddRazorPages();

// 2. Register all HttpClients BEFORE builder.Build()
builder.Services.AddHttpClient<StudentApiClient>(client => client.BaseAddress = new Uri("http://localhost:5253/"));
builder.Services.AddHttpClient<TeacherApiClient>(client => client.BaseAddress = new Uri("http://localhost:5253/"));
builder.Services.AddHttpClient<SubjectApiClient>(client => client.BaseAddress = new Uri("http://localhost:5253/"));
builder.Services.AddHttpClient<EnrollmentApiClient>(client => client.BaseAddress = new Uri("http://localhost:5253/"));
builder.Services.AddHttpClient<AttendanceApiClient>(client => client.BaseAddress = new Uri("http://localhost:5253/"));
builder.Services.AddHttpClient<ImportApiClient>(client => client.BaseAddress = new Uri("http://localhost:5253/"));
builder.Services.AddHttpClient<ClassAssignmentApiClient>(client => client.BaseAddress = new Uri("http://localhost:5253/"));
builder.Services.AddHttpClient<DashboardApiClient>(client => client.BaseAddress = new Uri("http://localhost:5253/"));

builder.Services.AddHttpClient<ChatApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5253/");
    client.Timeout = TimeSpan.FromMinutes(2); // the model can take a while to answer
});

// 3. Build the Application
var app = builder.Build();

// 4. Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();