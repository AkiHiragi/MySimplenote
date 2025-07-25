using System.Collections.Generic;
using System.Threading.Tasks;
using MySimplenote.Models;

namespace MySimplenote.Services;

public interface ITagService
{
    Task<List<Tag>> GetAllTagsAsync();
    Task<Tag?>      GetTagByIdAsync(int      id);
    Task<Tag>       CreateTagAsync(Tag       tag);
    Task<Tag>       UpdateTagAsync(Tag       tag);
    Task            DeleteTagAsync(int       id);
    Task<Tag?>      GetTagByNameAsync(string name);
    Task<List<Tag>> GetTagsForNoteAsync(int  noteId);
}