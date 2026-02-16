using Microsoft.AspNetCore.Mvc;
using MvcNetCoreEFMultiplesBBDD.Models;
using MvcNetCoreEFMultiplesBBDD.Repositories;
using System.Threading.Tasks;

namespace MvcNetCoreEFMultiplesBBDD.Controllers
{
    public class EmpleadosController : Controller
    {
        private IRepositoryEmpleados repo;

    public EmpleadosController(IRepositoryEmpleados repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<VistaEmpleado> empleados = await this.repo.GetEmpleadosDepartamentosAsync();
            return View(empleados);
        }

        public async Task<IActionResult> Details(int idemp)
        {//USAR BREARKPOINTS SI NO LLEGA O EL NOMBRE ES DISTINTO
            VistaEmpleado empleado = await this.repo.FindEmpleadoDepartamentoPorIDAsync(idemp);
            return View(empleado);
        }

        //HACEMOS EL INSERT DE EMPLEADO CON ALGUNAS MODIFICACIONES
        public IActionResult CreateEmpleado()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateEmpleado(Empleado emp, string nombredept)
        {
            await this.repo.InsertEmpleado(emp.Apellido, emp.Oficio, emp.Dir,emp.Salario,emp.Comision, nombredept);
            return RedirectToAction("Index");
        }

        //Delete
        public async Task<IActionResult> DeleteEmpleado(int idemp)
        {//USAR BREARKPOINTS SI NO LLEGA O EL NOMBRE ES DISTINTO
            await this.repo.DeleteEmpleadoAsync(idemp);
            return RedirectToAction("Index");
        }


    }
}
