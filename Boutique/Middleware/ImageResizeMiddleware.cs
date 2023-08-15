
using Boutique.Entity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.FileProviders;

namespace Boutique.Middleware;

// public class ImageResizeMiddleware
// {
//     private readonly RequestDelegate _next;
//     private readonly IFileProvider _fileProvider;

//     public ImageResizeMiddleware(RequestDelegate next, IFileProvider fileProvider)
//     {
//         _next = next;
//         _fileProvider = fileProvider;
//     }  

//     public async Task Invoke(HttpContext context)
//     {
//         var imageExtensions = new List<string> { "jpg", "png" };
//         bool isImage = imageExtensions.Any(ext => context.Request.Path.ToString().ToLower().EndsWith(ext));

//         // check if the request is a valid image
//         if (isImage)
//         {
//             var imagePath = $"wwwroot/{Uri.UnescapeDataString(context.Request.Path.ToString())}";

//             // check if the image request has a query string,
//             // otherwise, return to the next middleware
//             if (context.Request.QueryString.HasValue)
//             {
//                 int w = 0, h = 0;

//                 var imageQueryString = QueryHelpers.ParseQuery(context.Request.QueryString.ToString());

//                 if (imageQueryString.TryGetValue("w", out var value))
//                     w = Convert.ToInt32(value);

//                 if (imageQueryString.TryGetValue("h", out var value2))
//                     h = Convert.ToInt32(value2);

//                 // if either width or height is present in the query string,
//                 // resize the image
//                 if (w > 0 || h > 0)
//                 {
//                     using var input = File.OpenRead(imagePath);
//                     var image = Image.Load(input);

//                     w = w == 0 ? image.Width : w;
//                     h = h == 0 ? image.Height : h;

//                     image.Mutate(x => x.Resize(new ResizeOptions
//                     {
//                         Size = new Size(w, h),
//                         Mode = ResizeMode.Min
//                     }));

//                     using var ms = new MemoryStream();
//                     image.Save(ms, PngFormat.Instance);
//                     await context.Response.Body.WriteAsync(ms.ToArray().AsMemory(0, ms.ToArray().Length));
//                     return;
//                 }
//             }
//         }

//         await _next(context);
//     }
// }

// public static class ImageResizeMiddlewareExtensions
// {
//     public static IApplicationBuilder UseImageResize(this IApplicationBuilder builder)
//     {
//         return builder.UseMiddleware<ImageResizeMiddleware>();
//     }
// }
