using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Task1.model;
using Task1.Repositories;
using static Microsoft.Data.SqlClient.Internal.SqlClientEventSource;

namespace Task1.Pages
{
    public class EditModel : PageModel
    {

        private readonly TaskRepository _repository;
        public EditModel(TaskRepository repository)
        {
            _repository = repository;
        }

        [BindProperty]
        public TaskModels Result { get; set; }
        public void OnGet(int Id)
        {
            try
            {
                Result = _repository.GetById(Id);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "タスクを読み込めませんでした: " + ex.Message);
                Result = new TaskModels();
            }

        }
        public IActionResult OnPost(TaskModels task)
        {
            if (string.IsNullOrWhiteSpace(task.TASK_NAME) || string.IsNullOrWhiteSpace(task.ASSIGNEE))
            {
                ModelState.AddModelError("", "文字を入力してください（スペースのみは不可）");
                return RedirectToPage("Index");
            }


            try
            {
                _repository.Update(task);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "タスクを読み込めませんでした: " + ex.Message);
            }


            return RedirectToPage("Index");
        }
    }

}

