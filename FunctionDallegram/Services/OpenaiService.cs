using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenAI.GPT3.Interfaces;

namespace FunctionDallegram.Services;

public class OpenaiService
{

    private readonly IOpenAIService _openAiService;
    private readonly ILogger<OpenaiService> _logger;

    public OpenaiService(IOpenAIService openAiService, ILogger<OpenaiService> logger)
    {
        _openAiService = openAiService ?? throw new ArgumentNullException(nameof(openAiService));
        _logger = logger;
    }

    public async Task<string> GenerateImage(string input)
    {
        var result = await _openAiService.Image.CreateImage(input);

        if (result.Successful)
        {
            _logger.LogInformation("Image generation success");
            return result.Results.FirstOrDefault().Url;
        }

        _logger.LogInformation("Image generation NOT success");
        return string.Empty;
    }
}