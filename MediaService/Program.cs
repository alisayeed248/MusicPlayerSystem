using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MediaService.Services;
using Microsoft.Extensions.Options;
using Amazon.S3;

var builder = WebApplication.CreateBuilder(args);
var httpsPort = builder.Configuration.GetValue<int?>("ASPNETCORE_HTTPS_PORT");
//builder.Services.AddHttpsRedirection(options =>
//{
//    options.HttpsPort = httpsPort ?? 443;
//});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAWSService<IAmazonS3>(builder.Configuration.GetAWSOptions());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<VideoDownloadService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policyBuilder => policyBuilder.WithOrigins("https://localhost:44300")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
builder.Logging.ClearProviders(); // Optional: Clears all default providers.
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowSpecificOrigin");
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
