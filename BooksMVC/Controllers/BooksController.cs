﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksMVC.Domain;
using BooksMVC.Models.Books;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly BooksDataContext _dataContext;

        public BooksController(BooksDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("/books/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _dataContext.Books.Where(b => b.Id == id)
                .Select(b => new GetSingleBookResponseModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    InInventory = b.InInventory,
                    NumberOfPages = b.NumberOfPages
                })
                .SingleOrDefaultAsync();
            if(response == null)
            {
                return NotFound("No Book with the Id");
            }
            else
            {
                return View(response);
            }
        }

        // Get /books
        // Get /books/index
        // Get /books?showall=true
        public async Task<IActionResult> Index([FromQuery] bool showall = false)
        {
            //// NO Model. just serializing the domain objects.
            //var response = await _dataContext.Books.Where(b => b.InInventory).ToListAsync();
            //return View(response);

            var response = new GetBooksResponseModel
            {
                Books = await _dataContext.Books.Where(b => b.InInventory).Select(b => new BooksResponseItemModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author
                }).ToListAsync(),
                NumberOfBooksInInventory = await _dataContext.Books.CountAsync(b => b.InInventory),
                NumberOfBooksNotInInventory = await _dataContext.Books.CountAsync(b => b.InInventory == false)
            };
            if(showall)
            {
                response.BooksNotInInventory = await _dataContext.Books.Where(b => b.InInventory == false)
                    .Select(b => new BooksResponseItemModel
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Author = b.Author
                    }).ToListAsync();
            }
            return View(response);
        }
    }
}
