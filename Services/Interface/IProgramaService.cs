using StudentPortal.WEB.Models;
using StudentPortal.WEB.Models.Academico;

namespace StudentPortal.WEB.Services.Interface
{
    public interface IProgramaService
    {
        Task<(bool Success, string Message, List<ProgramaDto>? Data)> GetAll();
        Task<(bool Success, string Message, ProgramaDto? Data)> GetById(int id);
        Task<(bool Success, string Message, PaginacionDto<ProgramaDto>? Data)> GetPaginated(int page, int itemsPerPage, string? search = null);
    }
}
