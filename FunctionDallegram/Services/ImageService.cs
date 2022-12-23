using System.IO;
using SixLabors.ImageSharp;
using System.Threading.Tasks;

namespace FunctionDallegram.Services;

public class ImageService
{
    public async Task<byte[]> ToJpeg(byte[] source)
    {
        await using var sourceStream = new MemoryStream(source);
        using var image = await Image.LoadAsync(sourceStream);

        await using var targetStream = new MemoryStream();
        await image.SaveAsJpegAsync(targetStream);

        return targetStream.ToArray();
    }
}