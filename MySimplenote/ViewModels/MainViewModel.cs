using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySimplenote.Models;
using MySimplenote.Services;

namespace MySimplenote.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly INoteService  _noteService;
    private readonly ITagService   _tagService;
    private readonly IThemeService _themeService;

    [ObservableProperty] private ObservableCollection<NoteViewModel> notes = [];

    [ObservableProperty] private NoteViewModel? selectedNote;

    [ObservableProperty] private string searchText = string.Empty;

    [ObservableProperty] private ObservableCollection<TagViewModel> allTags = [];

    [ObservableProperty] private TagViewModel? selectedTag;

    [ObservableProperty] private string newTagName = string.Empty;

    [ObservableProperty] private bool isTagFilterActive = false;

    [ObservableProperty] private string currentTheme = "Light";

    public MainViewModel()
    {
        _noteService  = new NoteService();
        _tagService   = new TagService();
        _themeService = new ThemeService();

        CurrentTheme = _themeService.CurrentTheme;

        _ = LoadNotesAsync();
        _ = LoadTagsAsync();
    }

    [RelayCommand]
    private async Task LoadNotesAsync()
    {
        var noteModels = await _noteService.GetAllNotesAsync();
        Notes.Clear();
        foreach (var note in noteModels)
        {
            Notes.Add(new NoteViewModel(note));
        }
    }

    [RelayCommand]
    private async Task CreateNoteAsync()
    {
        var newNote       = new Note { Title = "Новая заметка" };
        var createdNote   = await _noteService.CreateNoteAsync(newNote);
        var noteViewModel = new NoteViewModel(createdNote);
        Notes.Insert(0, noteViewModel);
        SelectedNote = noteViewModel;
    }

    [RelayCommand]
    private async Task SaveNoteAsync()
    {
        if (SelectedNote == null) return;

        var noteModel = SelectedNote.ToModel();
        await _noteService.UpdateNoteAsync(noteModel);
        await LoadNotesAsync();
    }

    [RelayCommand]
    private async Task DeleteNoteAsync()
    {
        if (SelectedNote == null) return;

        await _noteService.DeleteNoteAsync(SelectedNote.Id);
        Notes.Remove(SelectedNote);
        SelectedNote = null;
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await LoadNotesAsync();
            return;
        }

        var searchResult = await _noteService.SearchNotesAsync(SearchText);
        Notes.Clear();
        foreach (var note in searchResult)
        {
            Notes.Add(new NoteViewModel(note));
        }
    }

    [RelayCommand]
    private async Task LoadTagsAsync()
    {
        var tags = await _tagService.GetAllTagsAsync();
        AllTags.Clear();
        foreach (var tag in tags)
        {
            AllTags.Add(new TagViewModel(tag));
        }
    }

    [RelayCommand]
    private async Task CreateTagAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTagName)) return;

        var trimmedName = newTagName.Trim();

        var existingTag = await _tagService.GetTagByNameAsync(trimmedName);
        if (existingTag != null)
        {
            NewTagName = string.Empty;
            return;
        }

        var colors = new[] { "#2196F3", "#4CAF50", "#FF9800", "#9C27B0", "#F44336", "#607D8B", "#795548", "#009688" };
        var randomColor = colors[Random.Shared.Next(colors.Length)];

        var newTag     = new Tag { Name = trimmedName, Color = randomColor };
        var createdTag = await _tagService.CreateTagAsync(newTag);
        AllTags.Add(new TagViewModel(createdTag));
        NewTagName = string.Empty;
    }

    [RelayCommand]
    private async Task AddTagToNoteAsync(TagViewModel tag)
    {
        if (SelectedNote == null || tag == null) return;

        if (!SelectedNote.Tags.Any(t => t.Id == tag.Id))
        {
            await _noteService.AddTagToNoteAsync(SelectedNote.Id, tag.Id);
            SelectedNote.Tags.Add(tag);
        }
    }

    [RelayCommand]
    private async Task RemoveTagFromNoteAsync(TagViewModel tag)
    {
        if (SelectedNote == null || tag == null) return;

        await _noteService.RemoveTagFromNoteAsync(SelectedNote.Id, tag.Id);
        var tagToRemove = SelectedNote.Tags.FirstOrDefault(t => t.Id == tag.Id);
        if (tagToRemove != null)
        {
            SelectedNote.Tags.Remove(tagToRemove);
        }
    }

    [RelayCommand]
    private async Task FilterByTagAsync(TagViewModel? tag)
    {
        if (tag == null)
        {
            IsTagFilterActive = false;
            await LoadNotesAsync();
            return;
        }

        IsTagFilterActive = true;
        SelectedTag       = tag;
        var filteredNotes = await _noteService.GetNotesByTagAsync(tag.Id);
        Notes.Clear();
        foreach (var note in filteredNotes)
        {
            Notes.Add(new NoteViewModel(note));
        }
    }

    [RelayCommand]
    private async Task ClearTagFilterAsync()
    {
        IsTagFilterActive = false;
        SelectedTag       = null;
        await LoadNotesAsync();
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        var newTheme = CurrentTheme == "Light" ? "Dark" : "Light";
        _themeService.SetTheme(newTheme);
        CurrentTheme = newTheme;
    }

    [RelayCommand]
    private async Task EditTagAsync(TagViewModel tag)
    {
        if (tag == null) return;

        var updatedTag = tag.ToModel();
        await _tagService.UpdateTagAsync(updatedTag);
        await LoadTagsAsync();
    }

    [RelayCommand]
    private async Task DeleteTagAsync(TagViewModel tag)
    {
        if (tag == null) return;

        var notesWithTag = Notes.Where(n => n.Tags.Any(t => t.Id == tag.Id)).ToList();

        if (notesWithTag.Any())
        {
            // TODO: добавить MessageBox с удалением
        }

        await _tagService.DeleteTagAsync(tag.Id);
        AllTags.Remove(tag);

        foreach (var note in Notes)
        {
            var tagToRemove = note.Tags.FirstOrDefault(t => t.Id == tag.Id);
            if (tagToRemove != null)
                note.Tags.Remove(tagToRemove);
        }
    }

    [RelayCommand]
    private async Task UpdateTagNameAsync(TagViewModel tag)
    {
        if (tag == null || string.IsNullOrWhiteSpace(tag.Name)) return;

        var existingTag =
            AllTags.FirstOrDefault(t => t.Id != tag.Id &&
                                        t.Name.Equals(tag.Name.Trim(), StringComparison.OrdinalIgnoreCase));
        if (existingTag != null)
        {
            await LoadTagsAsync();
            return;
        }

        tag.Name = tag.Name.Trim();
        var updatedTag = tag.ToModel();
        await _tagService.UpdateTagAsync(updatedTag);

        foreach (var note in Notes)
        {
            var noteTag = note.Tags.FirstOrDefault(t => t.Id == tag.Id);
            if (noteTag != null)
                noteTag.Name = tag.Name;
        }
    }

    [RelayCommand]
    private async Task ChangeTagColorAsync(TagViewModel tag)
    {
        if (tag == null) return;

        var colors = new[] { "#2196F3", "#4CAF50", "#FF9800", "#9C27B0", "#F44336", "#607D8B", "#795548", "#009688" };
        var currentIndex = Array.IndexOf(colors, tag.Color);
        var nextIndex = (currentIndex + 1) % colors.Length;

        tag.Color = colors[nextIndex];

        var updatedTag = tag.ToModel();
        await _tagService.UpdateTagAsync(updatedTag);

        foreach (var note in Notes)
        {
            var noteTag = note.Tags.FirstOrDefault(t => t.Id == tag.Id);
            if (noteTag != null)
                noteTag.Color = tag.Color;
        }
    }
}