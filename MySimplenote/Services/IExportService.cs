using System.Threading.Tasks;
using MySimplenote.Models;

namespace MySimplenote.Services;

public interface IExportService
{
    Task ExportNoteToMarkdownAsync(Note       note, string filePath);
    Task ExportAllNotesToMarkdownAsync(string folderPath);
}