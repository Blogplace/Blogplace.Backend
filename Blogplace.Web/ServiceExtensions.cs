using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Consts;
using Blogplace.Web.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace Blogplace.Web;

public static class ServiceExtensions
{
    public static IServiceCollection SetupAuth(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddOptions<AuthOptions>().Bind(config.GetSection("Auth")).ValidateDataAnnotations();
        services.AddOptions<CookieOptions>().Bind(config.GetSection("Auth:Cookie")).ValidateDataAnnotations();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IAuthorizationHandler, SessionCheckHandler>();
        services.AddSingleton<IAuthManager, AuthManager>();

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthConsts.WEB_POLICY, x => x.AddRequirements(new SessionCheckRequirement()).RequireRole(AuthConsts.ROLE_WEB));

        var options = new AuthOptions();
        config.GetSection("Auth").Bind(options);

        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireAudience = options.RequireAudience,
            ValidIssuer = options.ValidIssuer,
            ValidIssuers = options.ValidIssuers,
            ValidateActor = options.ValidateActor,
            ValidAudience = options.ValidAudience,
            ValidAudiences = options.ValidAudiences,
            ValidateAudience = options.ValidateAudience,
            ValidateIssuer = options.ValidateIssuer,
            ValidateLifetime = options.ValidateLifetime,
            ValidateTokenReplay = options.ValidateTokenReplay,
            ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
            SaveSigninToken = options.SaveSigninToken,
            RequireExpirationTime = options.RequireExpirationTime,
            RequireSignedTokens = options.RequireSignedTokens,
            ClockSkew = TimeSpan.Zero
        };

        if (string.IsNullOrWhiteSpace(options.IssuerSigningKey))
        {
            throw new ArgumentException("Missing issuer signing key.");
        }

        if (!string.IsNullOrWhiteSpace(options.AuthenticationType))
        {
            tokenValidationParameters.AuthenticationType = options.AuthenticationType;
        }

        var rawKey = Encoding.UTF8.GetBytes(options.IssuerSigningKey);
        tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(rawKey);

        if (!string.IsNullOrWhiteSpace(options.NameClaimType))
        {
            tokenValidationParameters.NameClaimType = options.NameClaimType;
        }

        if (!string.IsNullOrWhiteSpace(options.RoleClaimType))
        {
            tokenValidationParameters.RoleClaimType = options.RoleClaimType;
        }

        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.Authority = options.Authority;
            o.Audience = options.Audience;
            o.MetadataAddress = options.MetadataAddress;
            o.SaveToken = options.SaveToken;
            o.RefreshOnIssuerKeyNotFound = options.RefreshOnIssuerKeyNotFound;
            o.RequireHttpsMetadata = options.RequireHttpsMetadata;
            o.IncludeErrorDetails = options.IncludeErrorDetails;
            o.TokenValidationParameters = tokenValidationParameters;
            if (!string.IsNullOrWhiteSpace(options.Challenge))
            {
                o.Challenge = options.Challenge;
            }

            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.TryGetValue(AuthConsts.ACCESS_TOKEN_COOKIE, out var cookieToken))
                    {
                        context.Token = cookieToken;
                    }
                    else if (context.Request.Headers.TryGetValue(AuthConsts.AUTHORIZATION_HEADER, out var headerToken))
                    {
                        context.Token = headerToken.ToString().Replace("Bearer ", "");
                    }
                    else if (context.Request.Query.TryGetValue(AuthConsts.AUTHORIZATION_QUERY, out var queryToken))
                    {
                        context.Token = queryToken.ToString().Replace("Bearer ", "");
                    }

                    return Task.CompletedTask;
                },
            };
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

        return services;
    }

    public static IServiceCollection SetupMediatr(this IServiceCollection services)
    {
        var assemblies = typeof(Program)
            .Assembly
            .GetReferencedAssemblies()
            .Select(Assembly.Load)
            .ToArray();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(assemblies));

        return services;
    }
}