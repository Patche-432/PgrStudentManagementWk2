using Microsoft.EntityFrameworkCore;
using PgrStudentManagement.Web.Modules.AcademicSupervisors.Data;
using PgrStudentManagement.Web.Modules.AcademicSupervisors.Services;
using PgrStudentManagement.Web.Modules.Students.Contracts;
using PgrStudentManagement.Web.Modules.Students.Data;
using PgrStudentManagement.Web.Modules.Students.Services;
using PgrStudentManagement.Web.Modules.Students.Data.Daos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<StudentStorage>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddSingleton<AcademicSupervisorStorage>();
builder.Services.AddScoped<AcademicSupervisorService>();
builder.Services.AddScoped<IStudentLookup>(provider =>provider.GetRequiredService<StudentService>());
builder.Services.AddDbContext<StudentManagementDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("StudentManagementDatabase")));
builder.Services.AddScoped<EnrolmentDao>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
    .GetRequiredService<StudentManagementDbContext>();
    await context.Database.MigrateAsync();
    await StudentDataSeeder.SeedAsync(context);
}

app.Run();
