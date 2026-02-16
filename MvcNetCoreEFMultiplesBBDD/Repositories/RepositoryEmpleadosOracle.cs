
using Microsoft.EntityFrameworkCore;
using MvcNetCoreEFMultiplesBBDD.Data;
using MvcNetCoreEFMultiplesBBDD.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

#region STORED PROCEDURES
/*
 --ORACLE--
create or replace view V_EMPLEADOSDEPT
as
	select EMP.EMP_NO,EMP.APELLIDO,EMP.OFICIO,EMP.SALARIO,DEPT.DEPT_NO,DEPT.DNOMBRE,DEPT.LOC 
    from EMP inner join DEPT ON EMP.DEPT_NO = DEPT.DEPT_NO;

--Si necesitamos select dentro de un procedure,debemos devolverlo como parámetro de salida

create or replace procedure SP_ALL_VEMPLEADOS
(p_cursor_empleados out SYS_REFCURSOR)--el tipo sería cualquier cursor
AS
BEGIN
    open p_cursor_empleados for
    SELECT * from V_EMPLEADOSDEPT;   
end;
 */
#endregion
namespace MvcNetCoreEFMultiplesBBDD.Repositories
{
    public class RepositoryEmpleadosOracle : IRepositoryEmpleados
    {
        private HospitalContext context;

        public RepositoryEmpleadosOracle(HospitalContext context)
        {
            this.context = context;
        }
        public async Task<List<VistaEmpleado>> GetEmpleadosDepartamentosAsync()
        {
            //SE DEBE REALIZAR DE ESTA FORMA CON ORACLE
            //dejamos espacio y si puede ser que coincida el nombre 

            string sql = "begin " +

                " SP_ALL_VEMPLEADOS(:p_cursor_empleados); " +
                
                " end; ";

            OracleParameter pamCursor = new OracleParameter();

            //PUESTO QUE ES UN CURSOS HAY QUE DECIR EL NOMBRE Y SU TIPADO
            pamCursor.ParameterName = "p_cursor_empleados";
            pamCursor.Value = null;
            pamCursor.Direction = ParameterDirection.Output;
            //INDICAMOS EL TIPADO  OracleDbType
            pamCursor.OracleDbType = OracleDbType.RefCursor;

            var consulta = this.context.VistaEmpleados.FromSqlRaw(sql, pamCursor);

            return await consulta.ToListAsync();

        }

        public async Task<VistaEmpleado> FindEmpleadoDepartamentoPorIDAsync(int idemp)
        {
            //USAR BREARKPOINTS SI NO LLEGA O EL NOMBRE ES DISTINTO
            var consulta = from datos in this.context.VistaEmpleados
                           where datos.IdEmp == idemp
                           select datos;
            VistaEmpleado empleado = await consulta.ToAsyncEnumerable().FirstOrDefaultAsync();
            return empleado;
        }

    }
}
