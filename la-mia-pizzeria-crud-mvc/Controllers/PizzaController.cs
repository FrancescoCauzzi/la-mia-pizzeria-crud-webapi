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

namespace la_mia_pizzeria_crud_mvc.Controllers
{
    [Authorize(Roles = "ADMIN,USER")]
    public class PizzaController : Controller
    {
        // Dependency Injection
        private ICustomLogger _myLogger;

        private PizzeriaContext _myDb;

        // Dependency Injection in the constructor
        public PizzaController(ICustomLogger myLogger, PizzeriaContext myDb)
        {
            _myLogger = myLogger;
            _myDb = myDb;
        }



        // GET: PizzaController
        public ActionResult Index()
        {
            try
            {
                _myLogger.WriteLog("User has reached the page Pizza > Index");

                List<Pizza> pizzas = _myDb.Pizzas.ToList<Pizza>();

                return View("Index", pizzas);

            }
            catch (Exception ex)
            {
                // Log the exception details here
                var errorModel = new ErrorViewModel
                {
                    //ErrorMessage = "An error occurred while retrieving the data from the database.",
                    ErrorMessage = ex.Message,
                    RequestId = HttpContext.TraceIdentifier // This is optional, just if you want to include the request ID
                };
                return View("Error", errorModel);

            }

        }

        // GET: PizzaController/Details/5


        public ActionResult Details(string name)
        {
            try
            {
                _myLogger.WriteLog($"User has reached the page Pizza {name} > Details");


                Pizza? foundedPizza = _myDb.Pizzas.Where(pizza => pizza.Name == name).Include(pizza => pizza.PizzaCategory).Include(pizza => pizza.Ingredients).FirstOrDefault();

                if (foundedPizza == null)
                {

                    var errorModel = new ErrorViewModel
                    {
                        ErrorMessage = $"The item '{name}' was not found!",
                        RequestId = HttpContext.TraceIdentifier
                    };
                    return View("Error", errorModel);
                }
                else
                {
                    return View("Details", foundedPizza);
                }

            }
            catch (Exception ex)
            {
                var errorModel = new ErrorViewModel
                {
                    //ErrorMessage = "An error occurred while retrieving the pizza details.",
                    ErrorMessage = ex.Message,
                    RequestId = HttpContext.TraceIdentifier // This is optional, just if you want to include the request ID
                };
                return View("Error", errorModel);
            }
        }



        // GET: PizzaController/Create
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public ActionResult Create()
        {
            List<PizzaCategory> pizzaCategories = _myDb.PizzaCategories.ToList();

            List<SelectListItem> allIngredientsSelectList = new List<SelectListItem>();
            List<Ingredient> databaseAllIngredients = _myDb.Ingredients.ToList();

            foreach (Ingredient ingredient in databaseAllIngredients)
            {
                allIngredientsSelectList.Add(
                    new SelectListItem
                    {
                        Text = ingredient.Name,
                        Value = ingredient.Id.ToString()
                    });
            }

            PizzaFormModel formModel = new PizzaFormModel { Pizza = new Pizza(), PizzaCategories = pizzaCategories, Ingredients = allIngredientsSelectList };
            return View("Create", formModel);
        }

        // POST: PizzaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PizzaFormModel data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.Pizza.ImageUrl))
                {
                    data.Pizza.ImageUrl = "/images/default_pizza.png";
                    //ModelState.Remove("ImageUrl");
                }
                if (!ModelState.IsValid)
                {
                    List<PizzaCategory> pizzaCategories = _myDb.PizzaCategories.ToList();
                    data.PizzaCategories = pizzaCategories;
                    // now I initialize a new empty list
                    List<SelectListItem> allIngredientsSelectList = new List<SelectListItem>();
                    // and I fill it with all the ingredients because I want to show them in the form again if the form is not filled well so the user can input them again

                    List<Ingredient> databaseAllIngredients = _myDb.Ingredients.ToList();

                    foreach (Ingredient ingredient in databaseAllIngredients)
                    {
                        allIngredientsSelectList.Add(
                            new SelectListItem
                            {
                                Text = ingredient.Name,
                                Value = ingredient.Id.ToString()
                            });
                    }

                    data.Ingredients = allIngredientsSelectList;

                    return View("Create", data);
                }
                // here if the form is filled well I can continue the process and eventually add the pizza to the database
                data.Pizza.Ingredients = new List<Ingredient>();

