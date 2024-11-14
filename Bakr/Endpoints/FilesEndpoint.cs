using System.Net;
using Bakr.Shared.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bakr.Endpoints;

public static class FilesEndpoint
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    public static RouteGroupBuilder MapFileEndpoint(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api/fileSave").RequireAuthorization();

        group.MapPost("/", UploadImageAsync);

        group.MapGet("/Assets/Products/{fileName}", GetFile);

        return group;
    }
    
    private static Results<FileStreamHttpResult, NotFound> GetFile(string fileName, IHostEnvironment env)
    {
        var filePath = Path.Combine(env.ContentRootPath, "wwwroot", "Assets", "Products", "unsafe_uploads", fileName);
        if (File.Exists(filePath))
        {
            return TypedResults.File(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None));
        }
        else return TypedResults.NotFound();
    }
    
    private static async Task<Results<BadRequest<string>, Created<UploadResult>>> UploadImageAsync(HttpRequest request, IHostEnvironment env, ILogger<Program> logger)
    {
        const long maxFileSize = 1024 * 1024 * 1;
        var resourcePath = new Uri($"{request.Scheme}://{request.Host}/");
        var uploadResult = new UploadResult();

        if (!request.HasFormContentType)
        {
            return TypedResults.BadRequest("Expected a multipart/form-data request.");
        }

        var file = request.Form.Files[0];

        var untrustedFileName = file.FileName;
        uploadResult.FileName = untrustedFileName;
        var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        switch (file.Length)
        {
            case 0:
                logger.LogInformation("{FileName} length is 0 (Err: 1)", trustedFileNameForDisplay);
                uploadResult.ErrorCode = 1;
                break;
            case > maxFileSize:
                logger.LogInformation("{FileName} of {Length} bytes is larger than the limit of {Limit} bytes (Err: 2)",
                    trustedFileNameForDisplay, file.Length, maxFileSize);
                uploadResult.ErrorCode = 2;
                break;
            default:
            {
                if (!file.ContentType.StartsWith("image/") || !AllowedExtensions.Contains(fileExtension))
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
                        var path = Path.Combine(env.ContentRootPath, "wwwroot", "Assets", "Products", "unsafe_uploads", trustedFileNameForFileStorage);

                        Directory.CreateDirectory(Path.Combine(env.ContentRootPath, "wwwroot", "Assets", "Products", "unsafe_uploads"));

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

                break;
            }
        }

        if (uploadResult.Uploaded)
        {
            resourcePath = new Uri($"{resourcePath.AbsoluteUri}api/filesave/Assets/Products/{uploadResult.StoredFileName}");
        }
        
        return TypedResults.Created(resourcePath, uploadResult);
    }
}
