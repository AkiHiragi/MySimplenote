using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySimplenote.Models;
using MySimplenote.Services;

namespace MySimplenote.ViewModels;

public partial class MainViewModel:ObservableObject
{
    private readonly INoteService _noteService;
    private readonly ITagService  _tagService;

    [ObservableProperty] private ObservableCollection<NoteViewModel> notes = [];

    [ObservableProperty] private NoteViewModel? selectedNote;

    [ObservableProperty] private string searchText = string.Empty;

    [ObservableProperty] private ObservableCollection<TagViewModel> allTags = [];

    [ObservableProperty] private TagViewModel? selectedTag;

    [ObservableProperty] private string newTagName = string.Empty;

    [ObservableProperty] private bool isTagFilterActive = false;

    public MainViewModel()
    {
        _noteService = new NoteService();
        _tagService  = new TagService();
        _            = LoadNotesAsync();
        _            = LoadTagsAsync();
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
        var newNote     = new Note { Title = "Новая заметка" };
        var createdNote = await _noteService.CreateNoteAsync(newNote);
        var noteViewModel = new NoteViewModel(createdNote);
        Notes.Insert(0,noteViewModel);
        SelectedNote = noteViewModel;
    }

    [RelayCommand]
    private async Task SaveNoteAsync()
    {
        if(SelectedNote==null) return;

        var noteModel = SelectedNote.ToModel();
        await _noteService.UpdateNoteAsync(noteModel);
        await LoadNotesAsync();
    }

    [RelayCommand]
    private async Task DeleteNoteAsync()
    {
        if (SelectedNote==null)return;

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
    
        var existingTag = await _tagService.GetTagByNameAsync(NewTagName);
        if (existingTag != null) return;
    
        var newTag     = new Tag { Name = NewTagName.Trim() };
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
}