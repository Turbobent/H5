namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            IConfiguration Configuration = builder.Configuration;

            builder.Services.AddDbContext<AppDBContext>(options =>

                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddCors(options =>
            {

                options.AddPolicy("AllowAll", policy =>

                    policy.AllowAnyOrigin()

                          .AllowAnyMethod()

                          .AllowAnyHeader()

                          .WithExposedHeaders("Authorization"));

            });

            builder.Services.AddHostedService<RabbitMQService>();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(opt =>
            {

                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });

                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {

                    In = ParameterLocation.Header,

                    Description = "Enter JWT Token",

                    Name = "Authorization",

                    Type = SecuritySchemeType.Http,

                    BearerFormat = "JWT",

                    Scheme = "bearer"

                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {

                        new OpenApiSecurityScheme
                        {

                            Reference = new OpenApiReference
                            {

                                Type = ReferenceType.SecurityScheme,

                                Id = "Bearer"

                            }

                        },

                        new string[] {}

                    }

                });

            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

                .AddJwtBearer(options =>
                {

                    var key = Configuration["JwtSettings:Key"] ?? Environment.GetEnvironmentVariable("Key");

                    if (string.IsNullOrEmpty(key))
                    {

                        throw new InvalidOperationException("JWT Key is not configured.");

                    }

                    options.TokenValidationParameters = new TokenValidationParameters
                    {

                        ValidIssuer = Configuration["JwtSettings:Issuer"] ?? Environment.GetEnvironmentVariable("Issuer"),

                        ValidAudience = Configuration["JwtSettings:Audience"] ?? Environment.GetEnvironmentVariable("Audience"),

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

                        ValidateIssuer = true,

                        ValidateAudience = true,

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true

                    };

                });

            var app = builder.Build();

            app.UseCors("AllowAll");

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {

                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }

    }

}

