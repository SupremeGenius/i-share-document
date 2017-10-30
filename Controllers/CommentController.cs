using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISDProject.Models;

namespace ISDProject.Controllers
{   
    public class CommentController : Controller
    {
        private ISDProjectContext context = new ISDProjectContext();

        //
        // GET: /Comment/

        public ViewResult Index()
        {
            return View(context.Comment.ToList());
        }

        //
        // GET: /Comment/Details/5

        public ViewResult Details(int id)
        {
            Comment comment = context.Comment.Single(x => x.CommentId == id);
            return View(comment);
        }

        //
        // GET: /Comment/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Comment/Create

        [HttpPost]
        public ActionResult Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                context.Comment.Add(comment);
                context.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(comment);
        }
        
        //
        // GET: /Comment/Edit/5
 
        public ActionResult Edit(int id)
        {
            Comment comment = context.Comment.Single(x => x.CommentId == id);
            return View(comment);
        }

        //
        // POST: /Comment/Edit/5

        [HttpPost]
        public ActionResult Edit(Comment comment)
        {
            if (ModelState.IsValid)
            {
                context.Entry(comment).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(comment);
        }

        //
        // GET: /Comment/Delete/5
 
        public ActionResult Delete(int id)
        {
            Comment comment = context.Comment.Single(x => x.CommentId == id);
            return View(comment);
        }

        //
        // POST: /Comment/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = context.Comment.Single(x => x.CommentId == id);
            context.Comment.Remove(comment);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}