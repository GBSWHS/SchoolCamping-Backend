using System.Text.Json.Serialization;
using AspNetCoreRateLimit;
using SchoolCamping.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://+:3001");

Vault.AuthKey = builder.Configuration.GetSection("AdministratorPassword").Value;
Vault.RecaptchaKey = builder.Configuration.GetSection("RecaptchaSecretKey").Value;

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    x.JsonSerializerOptions.Converters.Add(new DateOnlyConverter("yyyy-MM-dd"));
});
builder.Services.AddOptions();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(x =>
{
    x.Cookie.Name = "redesigned";
    x.IdleTimeout = TimeSpan.FromHours(1);
    x.Cookie.IsEssential = true;
});
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

var app = builder.Build();

// if (bool.Parse(builder.Configuration.GetSection("EnableSwagger").Value))
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseCors(x =>
{
    x.AllowAnyOrigin();
    x.AllowAnyHeader();
});
app.Use(CaptchaMiddleware.InvokeAsync);
app.UseClientRateLimiting();
app.MapControllers();
app.UseSession();
app.Run();


class Vault
{
    public static string AuthKey { get; set; }
    public static string RecaptchaKey { get; set; }
}