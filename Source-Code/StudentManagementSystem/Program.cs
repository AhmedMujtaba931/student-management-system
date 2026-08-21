using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await SeedIdentityAsync(app);

app.Run();

static async Task SeedIdentityAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Admin", "Student", "Teacher" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    const string adminEmail = "admin@sms.local";
    const string adminPassword = "Admin@12345";

    var admin = await userManager.FindByEmailAsync(adminEmail);

    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "System Administrator"
        };

        var result = await userManager.CreateAsync(admin, adminPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Unable to create default administrator: {errors}");
        }
    }

    if (!await userManager.IsInRoleAsync(admin, "Admin"))
        await userManager.AddToRoleAsync(admin, "Admin");

    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    const string teacherEmail = "teacher@sms.local";
    const string teacherPassword = "Teacher@12345";

    var teacherUser = await userManager.FindByEmailAsync(teacherEmail);

    if (teacherUser == null)
    {
        teacherUser = new ApplicationUser
        {
            UserName = teacherEmail,
            Email = teacherEmail,
            EmailConfirmed = true,
            FullName = "Demo Teacher"
        };

        var result = await userManager.CreateAsync(teacherUser, teacherPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(teacherUser, "Teacher");

            bool teacherProfileExists = await context.Teachers.AnyAsync(t => t.UserId == teacherUser.Id);

            if (!teacherProfileExists)
            {
                context.Teachers.Add(new Teacher
                {
                    UserId = teacherUser.Id,
                    EmployeeCode = "EMP-001",
                    FirstName = "Demo",
                    LastName = "Teacher",
                    Email = teacherEmail,
                    Department = "Computer Science",
                    Designation = "Lecturer",
                    Gender = "Other",
                    CreatedAt = DateTime.Now
                });

                await context.SaveChangesAsync();
            }
        }
    }
}