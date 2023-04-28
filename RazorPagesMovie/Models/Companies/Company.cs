﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesMovie.Models;

public class Company {

    public int Id { get; set; }

    [StringLength(20, MinimumLength = 3)]
    [Required]
    public string Name { get; set; } = string.Empty;

    [StringLength(20, MinimumLength = 3)]
    [Required]
    public string Country { get; set; }
}