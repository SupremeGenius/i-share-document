using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISDProject.Models;
using ISDProject.Filters;
using PagedList;
using System.IO;
using ISDProject.Tools;
using BootstrapMvcSample.Controllers;

namespace ISDProject.Controllers
{

    public class DocumentController : BootstrapBaseController
    {
        private ISDProjectContext context = new ISDProjectContext();

        //
        // GET: /Document/

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var doc_user = User.Identity.Name;
            //
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Link_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date_desc" : "Date";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            //
            var objects = context.Document.Include(objectmodels => objectmodels.Comments);

            if (!String.IsNullOrEmpty(searchString))
            {
                objects = objects.Where(s => s.DocumentName.ToUpper().Contains(searchString.ToUpper())
                                       || s.DocumentName.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Link_desc":
                    objects = objects.OrderByDescending(s => s.DocumentName);
                    break;
                case "Date":
                    objects = objects.OrderBy(s => s.DocumentCreation);
                    break;
                case "Date_desc":
                    objects = objects.OrderByDescending(s => s.DocumentCreation);
                    break;
                default:
                    objects = objects.OrderBy(s => s.DocumentName);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(objects.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Document/Details/5

        public ViewResult Details(int id)
        {
            var doc_user = User.Identity.Name;
            Document document = context.Document.Single(x => x.DocumentId == id);
            document.Comments = document.Comments.OrderBy(x => x.CommentCreation).ToList();
            Comment comment = new Comment();
            comment.DocumentId = document.DocumentId;
            comment.CommentUserName = doc_user;
            DocumentComment doc_com = new DocumentComment()
            {
                Comment = comment,
                Document = document
            };
            return View(doc_com);
        }

        //
        // GET: /Document/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Document/Create

        [HttpPost]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> files)
        {
            var doc_user = User.Identity.Name;
            // Fichiers joints
            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var doc_name = Path.GetFileName(file.FileName);
                    String dir = AppDomain.CurrentDomain.BaseDirectory + doc_user;
                    var doc_path = Path.Combine(dir, doc_name);
                    //
                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);
                    //

                    file.SaveAs(doc_path);
                    int doc_size = Tools.Tools.GetFileSize(doc_path);
                    DateTime now = DateTime.Now;
                    //
                    Document document = new Document()
                    {
                         DocumentName = doc_name,
                         DocumentPath = doc_path,
                         DocumentSize = doc_size,
                         DocumentUser = doc_user,
                         DocumentCreation = now,
                         DocumentModification = now
                    };
                    context.Document.Add(document);
                }
            }
            context.SaveChanges();
            Success("Your document is uploaded successfully !");
            return RedirectToAction("Index");
        }

        //
        // GET: /Document/Edit/5

        public ActionResult Edit(int id)
        {
            Document document = context.Document.Single(x => x.DocumentId == id);
            return View(document);
        }

        //
        // POST: /Document/Edit/5

        [HttpPost]
        public ActionResult Edit(Document document)
        {
            if (ModelState.IsValid)
            {
                context.Entry(document).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(document);
        }

        //
        // GET: /Document/Delete/5

        public ActionResult Delete(int id)
        {
            Document document = context.Document.Single(x => x.DocumentId == id);
            Tools.Tools.DeleteFile(document.DocumentPath);
            context.Document.Remove(document);
            context.SaveChanges();
            //

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ShareLink(String link, String email, String message, String file)
        {
            var doc_user = User.Identity.Name;
            Tools.Tools.SendMessage(email, doc_user, message, link, file);
            Success("Mail sended successfully !");
            return RedirectToAction("Index");
        }

        public ActionResult AddComment(DocumentComment doc_com)
        {
            DateTime now = DateTime.Now;
            doc_com.Comment.CommentCreation = now;
            doc_com.Comment.CommentModification = now;
            if (ModelState.IsValid)
            {
                context.Comment.Add(doc_com.Comment);

                context.SaveChanges();

                doc_com.Document = doc_com.Document;
                doc_com.Document.DocumentRate = doc_com.Document.DocumentRate + doc_com.Comment.CommentRate;
                doc_com.Document.DocumentModification = now;
                context.Entry(doc_com.Document).State = EntityState.Modified;
                context.SaveChanges();
            }
            Document document = context.Document.Include(x => x.Comments).Single(x => x.DocumentId == doc_com.Comment.DocumentId);
            Success("Comment added successfully !");
            return PartialView("_ListComments", document.Comments);
        }

        public void Capture()
        {
            var stream = Request.InputStream;
            string dump;

            using (var reader = new StreamReader(stream))
                dump = reader.ReadToEnd();

            var path = Server.MapPath("~/test.jpg");
            System.IO.File.WriteAllBytes(path, String_To_Bytes2(dump));
        }

        private byte[] String_To_Bytes2(string strInput)
        {
            int numBytes = (strInput.Length) / 2;
            byte[] bytes = new byte[numBytes];

            for (int x = 0; x < numBytes; ++x)
            {
                bytes[x] = Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }

            return bytes;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}