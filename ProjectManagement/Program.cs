using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using ProjectManagement;
using ProjectManagement.Data.Identity;
using ProjectManagement.Database.Seeds;
using ProjectManagement.Options;
using ProjectManagement.Utilities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    options => options.UseNodaTime()));

builder.Services.Configure<DefaultUserSettings>(builder.Configuration.GetSection("DefaultUser"));

builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    config.IncludeXmlComments(xmlPath);
});

builder.Services.AddSingleton<IClock>(SystemClock.Instance);

// Identity registration
builder.Services
    .AddIdentityCore<User>(options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services
    .AddAuthorization(config =>
    {
        config.AddPolicy(Policies.SUPERADMIN, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim(Claims.SUPERUSER);
        });
        config.AddPolicy(Policies.USER, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new ProjectAdminRequirement());
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

using var scope = app.Services.CreateScope();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
var userSettings = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<DefaultUserSettings>>().Value;
var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
var clock = scope.ServiceProvider.GetRequiredService<IClock>();

await dbContext.Database.MigrateAsync();

await UserSeed.CreateAdmin(clock, userManager, userSettings, dbContext);

app.Run();
