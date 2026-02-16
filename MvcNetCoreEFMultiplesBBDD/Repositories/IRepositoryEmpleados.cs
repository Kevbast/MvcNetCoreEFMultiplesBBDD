using MvcNetCoreEFMultiplesBBDD.Models;

namespace MvcNetCoreEFMultiplesBBDD.Repositories
{
    public interface IRepositoryEmpleados
    {
        Task<List<VistaEmpleado>> GetEmpleadosDepartamentosAsync();
        Task<VistaEmpleado> FindEmpleadoDepartamentoPorIDAsync(int idemp);
    }
}
