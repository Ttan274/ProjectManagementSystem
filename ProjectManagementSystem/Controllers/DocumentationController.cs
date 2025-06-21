using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.Documentation;
using ProjectManagementSystem.Application.Abstractions.Documentation.Dto;

namespace ProjectManagementSystem.Controllers
{
    public class DocumentationController : Controller
    {
        private readonly IDocumentationService _documentationService;

        public DocumentationController(IDocumentationService documentationService)
        {
            _documentationService = documentationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDocumentation(DocumentationDto documentation)
        {
            if(ModelState.IsValid)
            {
                var response = await _documentationService.CreateDocumentation(documentation);

                if(response)
                    return RedirectToAction("GetMyTasks", "Project");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDocumentation(Guid id)
        {
            await _documentationService.DeleteDocumentation(id);
            return RedirectToAction("GetDocuments", "Documentation");
        }

        public async Task<IActionResult> GetDocuments()
        {
            var documentations = await _documentationService.GetAllDocumentations();

            return View(documentations);
        }
    }
}
