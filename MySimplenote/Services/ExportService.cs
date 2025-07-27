using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySimplenote.Models;

namespace MySimplenote.Services;

public class ExportService : IExportService
{
    private readonly INoteService _noteService;

    public ExportService(INoteService noteService)
    {
        _noteService = noteService;
    }

    public async Task ExportNoteToMarkdownAsync(Note note, string filePath)
    {
        var markdown = ConvertNoteToMarkdown(note);
        await File.WriteAllTextAsync(filePath, markdown, Encoding.UTF8);
    }

    public async Task ExportAllNotesToMarkdownAsync(string folderPath)
    {
        var notes = await _noteService.GetAllNotesAsync();

        foreach (var note in notes)
        {
            var fileName = SanitizeFileName(note.Title) + ".md";
            var filePath = Path.Combine(folderPath, fileName);
            await ExportNoteToMarkdownAsync(note, filePath);
        }
    }

    private string ConvertNoteToMarkdown(Note note)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"# {note.Title}");
        sb.AppendLine();

        sb.AppendLine(note.Content);
        sb.AppendLine();

        if (note.Tags.Count != 0)
        {
            sb.AppendLine("## Тэги");
            foreach (var tag in note.Tags)
            {
                sb.AppendLine($"- {tag.Name}");
            }

            sb.AppendLine();
        }

        sb.AppendLine("---");
        sb.AppendLine($"Создано: {note.CreatedAt:dd.MM.yyyy HH:mm}");
        sb.AppendLine($"Изменено: {note.UpdatedAt:dd.MM.yyyy HH:mm}");

        return sb.ToString();
    }

    private string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return invalidChars.Aggregate(fileName, (current, c) => current.Replace(c, '_'));
    }
}