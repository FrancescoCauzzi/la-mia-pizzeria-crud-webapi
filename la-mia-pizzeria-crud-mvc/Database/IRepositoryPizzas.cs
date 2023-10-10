using la_mia_pizzeria_crud_mvc.Models;
using la_mia_pizzeria_crud_mvc.Models.DataBaseModels;

namespace la_mia_pizzeria_crud_mvc.Database
{
    public interface IRepositoryPizzas
    {
        public Pizza GetPizzaById(int id);
        public List<Pizza> GetAllPizzas();

        public List<Pizza> GetPizzasByName(string name);

        public bool AddPizza(PizzaFormModel pizzaToAdd);

        public bool UpdatePizza(int id, PizzaFormModel updatedPizza);

        public bool DeletePizza(int id);
    }
}
