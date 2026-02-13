using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MvcNetCoreEFMultiplesBBDD.Data;
using MvcNetCoreEFMultiplesBBDD.Models;

#region STORED PROCEDURES
/*
 --ORACLE--
create or replace view V_EMPLEADOSDEPT
as
	select EMP.EMP_NO,EMP.APELLIDO,EMP.OFICIO,EMP.SALARIO,DEPT.DEPT_NO,DEPT.DNOMBRE,DEPT.LOC 
    from EMP inner join DEPT ON EMP.DEPT_NO = DEPT.DEPT_NO;


 ---SqlServer--
create view V_EMPLEADOSDEPT
AS
	select EMP.EMP_NO,EMP.APELLIDO,EMP.OFICIO,EMP.SALARIO,DEPT.DEPT_NO,DEPT.DNOMBRE,DEPT.LOC from EMP inner join DEPT ON EMP.DEPT_NO = DEPT.DEPT_NO
GO
 */
#endregion

namespace MvcNetCoreEFMultiplesBBDD.Repositories
{
    public class RepositoryEmpleados
    {
        private HospitalContext context;

        public RepositoryEmpleados(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<VistaEmpleado>> GetEmpleadosDepartamentosAsync()
        {
            var consulta = from datos in this.context.VistaEmpleados
                           select datos;
            return await consulta.ToListAsync();

        }

        public async Task<VistaEmpleado> FindEmpleadoDepartamentoPorIDAsync(int idemp)
        {//USAR BREARKPOINTS SI NO LLEGA O EL NOMBRE ES DISTINTO
            var consulta = from datos in this.context.VistaEmpleados
                           where datos.IdEmp == idemp
                           select datos;
            VistaEmpleado empleado = await consulta.ToAsyncEnumerable().FirstOrDefaultAsync();
            return empleado;
        }


    }
}
