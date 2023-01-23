using System.IO;
using ChatRoom.Core.Application.Services.Implementations;
using ChatRoom.Core.Domain.Abstractions.Services;
using ChatRoom.UserFunction;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace ChatRoom.UserFunction;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        builder.Services.AddSingleton<IConfiguration>(configuration);

        builder.Services.AddSingleton<IMessageService, MessageService>();
    }
}