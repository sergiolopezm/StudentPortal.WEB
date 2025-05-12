using StudentPortal.WEB.Models.Academico;
using StudentPortal.WEB.Models;

namespace StudentPortal.WEB.Services.Interface
{
    public interface IEstudianteService
    {
        Task<(bool Success, string Message, List<EstudianteDto>? Data)> GetAll();
        Task<(bool Success, string Message, EstudianteDto? Data)> GetById(int id);
        Task<(bool Success, string Message, PaginacionDto<EstudianteDto>? Data)> GetPaginated(int page, int itemsPerPage, string? search = null);
        Task<(bool Success, string Message, EstudianteDto? Data)> Create(EstudianteDto estudiante);
        Task<(bool Success, string Message, EstudianteDto? Data)> Update(int id, EstudianteDto estudiante);
        Task<(bool Success, string Message)> Delete(int id);
        Task<(bool Success, string Message, List<CompaneroClaseDto>? Data)> GetCompaneros(int id);
    }
}
