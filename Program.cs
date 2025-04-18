using LabTrainer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Voeg appsettings.json toe aan de configuratie
builder.Configuration.AddJsonFile("appsettings.json");

// ✅ Voeg controllers (voor API zoals /api/sessions)
builder.Services.AddControllers();

// ✅ Voeg Entra ID-authenticatie toe
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd");

builder.Services.AddRazorPages()
    .AddMvcOptions(options =>
    {
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        options.Filters.Add(new AuthorizeFilter(policy));
    })
    .AddMicrosoftIdentityUI();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddHttpContextAccessor();

// ✅ Database context toevoegen
builder.Services.AddDbContext<LabTrainerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LabTrainerContext")));

var app = builder.Build();

// Configureer de HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); // ✅ Als je Entra ID gebruikt
app.UseAuthorization();

// ✅ Routes activeren
app.MapRazorPages().WithStaticAssets();
app.MapControllers(); // 👈 BELANGRIJK: voor API routes zoals /api/sessions

app.Run();
