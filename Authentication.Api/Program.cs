using Authentication.Application.Commands.CreateUserCommand;
using Authentication.Application.Services.JwtService;
using Authentication.Infrastructure.Data;
using Authentication.Infrastructure.Entities;
using Common;
using Common.Api.BaseConfiguration;
using Common.Settings;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
// using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5050); 
    options.AddServerHeader = false;
});

builder.AddBasicMicroserviceFeatures();
// Add services to the container
builder.Services.AddControllers().ConfigureApiBehaviorOptions(opt => { opt.SuppressModelStateInvalidFilter = true; });

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(SettingsSectionKey.JwtSettings));

builder.Services.AddDbContext<AuthenticationDbContext>(
    options => options.UseNpgsql(
        builder.Configuration.GetConnectionString(SettingsSectionKey.DatabaseDefaultConnection)));

builder.Services.AddIdentity<FitCoreUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthenticationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddControllers();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();