using System.ComponentModel.DataAnnotations;

namespace NoteBackend.DTOs
{
    public class CreateNoteDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        public string? Content { get; set; }
    }

    public class UpdateNoteDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        public string? Content { get; set; }
    }
}