using System;
using System.Threading.Tasks;
using FunctionDallegram.Options;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FunctionDallegram.Services;

public class InstaService
{
    private IInstaApi? _instaApi;
    private readonly InstaOptions _instaOptions;
    private readonly ILogger<InstaService> _logger;

    public InstaService(IOptions<InstaOptions> instaOptions, ILogger<InstaService> logger)
    {
        _instaOptions = instaOptions?.Value ?? throw new ArgumentNullException(nameof(instaOptions));
        _logger = logger;
    }

    public async Task<string> Login()
    {
        var username = _instaOptions.Username;

        _logger.LogInformation("login started by username = " + username);


        _instaApi = InstaApiBuilder.CreateBuilder()
            .SetUser(UserSessionData.ForUsername(username).WithPassword(_instaOptions.Password))
            .SetRequestDelay(RequestDelay.FromSeconds(2, 2))
            .Build();

        await _instaApi.SendRequestsBeforeLoginAsync();
        var result = await _instaApi.LoginAsync();

        if (!result.Succeeded || !_instaApi.IsUserAuthenticated)
        {
            throw new Exception(result.Info.Message);
        }

        await _instaApi.SendRequestsAfterLoginAsync();

        var stateData = await _instaApi.GetStateDataAsStringAsync();
        return stateData;
    }

    public async Task Login(string sessionData)
    {

        _instaApi = InstaApiBuilder.CreateBuilder()
            .SetUser(UserSessionData.Empty)
            .SetRequestDelay(RequestDelay.FromSeconds(2, 2))
            .Build();

        await _instaApi.LoadStateDataFromStringAsync(sessionData);

        if (!_instaApi.IsUserAuthenticated) throw new Exception("Not Authorized");
    }

    public async Task<string> PostPhoto(string imageUrl, string caption)
    {
        _logger.LogInformation("PostPhoto");

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            throw new ArgumentException($"{nameof(imageUrl)} is null or empty", nameof(imageUrl));
        }


        var image = new InstaImageUpload(imageUrl);

        var result = await _instaApi.MediaProcessor.UploadPhotoAsync(image, caption);

        if (!result.Succeeded)
        {
            throw new Exception(result.Info.Message);
        }

        return result.Value.Pk;

    }

    public async Task<string> PostPhoto(byte[] imageBytes, string caption)
    {

        var image = new InstaImageUpload { ImageBytes = imageBytes };

        var result = await _instaApi.MediaProcessor.UploadPhotoAsync(image, caption);

        if (!result.Succeeded)
        {
            if (result.Info.ResponseType == ResponseType.CheckPointRequired)
            {
                await _instaApi.RequestVerifyCodeToEmailForChallengeRequireAsync();
            }

            throw new Exception(result.Info.Message);
        }

        return result.Value.Pk;

    }

}