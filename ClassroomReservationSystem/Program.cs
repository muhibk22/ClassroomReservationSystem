using ClassroomReservationSystem.Components;
using ClassroomReservationSystem.Components.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserState>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<AssignedCourseService>();
builder.Services.AddScoped<ClassroomService>();


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
