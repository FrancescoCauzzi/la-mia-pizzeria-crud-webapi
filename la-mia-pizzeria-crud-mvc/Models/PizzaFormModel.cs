using la_mia_pizzeria_crud_mvc.Models.DataBaseModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace la_mia_pizzeria_crud_mvc.Models
{
    public class PizzaFormModel
    {
        public Pizza Pizza { get; set; }

        public List<PizzaCategory>? PizzaCategories { get; set; }

        // We insert the new properties to manage a multiple selection
        public List<SelectListItem>? Ingredients { get; set; }
        public List<string>? SelectedIngredientsId { get; set; }
    }
}
