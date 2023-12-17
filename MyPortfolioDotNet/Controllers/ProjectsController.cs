using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPortfolioDotNet.Data;
using MyPortfolioDotNet.Models;

namespace MyPortfolioDotNet.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProjectsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var projectsWithImages = await _context.Project
                .Include(p => p.Images)
                .Include(p => p.ProjectTechnologies)
                    .ThenInclude(pt => pt.Technology) // Include le informazioni sulle tecnologie
                .ToListAsync();

            return View(projectsWithImages);
        }


        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.Images)
                .Include(p => p.ProjectTechnologies)
                    .ThenInclude(pt => pt.Technology) // Include le informazioni sulle tecnologie
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }


        // GET: Projects/Create
        public IActionResult Create()
        {
            ViewBag.AvailableTechnologies = _context.Technology.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project, List<int> SelectedTechnologyIds)
        {
            if (ModelState.IsValid)
            {
                _context.Project.Add(project);

                // Recupero tutti i progetti dal database
                var projects = _context.Project.ToList();

                // Se OrderShow non è stato fornito (è 0 o null), assegno 1 di default
                if (project.OrderShow == 0 || project.OrderShow == null)
                {
                    project.OrderShow = 1;
                }

                // Incremento di +1 tutti gli altri progetti
                foreach (var otherProject in projects)
                {
                    if (otherProject.Id != project.Id && otherProject.OrderShow >= project.OrderShow)
                    {
                        otherProject.OrderShow++;
                    }
                }

                await _context.SaveChangesAsync();

            

                    await _context.SaveChangesAsync();


                if (SelectedTechnologyIds != null && SelectedTechnologyIds.Any())
                {
                    foreach (var techId in SelectedTechnologyIds)
                    {
                        var projectTechnology = new ProjectTechnology
                        {
                            ProjectId = project.Id,
                            TechnologyId = techId
                        };
                        _context.ProjectTechnology.Add(projectTechnology);
                    }
                    await _context.SaveChangesAsync();
                }

                if (project.UploadFiles != null && project.UploadFiles.Any(f => f.Length > 0))
                {
                    foreach (var uploadedFile in project.UploadFiles.Where(f => f.Length > 0))
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + uploadedFile.FileName;
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await uploadedFile.CopyToAsync(fileStream);
                        }

                        var image = new Image
                        {
                            ImageUrl = "/uploads/" + uniqueFileName,
                            ProjectId = project.Id
                        };

                        _context.Image.Add(image);
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.AvailableTechnologies = _context.Technology.ToList();
            return View(project);
        }




        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.Images)
                .Include(p => p.ProjectTechnologies)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            // Carica tutte le tecnologie disponibili
            ViewBag.AvailableTechnologies = await _context.Technology.ToListAsync();

            // Ottieni gli ID delle tecnologie associate al progetto
            var selectedTechIds = project.ProjectTechnologies.Select(pt => pt.TechnologyId).ToList();

            // Passa l'elenco delle tecnologie selezionate alla vista
            ViewBag.SelectedTechnologies = selectedTechIds;

            return View(project);
        }


        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project, List<int> selectedTechnologyIds, List<int> deletedImages)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var projectToUpdate = await _context.Project
                        .Include(p => p.Images)
                        .Include(p => p.ProjectTechnologies)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (projectToUpdate == null)
                    {
                        return NotFound();
                    }

                    _context.Entry(projectToUpdate).CurrentValues.SetValues(project);

                    // Aggiorna le tecnologie associate
                    if (selectedTechnologyIds != null && selectedTechnologyIds.Any())
                    {
                        projectToUpdate.ProjectTechnologies = selectedTechnologyIds
                            .Select(techId => new ProjectTechnology { ProjectId = project.Id, TechnologyId = techId })
                            .ToList();
                    }
                    else
                    {
                        projectToUpdate.ProjectTechnologies = new List<ProjectTechnology>();
                    }

                    // Gestione delle immagini
                    if (deletedImages != null && deletedImages.Any() && projectToUpdate.Images != null)
                    {
                        var imagesToRemove = projectToUpdate.Images.Where(i => deletedImages.Contains(i.Id)).ToList();
                        foreach (var imageToRemove in imagesToRemove)
                        {
                            _context.Image.Remove(imageToRemove);
                        }
                    }

                    if (project.UploadFiles != null)
                    {
                        var filesWithContent = project.UploadFiles.Where(f => f.Length > 0);
                        foreach (var uploadedFile in filesWithContent)
                        {
                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + uploadedFile.FileName;
                            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await uploadedFile.CopyToAsync(fileStream);
                            }

                            var newImage = new Image
                            {
                                ImageUrl = "/uploads/" + uniqueFileName,
                                ProjectId = project.Id
                            };

                            projectToUpdate.Images.Add(newImage);
                        }
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


        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.ProjectTechnologies)
                    .ThenInclude(pt => pt.Technology) // Include le informazioni sulle tecnologie
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }


        // POST: Projects/Delete/5
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
    }
}
