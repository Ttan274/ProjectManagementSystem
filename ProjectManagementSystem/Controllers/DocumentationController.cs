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

        [HttpGet]
        public IActionResult CreateDocumentation(Guid Id)
        {
            DocumentationDto documentation = new DocumentationDto { TaskId = Id };
            return View(documentation);
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
    }
}
