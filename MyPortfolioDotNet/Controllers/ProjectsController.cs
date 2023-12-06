using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPortfolioDotNet.Data;
using MyPortfolioDotNet.Models;
using Microsoft.AspNetCore.Hosting;

namespace MyPortfolioDotNet.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProjectsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Project.Include(p => p.Images).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Technology,Link")] Project project, List<IFormFile> imageFiles)
        {
            if (ModelState.IsValid)
            {
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    foreach (var file in imageFiles)
                    {
                        var imagePath = await GetUploadedFileNameAsync(file);

                        var image = new Image
                        {
                            ScreenshotPath = imagePath,
                            ProjectId = project.Id
                        };
                        _context.Images.Add(image);
                    }

                    _context.Add(project);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Privacy","Home");
            }

            return View(project);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Technology,ScreenshotPath,UploadFile")] Project project, List<IFormFile> imageFiles)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProject = await _context.Project
                        .Include(p => p.Images)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    _context.Entry(existingProject).CurrentValues.SetValues(project);

                    if (imageFiles != null && imageFiles.Count > 0)
                    {
                        foreach (var file in imageFiles)
                        {
                            var imagePath = await GetUploadedFileNameAsync(file);

                            var existingImage = existingProject.Images.FirstOrDefault(i => i.ScreenshotPath == imagePath);

                            if (existingImage == null)
                            {
                                var newImage = new Image
                                {
                                    ScreenshotPath = imagePath,
                                    ProjectId = existingProject.Id
                                };
                                existingProject.Images.Add(newImage);
                            }
                        }
                    }

                    // Rimuovi eventuali immagini non più presenti
                    var removedImages = existingProject.Images.Where(i => !imageFiles.Any(f => f.FileName == i.ScreenshotPath)).ToList();
                    foreach (var removedImage in removedImages)
                    {
                        existingProject.Images.Remove(removedImage);
                        _context.Images.Remove(removedImage); // Rimuovi l'immagine dal database
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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

            return View(project);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Project.FindAsync(id);

            if (project != null)
            {
                _context.Project.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
        }

        public async Task<string> GetUploadedFileNameAsync(IFormFile uploadFile)
        {
            if (uploadFile != null && uploadFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(uploadFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadFile.CopyToAsync(fileStream);
                }

                return "/uploads/" + fileName;
            }

            return null;
        }
    }
}
