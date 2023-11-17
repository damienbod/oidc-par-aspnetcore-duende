using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;

namespace WebCodeFlowPkceClient;

internal static class HostingExtensions
{
    private static IWebHostEnvironment? _env;
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        _env = builder.Environment;

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "cookie";
            options.DefaultChallengeScheme = "oidc";
        })
        .AddCookie("cookie", options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = false;
        })
        .AddOpenIdConnect("oidc", options =>
        {
            options.Authority = "https://localhost:5001";
            options.ClientId = "web-par";
            options.ClientSecret = "ddedF4f289k$3eDa23ed0iTk4Raq&tttk23d08nhzd";
            options.ResponseType = "code";
            options.ResponseMode = "query";
            options.UsePkce = true;

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("offline_access");
            options.GetClaimsFromUserInfoEndpoint = true;
            options.SaveTokens = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "role"
            };
        });

        var privatePem = File.ReadAllText(Path.Combine(_env.ContentRootPath,
            "ecdsa384-private.pem"));
        var publicPem = File.ReadAllText(Path.Combine(_env.ContentRootPath,
            "ecdsa384-public.pem"));
        var ecdsaCertificate = X509Certificate2.CreateFromPem(publicPem, privatePem);
        var ecdsaCertificateKey = new ECDsaSecurityKey(ecdsaCertificate.GetECDsaPrivateKey());

        //var privatePem = File.ReadAllText(Path.Combine(_environment.ContentRootPath, 
        //    "rsa256-private.pem"));
        //var publicPem = File.ReadAllText(Path.Combine(_environment.ContentRootPath, 
        //    "rsa256-public.pem"));
        //var rsaCertificate = X509Certificate2.CreateFromPem(publicPem, privatePem);
        //var rsaCertificateKey = new RsaSecurityKey(rsaCertificate.GetRSAPrivateKey());

        services.AddRazorPages();

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        IdentityModelEventSource.ShowPII = true;

        app.UseSerilogRequestLogging();

        if (_env!.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        return app;
    }
}