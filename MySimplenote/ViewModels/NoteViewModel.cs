using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MySimplenote.Models;

namespace MySimplenote.ViewModels;

public partial class NoteViewModel : ObservableObject
{
    [ObservableProperty] private int id;

    [ObservableProperty] private string title = string.Empty;

    [ObservableProperty] private string content = string.Empty;

    [ObservableProperty] private DateTime createdAt;

    [ObservableProperty] private DateTime updatedAt;

    [ObservableProperty] private bool isSynced;

    [ObservableProperty] private ObservableCollection<TagViewModel> tags = [];

    public NoteViewModel()
    {
    }

    public NoteViewModel(Note note)
    {
        Id        = note.Id;
        Title     = note.Title;
        Content   = note.Content;
        CreatedAt = note.CreatedAt;
        UpdatedAt = note.UpdatedAt;
        IsSynced  = note.IsSynced;

        Tags.Clear();
        foreach (var tag in note.Tags)
        {
            Tags.Add(new TagViewModel(tag));
        }
    }

    public Note ToModel()
    {
        var note = new Note
        {
            Id        = Id,
            Title     = Title,
            Content   = Content,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            IsSynced  = IsSynced
        };

        foreach (var tagViewModel in Tags)
        {
            note.Tags.Add(tagViewModel.ToModel());
        }

        return note;
    }
}