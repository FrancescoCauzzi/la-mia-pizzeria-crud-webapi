using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using la_mia_pizzeria_crud_mvc.Models;
using la_mia_pizzeria_crud_mvc.Database;
using Microsoft.Docs.Samples;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using la_mia_pizzeria_crud_mvc.CustomLoggers;
using la_mia_pizzeria_crud_mvc.Models.DataBaseModels;
using Azure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;




namespace la_mia_pizzeria_crud_mvc.Database
{
    public class RepositoryPizzas : IRepositoryPizzas
    {

        private PizzeriaContext _db;
        
        
        public RepositoryPizzas(PizzeriaContext db)
        {
            _db = db;
            
        }
        public List<Pizza> GetAllPizzas()
        {
            List<Pizza> pizzas = _db.Pizzas.Include(pizza => pizza.PizzaCategory).Include(pizza => pizza.Ingredients).ToList();
            return pizzas;
        }

        public Pizza GetPizzaById(int id)
        {
            Pizza? pizza = _db.Pizzas.Where(p => p.Id == id).Include(p => p.PizzaCategory).Include(p => p.Ingredients).FirstOrDefault();
            if (pizza != null)
            {
                return pizza;

            }
            else
            {
                throw new Exception("This pizza has not been found");
            }
        }

        public List<Pizza> GetPizzasByName(string name)
        {
            List<Pizza> foundPizzas = _db.Pizzas
                 .Where(pizza => pizza.Name.ToLower().Contains(name.ToLower()))
                 .ToList();
            return foundPizzas;
        }
        public bool AddPizza(PizzaFormModel pizzaToAdd)
        {
            try
            {
                
                pizzaToAdd.Pizza.Ingredients = new List<Ingredient>();

                //if I have any ingredient selected
                if (pizzaToAdd.SelectedIngredientsId != null)
                {
                    foreach (string IngredientSelectedId in pizzaToAdd.SelectedIngredientsId)
                    {
                        // down here I need to transform the string into an integer because the database stores the id as an integer but the form send it as a string
                        int intIngredientSelectedId = int.Parse(IngredientSelectedId);
                        // down here I get the ingredient from the database matching the id of the selected ingredient
                        Ingredient? ingredientInDb = _db.Ingredients.Where(Ingredient => Ingredient.Id == intIngredientSelectedId).FirstOrDefault();
                        // after a control for null value I add it to the list of ingredients of the pizza
                        if (ingredientInDb != null)
                        {
                            pizzaToAdd.Pizza.Ingredients.Add(ingredientInDb);
                        }
                    }

                }
                Pizza newPizza = new()
                {
                    Name = pizzaToAdd.Pizza.Name,
                    Description = pizzaToAdd.Pizza.Description,
                    Price = pizzaToAdd.Pizza.Price,
                    ImageUrl = pizzaToAdd.Pizza.ImageUrl,
                    PizzaCategoryId = pizzaToAdd.Pizza.PizzaCategoryId,
                    Ingredients = pizzaToAdd.Pizza.Ingredients
                };
                _db.Pizzas.Add(newPizza);
                _db.SaveChanges();
                

                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public bool UpdatePizza(int id, PizzaFormModel data)
        {

            Pizza? pizzaToUpdate = _db.Pizzas.Where(Pizza => Pizza.Id == id).Include(pizza => pizza.Ingredients).Include(pizza => pizza.PizzaCategory).FirstOrDefault();

            if (pizzaToUpdate == null)
            {
                return false;
            }
            else
            {
                // down here I clear the ingredients list inside my pizza object because I want to add only the ingredients that the user has selected                    
                pizzaToUpdate.Ingredients.Clear();
                pizzaToUpdate.Name = data.Pizza.Name;
                pizzaToUpdate.Description = data.Pizza.Description;
                pizzaToUpdate.Price = data.Pizza.Price;
                pizzaToUpdate.ImageUrl = data.Pizza.ImageUrl;
                pizzaToUpdate.PizzaCategoryId = data.Pizza.PizzaCategoryId;

                // down here I add the ingredients to the pizza accordingly to the data coming from the form
                // if (data.Ingredients != null)
                // {
                //     foreach (Ingredient igredient in data.Ingredients)
                //     {                                           
                //         if (igredient != null)
                //         {
                //             pizzaToUpdate.Ingredients.Add(igredient);
                //         }
                //     }
                // }
                if (data.SelectedIngredientsId != null)
                    {
                        foreach (string igredientselectedId in data.SelectedIngredientsId)
                        {
                            int intIgredientselectedId = int.Parse(igredientselectedId);

                            Ingredient? igredientInDb = _db.Ingredients.Where(Igredient => Igredient.Id == intIgredientselectedId).FirstOrDefault();

                            if (igredientInDb != null)
                            {
                                pizzaToUpdate.Ingredients.Add(igredientInDb);
                            }
                        }
                    }


                _db.SaveChanges();
                return true;
            }
        }
        public bool DeletePizza(int id)
        {
            

            Pizza? pizzaToDelete = _db.Pizzas.Where(p => p.Id == id).FirstOrDefault();
            if (pizzaToDelete == null)
            {
                return false;
            }

            _db.Pizzas.Remove(pizzaToDelete);
            _db.SaveChanges();

            return true;
            
        }
               
              
    }
}
