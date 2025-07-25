using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MySimplenote.Models;

public class Tag
{
    public int Id { get; set; }

    [Required] [MaxLength(50)] public string Name { get; set; } = string.Empty;

    public string Color { get; set; } = "#2196F3";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<Note> Notes { get; set; } = [];
}