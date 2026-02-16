

#region STORED PROCEDURES
/*

DELIMITER //

CREATE PROCEDURE SP_ALL_VEMPLEADOS()
BEGIN
   SELECT * FROM V_EMPLEADOSDEPT;
END //

DELIMITER ;


DELIMITER //

DROP PROCEDURE IF EXISTS SP_INSERT_EMPLEADO //

CREATE PROCEDURE SP_INSERT_EMPLEADO (
    IN apellido   VARCHAR(50),
    IN oficio     VARCHAR(50),
    IN dir        INT,
    IN salario    INT,
    IN comision   INT,
    IN nombredept VARCHAR(50)
)
BEGIN
    DECLARE empno INT;
    DECLARE fecha_alt DATETIME;
    DECLARE deptno INT;

    -- Obtener siguiente EMP_NO
    SELECT IFNULL(MAX(EMP_NO),0) INTO empno
    FROM EMP;

    SET empno = empno + 1;

    -- Fecha actual
    SET fecha_alt = NOW();

    -- Obtener departamento
    SELECT DEPT_NO INTO deptno
    FROM DEPT
    WHERE DNOMBRE = nombredept;

    -- Insertar empleado
    INSERT INTO EMP
    VALUES (empno, apellido, oficio, dir, fecha_alt, salario, comision, deptno);

END //

DELIMITER ;

select * from EMP




 */
#endregion

using Microsoft.EntityFrameworkCore;
using MvcNetCoreEFMultiplesBBDD.Data;
using MvcNetCoreEFMultiplesBBDD.Models;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

namespace MvcNetCoreEFMultiplesBBDD.Repositories
{
    public class RepositoryEmpleadosMySql:IRepositoryEmpleados
    {
        private HospitalContext context;

        public RepositoryEmpleadosMySql(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<VistaEmpleado>> GetEmpleadosDepartamentosAsync()
        {//COMO EL CÓDIOGO DE CADA BBDD ES DISTINTO Y ESTAMOS MEZCLANDO EF CON PROCEDURES Y VISTAS DEBERÍAMOS CREAR UNA INTERFAZ
            
            //LLAMAMOS A LA CONSULTA
            string sql = " CALL SP_ALL_VEMPLEADOS";
            var consulta = this.context.VistaEmpleados.FromSqlRaw(sql);

            List<VistaEmpleado> data = await
                consulta.ToListAsync();

            return data;

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

        public async Task DeleteEmpleadoAsync(int idemp)
        {
            //USAR BREARKPOINTS SI NO LLEGA O EL NOMBRE ES DISTINTO
            VistaEmpleado empleado = await this.FindEmpleadoDepartamentoPorIDAsync(idemp);
            //Lo eliminamos temporalmente
            this.context.VistaEmpleados.Remove(empleado);
            //guardamos los cambios
            await this.context.SaveChangesAsync();
        }

        public async Task InsertEmpleado(string apellido, string oficio, int dir, int salario, int comision, string nombredept)
        {
            string sql = "SP_INSERT_EMPLEADO";
            MySqlParameter pamApell = new MySqlParameter("apellido", apellido);
            MySqlParameter pamOfi = new MySqlParameter("oficio", oficio);
            MySqlParameter pamDir = new MySqlParameter("dir", dir);
            MySqlParameter pamSalario = new MySqlParameter("salario", salario);
            MySqlParameter pamComi = new MySqlParameter("comision", comision);
            MySqlParameter pamDept = new MySqlParameter("nombredept", nombredept);

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



    }
}
