var builder = WebApplication.CreateBuilder(args);

RegisterServices(builder.Services);

var app = builder.Build();

Configure(app);

var APIs = app.Services.GetServices<IAPI>();
foreach (var API in APIs)
{
    if (API is null) throw new InvalidProgramException("API not found");
    API.Register(app);
}

app.Run();

void RegisterServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.AddDbContext<HotelDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite-Hotels"));
    });

    services.AddScoped<IHotelRepository, HotelRepository>();
    services.AddSingleton<ITokenService>(new TokenService());
    services.AddSingleton<IUserRepository>(new UserRepository());
    services.AddAuthorization();

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "default")
            )
        };
    });

    services.AddTransient<IAPI, HotelsAPI>();
    services.AddTransient<IAPI, AuthAPI>();

}

void Configure(WebApplication app) 
{
    app.UseAuthentication();
    app.UseAuthorization();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
        db.Database.EnsureCreated();
    }

    app.UseHttpsRedirection();
}