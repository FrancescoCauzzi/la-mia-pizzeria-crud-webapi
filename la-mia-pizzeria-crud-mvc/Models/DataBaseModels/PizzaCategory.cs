using la_mia_pizzeria_crud_mvc.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace la_mia_pizzeria_crud_mvc.Models.DataBaseModels
{
    public class PizzaCategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "This field is mandatory")]
        [MaxLength(100, ErrorMessage = $"The name must not exceed 100 characters")]
        public string Name { get; set; }

        // 1 - N relationship with Pizzas: one category can have many pizzas but a pizza can only have one category
        [JsonIgnore]

        public List<Pizza>? Pizzas { get; set; }

        public PizzaCategory()
        {

        }



    }
}
