using MvcNetCoreEFMultiplesBBDD.Models;

namespace MvcNetCoreEFMultiplesBBDD.Repositories
{
    public interface IRepositoryEmpleados
    {
        Task<List<VistaEmpleado>> GetEmpleadosDepartamentosAsync();
        Task<VistaEmpleado> FindEmpleadoDepartamentoPorIDAsync(int idemp);
        Task DeleteEmpleadoAsync(int idemp);
        Task InsertEmpleado(string apellido, string oficio, int dir, int salario, int comision, string nombredept);
    }
}
