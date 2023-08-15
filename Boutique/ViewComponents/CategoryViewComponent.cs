using System.Linq;
using Boutique.Areas.Editor.Services;
using Boutique.Services;
using Microsoft.AspNetCore.Mvc;

namespace StoreMvc.ViewComponents
{
    [ViewComponent(Name = "Category")]
    public class CategoryViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public CategoryViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IViewComponentResult Invoke()
        {
            return View(
                _categoryService.GetAllCategoriesWithoutParent().Where(x => x.Published));
        }
    }
}