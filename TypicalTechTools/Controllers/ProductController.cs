using TypicalTechTools.DataAccess;
using TypicalTechTools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TypicalTechTools.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Ganss.Xss;

namespace TypicalTools.Controllers
{
    public class ProductController : Controller
    {

        //Reference to our repository which will receive our database requests
        private readonly IProductRepository _repository;

        //Create variable for sanitiser class
        private readonly HtmlSanitizer _sanitizer;

        //Request the repository in the constructor by passing it as an input parameter then
        //pass it to our reference field. We are requesting this by the interface name, not the
        //class associated with it, so that if the class is changed, we do not need to change this code
        //so long as it uses the same interface
        public ProductController(IProductRepository repository, HtmlSanitizer sanitizer)
        {
            _repository = repository;
            _sanitizer = sanitizer;
        }


        //Return the details of a particular product. This can be accessed without authentication.
        //GET: ProductController/Details
        [HttpGet]
        public ActionResult Details(int id)
        {
            var product = _repository.GetProductById(id);
            return View(product);
        }


        //Return a list of all products
        //GET: ProductController
        public ActionResult Index()
        {
            var products = _repository.GetAllProducts();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //Create product view. Must have ADMIN role to be able to access.
        //GET: ProductController/Edit
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public ActionResult Create(int id)
        {

            var product = _repository.GetProductById(id);
            return View(product);
        }
        /*
         * 
         * 
         * */
        //Edit Product view. Must have ADMIN role to be able to access.
        //GET: ProductController/Edit
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public ActionResult Edit(int id)
        {

            var product = _repository.GetProductById(id);
            return View(product);
        }


        /** 
         * 
         * POST METHODS
         * 
         */

        
        // POST: ProductController/Create
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            try
            {
                //Sanitize Product Name and Description prior to processing
                //(I have not included Price as it is a decimal type and any other input will be rejected by the Model State Check)
                product.Name = _sanitizer.Sanitize(product.Name);
                product.Description = _sanitizer.Sanitize(product.Description);


                //Set updated date to the current time
                product.UpdatedDate = DateTime.UtcNow;

                //Create new product if input meets model rules
                if (ModelState.IsValid)
                {
                    
                    _repository.CreateProduct(product);
                    return RedirectToAction(nameof(Index));
                }
                return View(product);
            }
            catch
            {
                return View(product);
            }

        }


        //Update the price of a product. Only users with "ADMIN" role can access this endpoint.
        // POST: ProductController/Edit
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            try
            {

                //Set updated date to the current time

                product.UpdatedDate = DateTime.Now;

                //Edit product if input meets model rules

                if (ModelState.IsValid)
                {
                    _repository.UpdateProduct(product);
                    return RedirectToAction(nameof(Index));
                }
                return View(product);
            }
            catch
            {
                return View(product);
            }

        }

        //Delete selected product. User must have "ADMIN" role to access this endpoint.
        // DELETE: ProductController/Delete
        [Authorize(Roles = "ADMIN")]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection form)
        {
            try
            {
                //Delete product and redirect user to Products page
                _repository.DeleteProduct(id);
                return RedirectToAction(nameof(Index));
            }
            catch 
            {
                //If the delete fails, return the user back to the product's page.
                var product = _repository.GetProductById(id);
                return View(product);
            }
        }
    }
}
