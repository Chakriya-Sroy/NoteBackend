using Dapper;
using NoteBackend.Data;
using NoteBackend.Models;

namespace NoteBackend.Repositories
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetAllNotesAsync();
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

        public async Task<IEnumerable<Note>> GetAllNotesAsync()
        {
            var query = "SELECT * FROM Notes ORDER BY CreatedAt DESC";
            // create connection 
            using var connection = _context.CreateConnection();
            // query
            var notes = await connection.QueryAsync<Note>(query);
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
                VALUES (@Title, @Content, GETDATE(), GETDATE());
                
                SELECT * FROM Notes WHERE Id = SCOPE_IDENTITY();";

            using var connection = _context.CreateConnection();
            var createdNote = await connection.QuerySingleAsync<Note>(query, note);
            return createdNote;
        }

        public async Task<Note?> UpdateNoteAsync(int id, Note note)
        {
            var query = @"
                UPDATE Notes 
                SET Title = @Title, 
                    Content = @Content, 
                    UpdatedAt = GETDATE()
                WHERE Id = @Id;
                SELECT * FROM Notes WHERE Id = @Id;";

            using var connection = _context.CreateConnection();
            var updatedNote = await connection.QuerySingleOrDefaultAsync<Note>(query, new
            {
                Id = id,
                note.Title,
                note.Content
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