                //if I have any ingredient selected
                if (data.SelectedIngredientsId != null)
                {
                    foreach (string IngredientSelectedId in data.SelectedIngredientsId)
                    {
                        // down here I need to transform the string into an integer because the database stores the id as an integer but the form send it as a string
                        int intIngredientSelectedId = int.Parse(IngredientSelectedId);
                        // down here I get the ingredient from the database matching the id of the selected ingredient
                        Ingredient? ingredientInDb = _myDb.Ingredients.Where(Ingredient => Ingredient.Id == intIngredientSelectedId).FirstOrDefault();
                        // after a control for null value I add it to the list of ingredients of the pizza
                        if (ingredientInDb != null)
                        {
                            data.Pizza.Ingredients.Add(ingredientInDb);
                        }
                    }

                } // else I do not add any ingredient because the user has not selected any ingredient, the list of the ingredients to the pizza I want to add to the db remains empty       

                //finally I create the pizza and I add it to the database with all the parameters it needs to store

                Pizza newPizza = new()
                {
                    Name = data.Pizza.Name,
                    Description = data.Pizza.Description,
                    Price = data.Pizza.Price,
                    ImageUrl = data.Pizza.ImageUrl,
                    PizzaCategoryId = data.Pizza.PizzaCategoryId,
                    Ingredients = data.Pizza.Ingredients
                };

                _myDb.Pizzas.Add(newPizza);
                _myDb.SaveChanges();

