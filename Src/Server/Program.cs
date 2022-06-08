using ChatRoom.Core.Application.Services.DependencyInjection;
using ChatRoom.Infrastructure.Database.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDatabaseRepositories();
builder.Services.AddApplicationServices();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ChatRoom.BootCamp.Api", Version = "v1" });
});

builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatRoom.BootCamp.Api"));
}

app.UseRouting();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();