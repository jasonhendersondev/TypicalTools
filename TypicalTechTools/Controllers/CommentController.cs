using TypicalTechTools.DataAccess;
using TypicalTechTools.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TypicalTechTools.Models.Repositories;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;

namespace TypicalTools.Controllers
{
    public class CommentController : Controller
    {
        //CsvParser _csvParser;

        //Reference to our repository which will receive our database requests
        private readonly ICommentRepository _repository;

        //Create variable for sanitiser class
        private readonly HtmlSanitizer _sanitizer;

        //Constructor for controller so we can use the referenced repository and sanitizer.
        public CommentController(ICommentRepository repository, HtmlSanitizer sanitizer)
        {
            _repository = repository;
            _sanitizer = sanitizer;
        }

        //Returns a list of all comments for a particular product. Anyone can access this without authentication.
        [HttpGet]
        public IActionResult CommentList(int id)
        {
            List<Comment> comments = _repository.GetAllComments(id);

            if(comments == null)
            {
                return RedirectToAction("Index", "Product");
            }

            return View(comments);

        }


        // Show a form to add a new comment
        [HttpGet]
        public IActionResult AddComment(int Id)
        {
            Comment comment = new Comment();
            comment.ProductId = Id;
            return View(comment);
        }
        /// <summary>
        /// Post a new comment. The comment will be sanitized prior to being sent to the repository for processing.
        /// </summary>
        /// <param name="comment">The comment to be posted.</param>
        /// <returns></returns>
        // Receive and handle the newly created comment data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(Comment comment)
        {
            //Store the session Id in the session data. We could technically set any value here
            //We just need to set something to the session ID gets fixed.
            HttpContext.Session.SetString("SessionId", HttpContext.Session.Id);

            //Assign the session ID to the comment
            comment.SessionId = HttpContext.Session.Id;
            //Set the created date to current date time
            comment.CreatedDate = DateTime.Now;
            //Sanitizing the comment text to prevent script injection
            comment.Text = _sanitizer.Sanitize(comment.Text);


            //Check if all fields are filled in correctly to match the model state. If unsuccessful, 
            if (ModelState.IsValid)
            {

                _repository.CreateComment(comment);

                // A session id is only set once a value has been added!
                // adding a value here to ensure the session is created
                //HttpContext.Session.SetString("CommentText", comment.Text);

                return RedirectToAction("Index", "Product");
            }

            return View(comment);


        }
        //Remove comment. User can remove their own comment with this endpoint.
        [Authorize]
        [HttpGet]
        public ActionResult RemoveComment(int Id)
        {
            var comment = _repository.GetSingleComment(Id);
            return View(comment);

        }

        // Receive and handle a request to Delete a comment. User must be logged in to perform.
        [Authorize]
        [HttpPost]
        public IActionResult RemoveComment(int Id, IFormCollection collection)
        {
            var comment = _repository.GetSingleComment(Id);
            int productId = comment.ProductId;


            _repository.DeleteSingleComment(Id);


           return RedirectToAction("CommentList", new { id = productId });
        }

        // Show a existing comment details in a form to allow for editing. User must be logged in to access.
        [Authorize]
        [HttpGet]
        public ActionResult EditComment(int id)
        {
            Comment comment = _repository.GetSingleComment(id);
            return View(comment);
        }

       /// <summary>
       /// Edit comment. A user must be authorized to perform. The text will also be sanitized prior to being sent to the repository for processing.
       /// </summary>
       /// <param name="comment">The edited comment to be passed to the view</param>
       /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment(Comment comment)
        {
            //If no comment is received, return user to Products page.
            if(comment == null)
            {
                return RedirectToAction("Index", "Product");
            }

            //Sanitize edited comment before sending to repository
            comment.Text = _sanitizer.Sanitize(comment.Text);


            try
            {
                //If the comment meets the model state rules (which it should, as the user can only modify the text here), send to the repository for processing.
                if (ModelState.IsValid)
                {
                    _repository.UpdateComment(comment);
                    return RedirectToAction("CommentList", "Comment", new { id = comment.ProductId });
                }
                return View(comment);
            }
            catch
            {
                return View(comment);
            }


            

            



        }
    }
}
