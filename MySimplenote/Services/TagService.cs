using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySimplenote.Data;
using MySimplenote.Models;

namespace MySimplenote.Services;

public class TagService : ITagService
{
    public async Task<List<Tag>> GetAllTagsAsync()
    {
        using var context = new NoteDbContext();
        await context.Database.EnsureCreatedAsync();
        return await context.Tags.OrderBy(t => t.Name).ToListAsync();
    }
    
    public async Task<Tag?> GetTagByIdAsync(int id)
    {
        using var context = new NoteDbContext();
        return await context.Tags.FindAsync(id);
    }

    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        using var context = new NoteDbContext();
        await context.Database.EnsureCreatedAsync();
        context.Tags.Add(tag);
        await context.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag> UpdateTagAsync(Tag tag)
    {
        using var context = new NoteDbContext();
        context.Tags.Update(tag);
        await context.SaveChangesAsync();
        return tag;
    }

    public async Task DeleteTagAsync(int id)
    {
        using var context = new NoteDbContext();
        var       tag     = await context.Tags.FindAsync(id);
        if (tag != null)
        {
            context.Tags.Remove(tag);
            await context.SaveChangesAsync();
        }
    }

    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        using var context = new NoteDbContext();
        return await context.Tags.FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<List<Tag>> GetTagsForNoteAsync(int noteId)
    {
        using var context = new NoteDbContext();
        return await context.Notes
                            .Where(n => n.Id == noteId)
                            .SelectMany(n => n.Tags)
                            .ToListAsync();
    }
}