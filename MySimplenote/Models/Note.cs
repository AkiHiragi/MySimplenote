using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MySimplenote.Models;

public class Note
{
    public int Id { get; set; }

    [Required] 
    [MaxLength(200)] 
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public bool IsSynced { get; set; } = false;

    public List<Tag> Tags { get; set; } = [];
}