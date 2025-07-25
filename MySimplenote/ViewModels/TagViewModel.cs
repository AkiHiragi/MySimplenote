using System;
using CommunityToolkit.Mvvm.ComponentModel;
using MySimplenote.Models;

namespace MySimplenote.ViewModels;

public partial class TagViewModel : ObservableObject
{
    [ObservableProperty] private int id;

    [ObservableProperty] private string name = string.Empty;

    [ObservableProperty] private string color = "#2196F3";

    [ObservableProperty] private DateTime createdAt;

    public TagViewModel()
    {
    }

    public TagViewModel(Tag tag)
    {
        Id        = tag.Id;
        Name      = tag.Name;
        Color     = tag.Color;
        CreatedAt = tag.CreatedAt;
    }

    public Tag ToModel()
    {
        return new Tag
        {
            Id        = Id,
            Name      = Name,
            Color     = Color,
            CreatedAt = CreatedAt
        };
    }
}