using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MvcNetCoreEFMultiplesBBDD.Data;
using MvcNetCoreEFMultiplesBBDD.Models;

#region STORED PROCEDURES
/*

 ---SqlServer--
create view V_EMPLEADOSDEPT
AS
	select EMP.EMP_NO,EMP.APELLIDO,EMP.OFICIO,EMP.SALARIO,DEPT.DEPT_NO,DEPT.DNOMBRE,DEPT.LOC from EMP inner join DEPT ON EMP.DEPT_NO = DEPT.DEPT_NO
GO

--CREAMOS UN PROCEDURE DE LO MISMO
CREATE PROCEDURE SP_ALL_VEMPLEADOS
as
	select * from V_EMPLEADOSDEPT
go

 */
#endregion

namespace MvcNetCoreEFMultiplesBBDD.Repositories
{
    public class RepositoryEmpleadosSqlServer:IRepositoryEmpleados
    {
        private HospitalContext context;

        public RepositoryEmpleadosSqlServer(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<VistaEmpleado>> GetEmpleadosDepartamentosAsync()
        {//COMO EL CÓDIOGO DE CADA BBDD ES DISTINTO Y ESTAMOS MEZCLANDO EF CON PROCEDURES Y VISTAS DEBERÍAMOS CREAR UNA INTERFAZ
            string sql = "SP_ALL_VEMPLEADOS";
            var consulta = this.context.VistaEmpleados.FromSqlRaw(sql);
            List<VistaEmpleado> data = await
                consulta.ToListAsync();

            return data;

            //var consulta = from datos in this.context.VistaEmpleados
            //               select datos;
            //return await consulta.ToListAsync();

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
