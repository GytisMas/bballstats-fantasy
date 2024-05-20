
using BBallStats.Data;
using BBallStats2.Auth;
using BBallStats2.Auth.Model;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BBallStatsV2
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var builder = WebApplication.CreateBuilder(args);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy
            .WithOrigins("http://localhost:3000;https://coral-app-6z8fk.ondigitalocean.app/".Split(';'))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
                    });
            });

            builder.Services.AddDbContext<ForumDbContext>();
            builder.Services.AddIdentity<ForumRestUser, IdentityRole>()
                .AddEntityFrameworkStores<ForumDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddControllers();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();
            builder.Services.AddTransient<JwtTokenService>();
            builder.Services.AddScoped<AuthDbSeeder>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters.ValidAudience = builder.Configuration["Jwt:ValidAudience"];
                options.TokenValidationParameters.ValidIssuer = builder.Configuration["Jwt:ValidIssuer"];
                options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]));
            });

            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.AddAuthApi();
            app.UseCors();
            JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            using var scope = app.Services.CreateScope();
            var dbSeeder = scope.ServiceProvider.GetRequiredService<AuthDbSeeder>();

            var dbContext = scope.ServiceProvider.GetRequiredService<ForumDbContext>();
            dbContext.Database.Migrate();

            await dbSeeder.SeedAsync();

            app.Run();

        }
    }
}
