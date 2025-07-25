using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySimplenote.Data;
using MySimplenote.Models;

namespace MySimplenote.Services;

public class NoteService : INoteService
{
    public async Task<List<Note>> GetAllNotesAsync()
    {
        using var context = new NoteDbContext();
        await context.Database.EnsureCreatedAsync();
        return await context.Notes
                            .Include(n => n.Tags)
                            .OrderByDescending(n => n.UpdatedAt)
                            .ToListAsync();
    }

    public async Task<Note?> GetNoteByIdAsync(int id)
    {
        using var context = new NoteDbContext();
        return await context.Notes
                            .Include(n => n.Tags)
                            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<Note> CreateNoteAsync(Note note)
    {
        using var context = new NoteDbContext();
        await context.Database.EnsureCreatedAsync();
        context.Notes.Add(note);
        await context.SaveChangesAsync();
        return note;
    }

    public async Task<Note> UpdateNoteAsync(Note note)
    {
        using var context = new NoteDbContext();
        note.UpdatedAt = DateTime.Now;
        context.Notes.Update(note);
        await context.SaveChangesAsync();
        return note;
    }

    public async Task DeleteNoteAsync(int id)
    {
        using var context = new NoteDbContext();
        var       note    = await context.Notes.FindAsync(id);
        if (note != null)
        {
            context.Notes.Remove(note);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<Note>> SearchNotesAsync(string searchTerm)
    {
        using var context = new NoteDbContext();
        return await context.Notes
                            .Where(n => n.Title.Contains(searchTerm) || n.Content.Contains(searchTerm))
                            .OrderByDescending(n => n.UpdatedAt)
                            .ToListAsync();
    }

    public async Task AddTagToNodeAsync(int noteId, int tagId)
    {
        using var context = new NoteDbContext();
        var       note    = await context.Notes.Include(n => n.Tags).FirstOrDefaultAsync(n => n.Id == noteId);
        var       tag     = await context.Tags.FindAsync(tagId);

        if (note != null && tag != null && !note.Tags.Any(t => t.Id == tagId))
        {
            note.Tags.Add(tag);
            note.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync();
        }
    }

    public async Task RemoveTagFromNoteAsync(int noteId, int tagId)
    {
        using var context = new NoteDbContext();
        var       note    = await context.Notes.Include(n => n.Tags).FirstOrDefaultAsync(n => n.Id == noteId);

        if (note != null)
        {
            var tag = note.Tags.FirstOrDefault(t => t.Id == tagId);
            if (tag != null)
            {
                note.Tags.Remove(tag);
                note.UpdatedAt = DateTime.Now;
                await context.SaveChangesAsync();
            }
        }
    }

    public async Task<List<Note>> GetNotesByTagAsync(int tagId)
    {
        using var context = new NoteDbContext();
        return await context.Notes
                            .Where(n => n.Tags.Any(t => t.Id == tagId))
                            .OrderByDescending(n => n.UpdatedAt)
                            .ToListAsync();
    }
}