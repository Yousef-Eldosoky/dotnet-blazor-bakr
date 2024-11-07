using System.Net;
using Bakr.Shared.Models;

namespace Bakr.Endpoints;

public static class FilesEndpoint
{
    const string getFileEndpointName = "GetFile";
    private static readonly string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    public static RouteGroupBuilder MapFileEndpoint(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("api/filesave").RequireAuthorization();

        group.MapPost("/", async (HttpRequest request, IHostEnvironment env, ILogger<Program> logger) =>
        {
            const long maxFileSize = 1024 * 1024 * 1;
            var resourcePath = new Uri($"{request.Scheme}://{request.Host}/");
            var uploadResult = new UploadResult();

            if (!request.HasFormContentType)
            {
                return Results.BadRequest("Expected a multipart/form-data request.");
            }

            var file = request.Form.Files[0];

            var untrustedFileName = file.FileName;
            uploadResult.FileName = untrustedFileName;
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();


            if (file.Length == 0)
            {
                logger.LogInformation("{FileName} length is 0 (Err: 1)", trustedFileNameForDisplay);
                uploadResult.ErrorCode = 1;
            }
            else if (file.Length > maxFileSize)
            {
                logger.LogInformation("{FileName} of {Length} bytes is larger than the limit of {Limit} bytes (Err: 2)",
                    trustedFileNameForDisplay, file.Length, maxFileSize);
                uploadResult.ErrorCode = 2;
            }
            else if (!file.ContentType.StartsWith("image/") || !allowedExtensions.Contains(fileExtension))
            {
                logger.LogInformation("{FileName} is not an allowed image type (Err: 4)", trustedFileNameForDisplay);
                uploadResult.ErrorCode = 4; // Custom error code for invalid file type
            }
            else
            {
                try
                {
                    // Extract the file extension from the original file name
                    fileExtension = Path.GetExtension(file.FileName);
                    var trustedFileNameForFileStorage = Path.ChangeExtension(Path.GetRandomFileName(), fileExtension);

                    //var trustedFileNameForFileStorage = Path.GetRandomFileName();
                    var path = Path.Combine(env.ContentRootPath, "Assets", "Products", "unsafe_uploads", trustedFileNameForFileStorage);

                    Directory.CreateDirectory(Path.Combine(env.ContentRootPath, "Assets", "Products", "unsafe_uploads"));

                    await using FileStream fs = new(path, FileMode.Create);
                    await file.CopyToAsync(fs);

                    logger.LogInformation("{FileName} saved at {Path}", trustedFileNameForDisplay, path);
                    uploadResult.Uploaded = true;
                    uploadResult.StoredFileName = trustedFileNameForFileStorage;
                }
                catch (IOException ex)
                {
                    logger.LogError("{FileName} error on upload (Err: 3): {Message}", trustedFileNameForDisplay, ex.Message);
                    uploadResult.ErrorCode = 3;
                }
            }


            if (uploadResult.Uploaded)
            {
                return Results.CreatedAtRoute(getFileEndpointName, new { fileName = uploadResult.StoredFileName }, uploadResult);
            }

            return Results.Created(resourcePath, uploadResult);
        });


        group.MapGet("/Assets/Products/{fileName}", (string fileName, IHostEnvironment env) =>
        {
            var filePath = Path.Combine(env.ContentRootPath, "Assets", "Products", "unsafe_uploads", fileName);
            return File.Exists(filePath) ? Results.File(filePath) : Results.NotFound();
        }).WithName(getFileEndpointName);

        return group;
    }
}
