using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Configure configuration
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("ocelot.json")
    .AddJsonFile("ocelot.main.json")
    .AddOcelot(builder.Environment);

// Configure services
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
    policy => policy.WithOrigins("http://localhost")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed((_) => true)));

builder.Services.AddSingleton(provider =>
{
    RSA rsa = RSA.Create();
    var publicKey = @"MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAKKF9y2iN8dunqBdJBHQ8M1LQ4kK3b/mecdbOOs1VSfDa0p7bzFDWUepGV9NyTW0BwE/6gV7Akncc/MQfJdE9n8CAwEAAQ==";
    rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out int _);
    return new RsaSecurityKey(rsa);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var securityKey = builder.Services.BuildServiceProvider().GetRequiredService<RsaSecurityKey>();
    
    options.IncludeErrorDetails = builder.Environment.IsDevelopment();
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = securityKey,
        ValidAudience = "jwt-fitcore",
        ValidIssuer = "jwt-fitcore",
        RequireSignedTokens = true,
        RequireExpirationTime = true,
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddOcelot();

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseWebSockets();

await app.UseOcelot();

app.Run();