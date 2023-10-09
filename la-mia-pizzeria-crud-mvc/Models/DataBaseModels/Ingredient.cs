using System.ComponentModel.DataAnnotations;
using la_mia_pizzeria_crud_mvc.ValidationAttributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace la_mia_pizzeria_crud_mvc.Models.DataBaseModels
{
    public class Ingredient
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "This field is mandatory")]
        [MaxLength(100, ErrorMessage = $"The name must not exceed 100 characters")]
        public string Name { get; set; }

        public List<Pizza> Pizzas { get; set; }

        // Empty constructor necessary to EF
        public Ingredient() { 
        
        }
    }
}
