using la_mia_pizzeria_crud_mvc.CustomLoggers;
using la_mia_pizzeria_crud_mvc.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using la_mia_pizzeria_crud_mvc.Models.DataBaseModels;
using la_mia_pizzeria_crud_mvc.Models;
using la_mia_pizzeria_crud_mvc.Database;
using Microsoft.EntityFrameworkCore;


namespace la_mia_pizzeria_crud_mvc.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PizzasController : ControllerBase
    {
        private PizzeriaContext _myDb;

        // Dependency Injection in the constructor
        public PizzasController(PizzeriaContext myDb)
        {            
            _myDb = myDb;
        }
        [HttpGet]
        public IActionResult GetPizzas()
        {
            List<Pizza> pizzas = _myDb.Pizzas.ToList();
            return Ok(pizzas);
        }
    }
}
