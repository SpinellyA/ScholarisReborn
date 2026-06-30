using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using ScholarisReborn.Components;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<MyDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();

// Without this, the cookie middleware challenges to its default "/Account/Login" (which doesn't
// exist in this app) instead of our actual login page, on any hard page load that fails [Authorize].
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/access-denied";
});

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Serves confidential PoR/TCG uploads. Authenticated, and authorized to admins or the file's owner.
app.MapGet("/files/{id:guid}", async (Guid id, MyDbContext db, ClaimsPrincipal user) =>
{
    var file = await db.StoredFiles.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
    if (file is null)
        return Results.NotFound();

    var isAdmin = user.IsInRole(ApplicationRoles.SuperAdmin) || user.IsInRole(ApplicationRoles.Admin);
    var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
    var isOwner = userIdClaim is not null && Guid.TryParse(userIdClaim, out var uid) && uid == file.OwnerUserId;

    if (!isAdmin && !isOwner)
        return Results.Forbid();

    return Results.File(file.Content, file.ContentType, file.FileName);
}).RequireAuthorization();

// School logos aren't confidential — any authenticated user can view them (lists, dashboards).
app.MapGet("/school-logo/{id:guid}", async (Guid id, MyDbContext db) =>
{
    var school = await db.Schools.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
    if (school?.Logo is null || school.Logo.Length == 0)
        return Results.NotFound();
    return Results.File(school.Logo, school.LogoContentType ?? "image/png");
}).RequireAuthorization();

app.Run();
