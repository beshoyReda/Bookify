﻿using Microsoft.AspNetCore.Mvc;
namespace Book.Web.Controllers
{
	public class CategoriesController:Controller
	{
		private readonly ApplicationDbContext _context;

		public CategoriesController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			//TODO: use viewModel
			var categories=_context.Categories.ToList();
			return View(categories);
		}
	}
}
