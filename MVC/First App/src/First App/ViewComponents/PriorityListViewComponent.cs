using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using First_App.Models;
using Microsoft.AspNetCore.Mvc;

namespace First_App.ViewComponents
{
    public class PriorityListViewComponent : ViewComponent
    {
        private readonly List<ToDoItem> _items;

        /* private readonly ToDoContext db;

         public PriorityListViewComponent(ToDoContext context)
         {
             db = context;
         }
         */


        public PriorityListViewComponent()
        {
            _items = Enumerable.Range(0, 10).Select(i => new ToDoItem
            {
                Name = $"Item {i}",
                Priority = i,
                IsDone = i % 2 == 0
            }).ToList();
        }

        public async Task<IViewComponentResult> InvokeAsync(int maxPriority, bool isDone)
        {
            var items = await GetItemsAsync(maxPriority, isDone);
        
            return View(items);
        }

        /*
        private Task<List<ToDoItem>> GetItemsAsync(int maxPriority, bool isDone)
        {
            return db.ToDo.Where(x => x.IsDone == isDone && x.Priority <= maxPriority).ToListAsync();
        }
        */

        private Task<List<ToDoItem>> GetItemsAsync(int maxPriority, bool isDone)
        {
            return Task.FromResult(_items.Where(x => x.IsDone == isDone && x.Priority <= maxPriority).ToList());
        }
    }
}