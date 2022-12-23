using System.Linq;
using System.Threading.Tasks;
using OpenAI.GPT3.Interfaces;

namespace FunctionDallegram.Services;

public class OpenaiService
{

    private readonly IOpenAIService _openAiService;

    public OpenaiService(IOpenAIService openAiService)
    {
        _openAiService = openAiService;
    }

    public async Task<string> GenerateImage(string input)
    {
        var result = await _openAiService.Image.CreateImage(input);

        if (result.Successful)
        {
            return result.Results.FirstOrDefault().Url;
        }

        return string.Empty;
    }
}