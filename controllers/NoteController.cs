using Microsoft.AspNetCore.Mvc;
using NoteBackend.DTOs;
using NoteBackend.Models;
using NoteBackend.Repositories;
using NoteBackend.Helpers;
namespace NoteBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteRepository _noteRepository;

        public NotesController(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetAllNotes([FromQuery] string? search)
        {
            try
            {
                var notes = await _noteRepository.GetAllNotesAsync(search);
                return Ok(ResponseHelper.Success(notes, "Success"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseHelper.Error($"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(int id)
        {
            try
            {
                var note = await _noteRepository.GetNoteByIdAsync(id);

                if (note == null)
                {
                    return NotFound(ResponseHelper.Error($"Note with id {id} not found"));
                }

                return Ok(ResponseHelper.Success(note, "Success"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseHelper.Error($"Internal server error: {ex.Message}"));
            }
        }


        [HttpPost]
        public async Task<ActionResult<Note>> CreateNote([FromBody] CreateNoteDto noteDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);

                    var combinedErrors = string.Join(", ", errorMessages);

                    return BadRequest(ResponseHelper.Error(combinedErrors));

                }


                var note = new Note
                {
                    Title = noteDto.Title,
                    Content = noteDto.Content ?? ""
                };

                var createdNote = await _noteRepository.CreateNoteAsync(note);
                return StatusCode(201, ResponseHelper.Success<Note>(createdNote, "Note create successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseHelper.Error($"Internal server error: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteDto noteDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);

                    var combinedErrors = string.Join(", ", errorMessages);

                    return BadRequest(ResponseHelper.Error(combinedErrors));

                }
                
                var existingNote = await _noteRepository.GetNoteByIdAsync(id);
                if (existingNote == null)
                {
                    return NotFound(ResponseHelper.Error($"Note with id {id} not found"));
                }

                var note = new Note
                {
                    Title = noteDto.Title,
                    Content = noteDto.Content ?? ""
                };

                var updatedNote = await _noteRepository.UpdateNoteAsync(id, note);


                return Ok(ResponseHelper.Success<Note>(updatedNote, "Note updated successfully"));


            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseHelper.Error($"Internal server error: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            try
            {
                var existingNote = await _noteRepository.GetNoteByIdAsync(id);
                if (existingNote == null)
                {
                    return NotFound(ResponseHelper.Error($"Note with id {id} not found"));
                }

                var deleted = await _noteRepository.DeleteNoteAsync(id);

                if (!deleted)

                {
                    return StatusCode(500, ResponseHelper.Error("Failed to delete note"));

                }

                return Ok(ResponseHelper.Success<object>(message: "Note deleted successfully"));

            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseHelper.Error($"Internal server error: {ex.Message}"));
            }
        }
    }
}