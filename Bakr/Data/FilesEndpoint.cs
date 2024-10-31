using System.Net;
using System.Security.Claims;
using Bakr.Shared.Models;

namespace Bakr.Data;

public static class FilesEndpoint
{
    const string getFileEndpointName = "GetFile";
    private static readonly string[] handler = [".jpg", ".jpeg", ".png", ".gif"];

    public static RouteGroupBuilder MapFileEndpoint(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("api/filesave").RequireAuthorization();


        //     group.MapPost("/", async (HttpRequest request, IHostEnvironment env, ILogger<Program> logger) =>
        //     {
        //         const int maxAllowedFiles = 1;
        //         const long maxFileSize = 1024 * 1024 * 1;
        //         var filesProcessed = 0;
        //         var uploadResults = new List<UploadResult>();

        //         if (!request.HasFormContentType)
        //         {
        //             return Results.BadRequest("Expected a multipart/form-data request.");
        //         }

        //         var files = request.Form.Files;

        //         foreach (var file in files)
        //         {
        //             var uploadResult = new UploadResult();
        //             var untrustedFileName = file.FileName;
        //             uploadResult.FileName = untrustedFileName;
        //             var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

        //             if (filesProcessed < maxAllowedFiles)
        //             {
        //                 if (file.Length == 0)
        //                 {
        //                     logger.LogInformation("{FileName} length is 0 (Err: 1)", trustedFileNameForDisplay);
        //                     uploadResult.ErrorCode = 1;
        //                 }
        //                 else if (file.Length > maxFileSize)
        //                 {
        //                     logger.LogInformation("{FileName} of {Length} bytes is larger than the limit of {Limit} bytes (Err: 2)",
        //                         trustedFileNameForDisplay, file.Length, maxFileSize);
        //                     uploadResult.ErrorCode = 2;
        //                 }
        //                 else
        //                 {
        //                     try
        //                     {
        //                         // Extract the file extension from the original file name
        //                         var fileExtension = Path.GetExtension(file.FileName);
        //                         var trustedFileNameForFileStorage = Path.ChangeExtension(Path.GetRandomFileName(), fileExtension);

        //                         //var trustedFileNameForFileStorage = Path.GetRandomFileName();
        //                         var path = Path.Combine(env.ContentRootPath, "Assets", "Products", "unsafe_uploads", trustedFileNameForFileStorage);

        //                         Directory.CreateDirectory(Path.Combine(env.ContentRootPath, "Assets", "Products", "unsafe_uploads"));

        //                         await using FileStream fs = new(path, FileMode.Create);
        //                         await file.CopyToAsync(fs);

        //                         logger.LogInformation("{FileName} saved at {Path}", trustedFileNameForDisplay, path);
        //                         uploadResult.Uploaded = true;
        //                         uploadResult.StoredFileName = trustedFileNameForFileStorage;
        //                     }
        //                     catch (IOException ex)
        //                     {
        //                         logger.LogError("{FileName} error on upload (Err: 3): {Message}", trustedFileNameForDisplay, ex.Message);
        //                         uploadResult.ErrorCode = 3;
        //                     }
        //                 }

        //                 filesProcessed++;
        //             }
        //             else
        //             {
        //                 logger.LogInformation("{FileName} not uploaded because the request exceeded the allowed {Count} of files (Err: 4)",
        //                     trustedFileNameForDisplay, maxAllowedFiles);
        //                 uploadResult.ErrorCode = 4;
        //             }

        //             uploadResults.Add(uploadResult);
        //         }

        //         return Results.CreatedAtRoute(getFileEndpointName, new { fileName = uploadResults[0].StoredFileName }, uploadResults);
        //     });
        group.MapPost("/", async (HttpRequest request, IHostEnvironment env, ILogger<Program> logger) =>
        {
            const int maxAllowedFiles = 1;
            const long maxFileSize = 1024 * 1024 * 1;
            var allowedExtensions = handler;
            var filesProcessed = 0;
            var resourcePath = new Uri($"{request.Scheme}://{request.Host}/");
            var uploadResults = new List<UploadResult>();

            if (!request.HasFormContentType)
            {
                return Results.BadRequest("Expected a multipart/form-data request.");
            }

            var files = request.Form.Files;

            foreach (var file in files)
            {
                var uploadResult = new UploadResult() {
                    StoredFileName = ""
                };
                var untrustedFileName = file.FileName;
                uploadResult.FileName = untrustedFileName;
                var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();


                if (filesProcessed < maxAllowedFiles)
                {
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
                        logger.LogInformation("{FileName} is not an allowed image type (Err: 5)", trustedFileNameForDisplay);
                        uploadResult.ErrorCode = 5; // Custom error code for invalid file type
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

                    filesProcessed++;
                }
                else
                {
                    logger.LogInformation("{FileName} not uploaded because the request exceeded the allowed {Count} of files (Err: 4)",
                        trustedFileNameForDisplay, maxAllowedFiles);
                    uploadResult.ErrorCode = 4;
                }

                uploadResults.Add(uploadResult);
            }
            foreach (UploadResult uploadResult in uploadResults)
            {
                if (uploadResult.Uploaded)
                {
                    return Results.CreatedAtRoute(getFileEndpointName, new { fileName = uploadResult.StoredFileName }, uploadResults);
                }
            }

            return Results.Created(resourcePath, uploadResults);
        });


        group.MapGet("/Assets/Products/{fileName}", (string fileName) =>
        {
            return fileName;
        }).WithName(getFileEndpointName);

        return group;
    }
}
