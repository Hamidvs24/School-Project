﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolProject.Models.Classes;
using SchoolProject.Models.Contexts;

namespace SchoolProject.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            List<User> users = await _context.Users
                .Where(u => u.Type == Models.Enums.UserType.Tələbə && u.Passive == false)
                .ToListAsync();

            users.ForEach(u => u.Class = _context.Classes.Find(u.ClassId));

            return View(users);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            User? user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            user.Class = await _context.Classes.FindAsync(user.ClassId);

            return View(user);
        }

        // GET: Students/Create
        public async Task<IActionResult> CreateAsync()
        {
            ViewBag.Classes = new SelectList(await _context.Classes.Where(c => c.Passive == false).ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Surname,Number,Email,Class")] User user)
        {
            user.Class = await _context.Classes.FindAsync(int.Parse(Request.Form["Class"].First()));

            user.Type = Models.Enums.UserType.Tələbə;

            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Classes = new SelectList(await _context.Classes.Where(c => c.Passive == false).ToListAsync(), "Id", "Name");

            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            User? user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,Number,Email,Class,Passive")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            user.Class = await _context.Classes.FindAsync(int.Parse(Request.Form["Class"].First()));

            user.Type = Models.Enums.UserType.Tələbə;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            User? user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'SchoolContext.Users'  is null.");
            }
            User? user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
