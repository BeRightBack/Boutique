﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Boutique.Entity;

public class Manufacturer
{
    public Guid Id { get; set; }

    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; }
    public string Description { get; set; }
    public string MainImage { get; set; }

    public string SeoUrl { get; set; }
    public string MetaTitle { get; set; }
    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }

    public bool Published { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime DateModified { get; set; }
}
