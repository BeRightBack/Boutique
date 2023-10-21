using System.ComponentModel.DataAnnotations;

namespace Boutique.Areas.Admin.Models;

public class ImageModel
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public byte[] ImageStored { get; set; }
    public string Path { get; set; }
    public IFormFile ImageFile { get; set; }
    public int SortOrder { get; set; }
}

//public Guid Id { get; set; }
//public string FileName { get; set; }
//public string Path { get; set; }
//public string ResizedPath { get; set; }
//public string ThumbnailPath { get; set; }

//[Display(Name = "Profile Picture")]
//public byte[] ProfilePicture { get; set; }

//[Display(Name = "Profile Picture Path")]
//public string ProfilePicturePath { get; set; }

//[Display(Name = "Profile Picture")]
//public IFormFile ProfilePictureFile { get; set; }
