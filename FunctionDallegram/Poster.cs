using System;
using System.Net.Http;
using System.Threading.Tasks;
using FunctionDallegram.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunctionDallegram
{
    public class Poster
    {
        private readonly OpenaiService _openaiService;
        private readonly ImageService _imageService;
        private readonly InstaService _instaService;
        private readonly HttpClient _httpClient;

        public Poster(OpenaiService openaiService, ImageService imageService, InstaService instaService, HttpClient httpClient)
        {
            _openaiService = openaiService ?? throw new ArgumentNullException(nameof(openaiService));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _instaService = instaService ?? throw new ArgumentNullException(nameof(instaService));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        [FunctionName("Poster")]
        public async Task Run([TimerTrigger("*/10 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var title = "comics style deadpool looks like panda and drinking beer";
            var url = await _openaiService.GenerateImage(title);

            //var url = "https://cdn.pixabay.com/photo/2015/10/01/17/17/car-967387_1280.png";

            var pngByteArray = await _httpClient.GetByteArrayAsync(url);

            var jpegByteArray = await _imageService.ToJpeg(pngByteArray);

            await _instaService.Login();

            var caption = Constants.Constants.Captions[new Random().Next(Constants.Constants.Captions.Count)];

            await _instaService.PostPhoto(jpegByteArray, caption);
        }
    }
}
