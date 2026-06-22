using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Task1.model;
using Task1.Repositories;

namespace Task1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly TaskRepository _repository;
        public IndexModel(TaskRepository repository)
        {
            _repository = repository;
        }

        public List<TaskModels> Tasks { get; set; } = new List<TaskModels>();
        
        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; }

        public string assignee { get; set; }
        public void OnGet()
        {
            try
            {
                // リポジトリを呼び出し、List<TodoItem> を直接取得します
                Tasks = _repository.GetAll(Filter);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "タスクを読み込めませんでした: " + ex.Message);
                Tasks = new List<TaskModels>();
            }

        }
       public void OnPost(string assignee)
       {
            try
            {
                // リポジトリを呼び出し、List<TodoItem> を直接取得します
                Tasks = _repository.PostSearch(assignee);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "タスクを読み込めませんでした: " + ex.Message);
                Tasks = new List<TaskModels>();
            }

       }
    }
}
