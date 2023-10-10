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
        
        private IRepositoryPizzas _repositoryPizzas;

        // Dependency Injection in the constructor
        public PizzasController(IRepositoryPizzas repositoryPizzas)
        {                        
            _repositoryPizzas = repositoryPizzas;
        }
        // get all pizzas with category and ingredients
        [HttpGet]
        public IActionResult GetPizzas()
        {
            try{

                List<Pizza> pizzas = _repositoryPizzas.GetAllPizzas();
                return Ok(pizzas);

            }catch(Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }
        // search pizzas by string snippets in pizza name
        public IActionResult SearchPizzas(string? search)
        {
            try{
                List<Pizza> foundPizzas = new();
                if(search == null){
                    foundPizzas = _repositoryPizzas.GetAllPizzas();
                }else{
                    foundPizzas = _repositoryPizzas.GetPizzasByName(search);
                }          
            
                return Ok(foundPizzas);

            }catch(Exception ex){
                return BadRequest(ex.Message);

            }
            
        }

        // get pizza by id
        [HttpGet("{id}")]        
        public IActionResult GetPizzaById(int id) {
            try{

                Pizza pizzaToGet = _repositoryPizzas.GetPizzaById(id);
                if(pizzaToGet == null){
                    return NotFound();             
                }

                return Ok(pizzaToGet);

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }                     
        }

        // here we start the POST requests
        // first create a new pizza
        [HttpPost]
        public IActionResult Create([FromBody] PizzaFormModel newPizza) {
            try{
                bool result = _repositoryPizzas.AddPizza(newPizza);
                if(!result){
                    return BadRequest();
                }
                return Ok();

            }catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }
        // modify a pizza
        [HttpPut("{id}")]
        public IActionResult Modify(int id, [FromBody] PizzaFormModel updatedPizza)
        {
            try{
                bool result = _repositoryPizzas.UpdatePizza(id, updatedPizza);
                if(!result){
                    return BadRequest();
                }

                return Ok();

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // delete a pizza
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                bool result = _repositoryPizzas.DeletePizza(id);
                if(!result){
                    return BadRequest();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                
                return BadRequest(ex.Message);
            }
        }

    }
}
