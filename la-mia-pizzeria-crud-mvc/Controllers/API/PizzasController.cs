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
        // get all pizzas with category and ingredients
        [HttpGet]
        public IActionResult GetPizzas()
        {
            List<Pizza> pizzas = _myDb.Pizzas.Include(pizza => pizza.PizzaCategory).Include(pizza => pizza.Ingredients).ToList();
            return Ok(pizzas);
        }
        // search pizzas by string snippets in pizza name
        public IActionResult SearchPizzas(string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return BadRequest(new { Message = "You did not insert any search string" });
            }

            List<Pizza> foundPizzas = _myDb.Pizzas
                .Where(pizza => pizza.Name.ToLower().Contains(search.ToLower()))
                .ToList();

            return Ok(foundPizzas);

        }

        // get pizza by id
        [HttpGet("{id}")]
        public IActionResult GetPizzaById(int id) {
             
            Pizza? pizza = _myDb.Pizzas.Where(p => p.Id == id).Include(p => p.PizzaCategory).Include(p => p.Ingredients).FirstOrDefault();
            if (pizza != null)
            {
                return Ok(pizza);

            }
            else
            {
                return NotFound();
            }
        }

        // here we start the POST requests
        // first create a new pizza
        [HttpPost]
        public IActionResult Create([FromBody] Pizza newPizza) {
            try
            {
            _myDb.Pizzas.Add(newPizza);
            _myDb.SaveChanges();
            return Ok();

            }catch (Exception ex) { 
                return BadRequest(new {Message = ex.Message});
            }
        }
        // modify a pizza
        [HttpPut("{id}")]
        public IActionResult Modify(int id, [FromBody] Pizza updatedPizza)
        {
            Pizza? pizzaToModify = _myDb.Pizzas.Where(p => p.Id == id).FirstOrDefault();

            if(pizzaToModify == null)
            {
                return NotFound();
            }
            pizzaToModify.Name = updatedPizza.Name;
            pizzaToModify.Description = updatedPizza.Description;
            pizzaToModify.Price = updatedPizza.Price;
            pizzaToModify.PizzaCategory = updatedPizza.PizzaCategory;
            pizzaToModify.Ingredients = updatedPizza.Ingredients;
            pizzaToModify.ImageUrl = updatedPizza.ImageUrl;
            _myDb.SaveChanges();

            return Ok();
        }
        // delete a pizza
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Pizza? pizzaToDelete = _myDb.Pizzas.Where(p => p.Id == id).FirstOrDefault();
            if(pizzaToDelete == null)
            {
                return NotFound();
            }

            _myDb.Pizzas.Remove(pizzaToDelete);
            _myDb.SaveChanges();
            return Ok();
        }

    }
}
