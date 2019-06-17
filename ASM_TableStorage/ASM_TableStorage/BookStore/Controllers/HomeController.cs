using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        BookDataSource bookData = new BookDataSource();
        // GET: Home
        public ActionResult Index()
        {
            return RedirectToAction("Create");
        }
        public ActionResult ViewBooks()
        {
            var model = bookData.GetBooks();
            return View(model);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(BookEntity newBook, HttpPostedFileBase imageUpload)
        {
            newBook.PartitionKey = Guid.NewGuid().ToString();
            newBook.RowKey = newBook.BookID.ToString();
            newBook.BookImage = bookData.ImageURL(imageUpload);
            bookData.AddBook(newBook);
            ViewBag.Msg = "Save completed.";
            return View();
        }
        public ActionResult Delete(int rowKey, string partitionKey)
        {
            bookData.DeleteBook(rowKey, partitionKey);
            return RedirectToAction("ViewBooks");
        }

        [HttpPost]
        public ActionResult Search(String searchTitle)
        {
            var model = bookData.SearchBooks(searchTitle);
            return View("ViewBooks", model);
        }

        public ActionResult Update(int rowKey, string partitionKey)
        {
            var model = bookData.FindBook(rowKey, partitionKey);
            return View("Update", model);
        }
        [HttpPost]
        public ActionResult Edit(BookEntity newBook, HttpPostedFileBase imageUpload)
        {
            newBook.BookImage = bookData.ImageURL(imageUpload);
            bookData.Update(newBook);

            return RedirectToAction("ViewBooks");
        }
    }
}