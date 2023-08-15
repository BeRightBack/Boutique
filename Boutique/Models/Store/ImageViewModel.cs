using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boutique.Models
{
    public class ImageViewModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public string ResizedPath { get; set; }
        public string ThumbnailPath { get; set; }

    }
}