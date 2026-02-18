using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MvcNetCoreEFMultiplesBBDD.Data;
using MvcNetCoreEFMultiplesBBDD.Models;
using System.Data;
using System.Data.Common;

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

--PROCEDURE PARA INSERTAR EL EMPLEADO

create OR ALTER procedure SP_INSERT_EMPLEADO
    (@apellido nvarchar(50),@oficio nvarchar(50),@dir int,@salario int,@comision int, @nombredept nvarchar(50) )
        AS
            declare @empno int
            select @empno= MAX(EMP_NO) FROM EMP 
            select @empno = @empno+1;
            print @empno

            declare @fecha_alt dateTime
            select @fecha_alt = CURRENT_DATE;
            print @fecha_alt

            declare @deptno int
            select @deptno= DEPT_NO FROM DEPT WHERE DNOMBRE =@nombredept;
            print @deptno

            insert into EMP values(@empno,@apellido,@oficio,@dir,@fecha_alt,@salario,@comision,@deptno)
            
        GO
----PARA DEVOLVER EMP OUT
(@apellido nvarchar(50),@oficio nvarchar(50),@dir int,@salario int,@comision int, @nombredept nvarchar(50), @empno int out )
        AS 
            select @empno= MAX(EMP_NO)+1 FROM EMP 
            print @empno

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

        public async Task InsertEmpleado(string apellido, string oficio, int dir, int salario, int comision, string nombredept)
        {
            string sql = "SP_INSERT_EMPLEADO";//en este caso no haría falta poner la variable ya que usamos DbCommand
            SqlParameter pamApell = new SqlParameter("@apellido", apellido);
            SqlParameter pamOfi = new SqlParameter("@oficio", oficio);
            SqlParameter pamDir = new SqlParameter("@dir", dir);
            SqlParameter pamSalario = new SqlParameter("@salario", salario);
            SqlParameter pamComi = new SqlParameter("@comision", comision);
            SqlParameter pamDept = new SqlParameter("@nombredept", nombredept);

            //(Mirar Referencia DeleteRaw)Se podría hacer con sqlraw(no usaríamos using) pero habría que añadir en sql las variables y 
            // await this.context.Database.ExecuteSqlRawAsync(sql, pamName,pamDir,pamFech,pamGen,pamNss);

            using (DbCommand com =
               this.context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = sql;
                com.Parameters.Add(pamApell);
                com.Parameters.Add(pamOfi);
                com.Parameters.Add(pamDir);
                com.Parameters.Add(pamSalario);
                com.Parameters.Add(pamComi);
                com.Parameters.Add(pamDept);

                await com.Connection.OpenAsync();
                await com.ExecuteNonQueryAsync();
                await com.Connection.CloseAsync();

                com.Parameters.Clear();

            }

        }









        public async Task DeleteEmpleadoAsync(int idemp)
        {
            //USAR BREARKPOINTS SI NO LLEGA O EL NOMBRE ES DISTINTO
            VistaEmpleado empleado = await this.FindEmpleadoDepartamentoPorIDAsync(idemp);
            //Lo eliminamos temporalmente
            this.context.VistaEmpleados.Remove(empleado);
            //guardamos los cambios
            await this.context.SaveChangesAsync();
        }




    }
}
