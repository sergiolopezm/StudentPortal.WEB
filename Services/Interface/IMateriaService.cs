using StudentPortal.WEB.Models.Academico;
using StudentPortal.WEB.Models;

namespace StudentPortal.WEB.Services.Interface
{
    public interface IMateriaService
    {
        Task<(bool Success, string Message, List<MateriaDto>? Data)> GetAll();
        Task<(bool Success, string Message, MateriaDto? Data)> GetById(int id);
        Task<(bool Success, string Message, PaginacionDto<MateriaDto>? Data)> GetPaginated(int page, int itemsPerPage, string? search = null);
    }
}
