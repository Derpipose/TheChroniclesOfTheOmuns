using System.Text;
using ChroniclesAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using PlayerApp.Models.Database;
// using Resend;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// // Resend
// builder.Services.AddOptions();
// builder.Services.AddHttpClient<ResendClient>();
// builder.Services.Configure<ResendClientOptions>(o =>
//     o.ApiToken = builder.Configuration["Resend:ApiKey"]!);
// builder.Services.AddTransient<IResend, ResendClient>();

// JWT
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddHttpClient();

builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer(); // required by Swashbuckle
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// Register services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PlayerContext>();
builder.Services.AddScoped<SciFiRaceService>();
builder.Services.AddScoped<SciFiSubraceService>();
builder.Services.AddScoped<ScifiSeedingService>();
builder.Services.AddScoped<ScifiWeaponService>();
builder.Services.AddScoped<ScifiWeaponInstanceService>();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowedOrigins", policy => {
        policy.WithOrigins(
            "http://localhost:5173",
            "http://localhost:5174",
            "http://localhost:5401",
            "http://derpserver.flyingfox-gourami.ts.net:5401",
            "http://derpserver.flyingfox-gourami.ts.net:5501",
            "https://derpserver.flyingfox-gourami.ts.net:5401",
            "https://derpserver.flyingfox-gourami.ts.net:5501"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// Auto-migrate on startup (dev-friendly)
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment()) {
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

