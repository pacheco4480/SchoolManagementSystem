﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Data.Entities;
using SchoolManagementSystem.Repositories;
using SchoolManagementSystem.Helpers;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers
{
    public class TeachersController : Controller
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IConverterHelper _converterHelper;

        public TeachersController(ITeacherRepository teacherRepository, IUserHelper userHelper, IBlobHelper blobHelper, IConverterHelper converterHelper)
        {
            _teacherRepository = teacherRepository;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _converterHelper = converterHelper;
        }

        // GET: Teachers
        public async Task<IActionResult> Index()
        {
            var teachers = await _teacherRepository.GetAll().ToListAsync();
            return View(teachers);
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _teacherRepository.GetByIdAsync(id.Value);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        public async Task<IActionResult> Create()
        {
            var users = await _userHelper.GetAllUsersInRoleAsync("Teacher");
            ViewData["UserId"] = new SelectList(users, "Id", "FullName");
            return View();
        }

        // POST: Teachers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;
                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "teachers");
                }

                var teacher = _converterHelper.ToTeacher(model, imageId, true);
                await _teacherRepository.CreateAsync(teacher);
                return RedirectToAction(nameof(Index));
            }

            var users = await _userHelper.GetAllUsersInRoleAsync("Teacher");
            ViewData["UserId"] = new SelectList(users, "Id", "FullName", model.UserId);
            return View(model);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _teacherRepository.GetByIdAsync(id.Value);
            if (teacher == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToTeacherViewModel(teacher);
            var users = await _userHelper.GetAllUsersInRoleAsync("Teacher");
            ViewData["UserId"] = new SelectList(users, "Id", "FullName", model.UserId);
            return View(model);
        }

        // POST: Teachers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeacherViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = model.ImageId;
                    if (model.ImageFile != null)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "teachers");
                    }

                    var teacher = _converterHelper.ToTeacher(model, imageId, false);
                    await _teacherRepository.UpdateAsync(teacher);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TeacherExists(model.Id))
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

            var users = await _userHelper.GetAllUsersInRoleAsync("Teacher");
            ViewData["UserId"] = new SelectList(users, "Id", "FullName", model.UserId);
            return View(model);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _teacherRepository.GetByIdAsync(id.Value);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher != null)
            {
                await _teacherRepository.DeleteAsync(teacher);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TeacherExists(int id)
        {
            return await _teacherRepository.ExistAsync(id);
        }
    }
}
