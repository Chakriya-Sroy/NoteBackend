using Dapper;
using NoteBackend.Data;
using NoteBackend.Models;

namespace NoteBackend.Repositories
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetAllNotesAsync(string? search);
        Task<Note?> GetNoteByIdAsync(int id);
        Task<Note> CreateNoteAsync(Note note);
        Task<Note?> UpdateNoteAsync(int id, Note note);
        Task<bool> DeleteNoteAsync(int id);

    }

    public class NoteRepository : INoteRepository
    {
        private readonly DapperContext _context;

        public NoteRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Note>> GetAllNotesAsync(string? search)
        {
            var query = "SELECT * FROM Notes";

            if (!string.IsNullOrWhiteSpace(search))
            {
                query += " WHERE Title LIKE @search OR Content LIKE @search";
            }

            query += " ORDER BY CreatedAt DESC";

            using var connection = _context.CreateConnection();
            var notes = await connection.QueryAsync<Note>(query, new { search = $"%{search}%" });

            return notes;
        }
        public async Task<Note?> GetNoteByIdAsync(int id)
        {
            var query = "SELECT * FROM Notes WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            var note = await connection.QuerySingleOrDefaultAsync<Note>(query, new { Id = id });
            return note;
        }

        public async Task<Note> CreateNoteAsync(Note note)
        {
            var query = @"
                INSERT INTO Notes (Title, Content, CreatedAt, UpdatedAt)
                VALUES (@Title, @Content, @CreatedAt, @UpdatedAt);
                
                SELECT * FROM Notes WHERE Id = SCOPE_IDENTITY();";


            var seAsiaTime = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
            );

            using var connection = _context.CreateConnection();
            var createdNote = await connection.QuerySingleAsync<Note>(query, new
            {
                note.Title,
                note.Content,
                CreatedAt = seAsiaTime,
                UpdatedAt = seAsiaTime
            });
            return createdNote;
        }

        public async Task<Note?> UpdateNoteAsync(int id, Note note)
        {
            var query = @"
                UPDATE Notes 
                SET Title = @Title, 
                    Content = @Content, 
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id;
                SELECT * FROM Notes WHERE Id = @Id;";

           var seAsiaTime = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
            );

            using var connection = _context.CreateConnection();

            var updatedNote = await connection.QuerySingleOrDefaultAsync<Note>(query, new
            {
                Id = id,
                note.Title,
                note.Content,
                UpdatedAt = seAsiaTime,
            });

            return updatedNote;
        }

        public async Task<bool> DeleteNoteAsync(int id)
        {
            var query = "DELETE FROM Notes WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, new { Id = id });

            return affectedRows > 0;
        }
    }
}