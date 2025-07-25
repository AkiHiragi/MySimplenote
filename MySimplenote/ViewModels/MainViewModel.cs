using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySimplenote.Models;
using MySimplenote.Services;

namespace MySimplenote.ViewModels;

public partial class MainViewModel:ObservableObject
{
    private readonly INoteService _noteService;

    [ObservableProperty] private ObservableCollection<NoteViewModel> notes = [];

    [ObservableProperty] private NoteViewModel? selectedNote;

    [ObservableProperty] private string searchText = string.Empty;

    public MainViewModel()
    {
        _noteService = new NoteService();
        _            = LoadNotesAsync();
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
}