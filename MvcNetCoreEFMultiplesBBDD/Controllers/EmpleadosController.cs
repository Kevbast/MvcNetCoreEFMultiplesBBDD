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

    }
}
