
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace VorteonWeb;

public static class Functions
{

    private static readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider = new ();

    private static string GetContentType(string path)
    {
        if (!_fileExtensionContentTypeProvider.TryGetContentType(path, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }

    public static async Task ServeFile(this IFileInfo fileInfo, HttpContext context)
    {
        if (fileInfo.Exists)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = GetContentType(fileInfo.Name);
            context.Response.ContentLength = fileInfo.Length;
            using var stream = fileInfo.CreateReadStream();
            await stream.CopyToAsync(context.Response.Body);
            await context.Response.Body.FlushAsync();
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }
    }

    public static string ToTitleCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var cultureInfo = System.Globalization.CultureInfo.CurrentCulture;
        var textInfo = cultureInfo.TextInfo;
        return textInfo.ToTitleCase(value.ToLower());
    }

}