                return RedirectToAction("Index");


            }
            catch (Exception ex)
            {
                string innerException = "";
                if (ex.InnerException != null)
                {
                    innerException = ex.InnerException.ToString();
                }
                var errorModel = new ErrorViewModel
                {
                    ErrorMessage = $"{ex.Message}: {innerException}",
                    RequestId = HttpContext.TraceIdentifier
                };                
                return View("Error", errorModel);

            }

        }


        // GET: PizzaController/Update/5
        [HttpGet]
        public ActionResult Update(string name)
        {
            try
            {

                Pizza? pizzaToEdit = _myDb.Pizzas.Where(Pizza => Pizza.Name == name).Include(pizza => pizza.Ingredients).FirstOrDefault();

                if (pizzaToEdit == null)
                {
                    var errorModel = new ErrorViewModel
                    {
                        ErrorMessage = $"The pizza you are searching has not been found",
                        RequestId = HttpContext.TraceIdentifier
                    };
                    return View("Error", errorModel);
                }
                else
                {
                    List<PizzaCategory> pizzaCategories = _myDb.PizzaCategories.ToList();

                    // here I initialize a new list with all the ingredients from the database
                    List<Ingredient> dbIngredientsList = _myDb.Ingredients.ToList();

                    // down here I create a new empty list of selected items
                    List<SelectListItem> selectListItems = new List<SelectListItem>();

                    // down here I iterate through the list of ingredients from the database to get some data to show in the form: the id converted to string, the name and least but not last the selected bool value which is important for a user friendly form so that the previous ingredients are still selected
                    foreach (Ingredient ingredient in dbIngredientsList)
                    {
                        selectListItems.Add(new SelectListItem
                        {
                            Value = ingredient.Id.ToString(),
                            Text = ingredient.Name,
                            // bool value to check if the ingredient is selected or not, I put the ! symbol because I'm sure that pizzaToEdit cannot be null in the else block                          
                            Selected = pizzaToEdit.Ingredients!.Any(ingredientAssociated => ingredientAssociated.Id == ingredient.Id)
                        });
                    }

                    PizzaFormModel formModel = new()
                    {
                        Pizza = pizzaToEdit,
                        PizzaCategories = pizzaCategories,
                        Ingredients = selectListItems
                    };
                    return View("Update", formModel);
                }


            }
            catch (Exception ex)
            {
                var errorModel = new ErrorViewModel
                {
                    //ErrorMessage = $"An error occurred: {ex.Message}",
                    ErrorMessage = ex.Message,
                    RequestId = HttpContext.TraceIdentifier
                };
                return View("Error", errorModel);
            }

        }

        // POST: PizzaController/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(int id, PizzaFormModel data)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    // Categories
                    List<PizzaCategory> pizzaCategories = _myDb.PizzaCategories.ToList();
                    data.PizzaCategories = pizzaCategories;

                    // Ingredients
                    List<Ingredient> dbIngredientList = _myDb.Ingredients.ToList();
                    List<SelectListItem> selectListItem = new();

                    // If I want to re-render the form with the same data I need to initialize a new list of selected ingredients and add the needed info
                    foreach (Ingredient ingredient in dbIngredientList)
                    {
                        // the selected items are the ones that were previously selected in the form 
                        selectListItem.Add(new SelectListItem
                        {
                            Value = ingredient.Id.ToString(),
                            Text = ingredient.Name
                        });
                    }

                    data.Ingredients = selectListItem;

                    return View("Update", data);
                }

                Pizza? pizzaToUpdate = _myDb.Pizzas.Where(pizza => pizza.Id == id).Include(pizza => pizza.Ingredients).FirstOrDefault();

                if (pizzaToUpdate != null)
                {
                    // down here I clear the ingredients list inside my pizza object because I want to add only the ingredients that the user has selected                    
                    pizzaToUpdate.Ingredients.Clear();

                    pizzaToUpdate.Name = data.Pizza.Name;
                    pizzaToUpdate.Description = data.Pizza.Description;
                    pizzaToUpdate.Price = data.Pizza.Price;
                    pizzaToUpdate.ImageUrl = data.Pizza.ImageUrl;
                    pizzaToUpdate.PizzaCategoryId = data.Pizza.PizzaCategoryId;

                    // down here I add the ingredients to the pizza accordingly to the data coming from the form
                    if (data.SelectedIngredientsId != null)
                    {
                        foreach (string igredientselectedId in data.SelectedIngredientsId)
                        {
                            int intIgredientselectedId = int.Parse(igredientselectedId);

                            Ingredient? igredientInDb = _myDb.Ingredients.Where(Igredient => Igredient.Id == intIgredientselectedId).FirstOrDefault();

                            if (igredientInDb != null)
                            {
                                pizzaToUpdate.Ingredients.Add(igredientInDb);
                            }
                        }
                    }

                    _myDb.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    var errorModel = new ErrorViewModel
                    {
                        ErrorMessage = $"The pizza you are trying to modify has not been found",
                        RequestId = HttpContext.TraceIdentifier
                    };
                    return View("Error", errorModel);
                }
            }
            catch (Exception ex)
            {
                var errorModel = new ErrorViewModel
                {
                    //ErrorMessage = $"An error occurred: {ex.Message}",
                    ErrorMessage = ex.Message,
                    RequestId = HttpContext.TraceIdentifier
                };
                return View("Error", errorModel);

            }
        }

        // GET: PizzaController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                Pizza? PizzaToDelete = _myDb.Pizzas.Where(Pizza => Pizza.Id == id).FirstOrDefault();

                if (PizzaToDelete != null)
                {
                    _myDb.Pizzas.Remove(PizzaToDelete);
                    _myDb.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    var errorModel = new ErrorViewModel
                    {
                        ErrorMessage = $"The pizza you are trying to delete is not present in the database",
                        RequestId = HttpContext.TraceIdentifier
                    };
                    return View("Error", errorModel);
                }
            }
            catch (Exception ex)
            {
                var errorModel = new ErrorViewModel
                {

                    ErrorMessage = ex.Message,
                    RequestId = HttpContext.TraceIdentifier
                };
                return View("Error", errorModel);

            }
        }

        // POST: PizzaController/Delete/5
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        */


    }
}
