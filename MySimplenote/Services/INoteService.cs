using System.Collections.Generic;
using System.Threading.Tasks;
using MySimplenote.Models;

namespace MySimplenote.Services;

public interface INoteService
{
    Task<List<Note>> GetAllNotesAsync();
    Task<Note?>      GetNoteByIdAsync(int id);
    Task<Note>       CreateNoteAsync(Note note);
    Task<Note>       UpdateNoteAsync(Note note);
    Task             DeleteNoteAsync(int  id);
    Task<List<Note>> SearchNotesAsync(string searchTerm);

    Task AddTagToNoteAsync(int noteId, int tagId);
    Task RemoveTagFromNoteAsync(int noteId, int tagId);
    Task<List<Note>> GetNotesByTagAsync(int tagId);
}