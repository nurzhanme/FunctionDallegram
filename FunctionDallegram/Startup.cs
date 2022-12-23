using FunctionDallegram.Options;
using FunctionDallegram.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.GPT3.Extensions;
using static Org.BouncyCastle.Math.EC.ECCurve;


[assembly: FunctionsStartup(typeof(FunctionDallegram.Startup))]

namespace FunctionDallegram;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var configuration = BuildConfiguration(builder.GetContext().ApplicationRootPath);

        builder.Services.Configure<InstaOptions>(configuration.GetSection(nameof(InstaOptions)));

        builder.Services.AddOpenAIService(o =>
        {
            o.ApiKey = configuration.GetSection("OpenAi:ApiKey").Value;
        });

        builder.Services.AddSingleton<OpenaiService>();
        builder.Services.AddSingleton<ImageService>();
        builder.Services.AddSingleton<InstaService>();
        builder.Services.AddHttpClient();
    }

    private IConfiguration BuildConfiguration(string applicationRootPath)
    {
        var config =
            new ConfigurationBuilder()
                .SetBasePath(applicationRootPath)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

        return config;
    }
}