using StudentPortal.WEB.Models.Academico;
using StudentPortal.WEB.Models;

namespace StudentPortal.WEB.Services.Interface
{
    public interface IInscripcionService
    {
        Task<(bool Success, string Message, List<InscripcionDto>? Data)> GetByEstudiante(int estudianteId);
        Task<(bool Success, string Message, List<InscripcionDto>? Data)> GetByMateria(int materiaId);
        Task<(bool Success, string Message, InscripcionDto? Data)> GetById(int id);
        Task<(bool Success, string Message, PaginacionDto<InscripcionDto>? Data)> GetPaginated(int page, int itemsPerPage, int? estudianteId = null, int? materiaId = null);
        Task<(bool Success, string Message, InscripcionDto? Data)> Inscribir(InscripcionDto inscripcion);
        Task<(bool Success, string Message, InscripcionDto? Data)> Calificar(int id, decimal calificacion);
        Task<(bool Success, string Message, InscripcionDto? Data)> Cancelar(int id);
    }
}
