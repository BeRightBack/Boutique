using Microsoft.AspNetCore.Mvc;
using Boutique.Services;
using Boutique.Entity;
using Boutique.Areas.Admin.Models;
using Boutique.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;
using Boutique.Areas.Editor.Services;

namespace Boutique.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ViewHelper _viewHelper;
    private readonly DataHelper _dataHelper;
    private readonly IMapper _mapper;

    private ISession Session => _httpContextAccessor.HttpContext.Session;
    private readonly string _sessionKey = "CategoryModel";

    public CategoryController(ICategoryService categoryService,
        IHttpContextAccessor httpContextAccessor,
        ViewHelper viewHelper,
        DataHelper dataHelper,
        IMapper mapper)
    {
        _categoryService = categoryService;
        _httpContextAccessor = httpContextAccessor;
        _viewHelper = viewHelper;
        _dataHelper = dataHelper;
        _mapper = mapper;
    }


    // GET: /Category/
    public IActionResult Index()
    {
        return RedirectToAction("List");
    }

    // GET: /Category/List
    public IActionResult List()
    {
        var categoryEntities = _categoryService.GetAllCategories();
        var categoryList = new List<Models.CategoryListModel>();

        foreach (var category in categoryEntities)
        {
            var categoryModel = _mapper.Map<Category, CategoryListModel>(category);

            // check if category have parent category
            if (category.ParentCategoryId != Guid.Empty)
                categoryModel.NameWithParent = _viewHelper.GetCategoryParentMapping(category.ParentCategoryId);

            categoryModel.NameWithParent += category.Name;
            categoryList.Add(categoryModel);
        }

        return View(categoryList);
    }

    // GET: /Category/Create
    public IActionResult Create()
    {
        var model = new CategoryCreateOrUpdateModel
        {
            ParentCategorySelectList = _viewHelper.GetParentCategorySelectList()
        };

        return View(model);
    }

    // POST: /Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CategoryCreateOrUpdateModel model, bool continueEditing)
    {
        var hasError = false;

        if (ModelState.IsValid)
        {
            // check if name exist
            if (_dataHelper.CheckForDuplicate(ServiceType.Category, Helpers.DataType.Name, model.Name))
            {
                ModelState.AddModelError(string.Empty, "Category name already exist");
                hasError = true;
            }

            // create seo friendly url if the user didn't provide
            if (string.IsNullOrEmpty(model.SeoUrl))
            {
                model.SeoUrl = _dataHelper.GenerateSeoFriendlyUrl(ServiceType.Category, model.Name);
            }
            else
            {
                // check if seo already exist
                if (_dataHelper.CheckForDuplicate(ServiceType.Category, Helpers.DataType.Seo, model.SeoUrl))
                {
                    ModelState.AddModelError(string.Empty, "SEO Url already exist");
                    hasError = true;
                }
            }

            // if everything is valid
            if (!hasError)
            {
                // map model to entity
                var categoryEntity = _mapper.Map<CategoryCreateOrUpdateModel, Category>(model);
                categoryEntity.DateAdded = DateTime.Now;
                categoryEntity.DateModified = DateTime.Now;

                // save
                _categoryService.InsertCategory(categoryEntity);

                if (continueEditing)
                    return RedirectToAction("Edit", new { id = categoryEntity.Id, model.ActiveTab });

                return RedirectToAction("List");
            }
        }

        // something went wrong, redisplay form
        model.ParentCategorySelectList = _viewHelper.GetParentCategorySelectList();
        return View(model);
    }

    public IActionResult Edit(Guid? id, string ActiveTab)
    {
        if (id == null)
            return RedirectToAction("List");

        // check category id exist
        var categoryEntity = _categoryService.GetCategoryById(id ?? Guid.Empty);
        if (categoryEntity == null)
            return RedirectToAction("List");

        // map entity to model
        var model = _mapper.Map<Category, CategoryCreateOrUpdateModel>(categoryEntity);
        model.ParentCategorySelectList = _viewHelper.GetParentCategorySelectList(model.Id);
        model.ActiveTab = ActiveTab ?? model.ActiveTab;

        // add model to session
        Session.SetString(_sessionKey, JsonConvert.SerializeObject(model));

        return View(model);
    }

    // Post: /Category/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(CategoryCreateOrUpdateModel model, bool continueEditing)
    {
        var hasError = false;

        if (ModelState.IsValid)
        {
            // get model from session
            var sessionModel = JsonConvert.DeserializeObject<CategoryCreateOrUpdateModel>(Session.GetString(_sessionKey));
            model.Id = sessionModel.Id;
            model.DateAdded = sessionModel.DateAdded;

            // check if user edit the name
            if (model.Name.ToLower() != sessionModel.Name.ToLower())
            {
                // check if name exist
                if (_dataHelper.CheckForDuplicate(ServiceType.Category, Helpers.DataType.Name, model.Name))
                {
                    ModelState.AddModelError(string.Empty, "Category name already exist");
                    hasError = true;
                }
            }

            // create seo friendly url if the user didn't provide
            if (string.IsNullOrEmpty(model.SeoUrl))
            {
                model.SeoUrl = _dataHelper.GenerateSeoFriendlyUrl(ServiceType.Category, model.Name);
            }
            else
            {
                // check if user change seo url
                if (model.SeoUrl.ToLower() != sessionModel.SeoUrl.ToLower())
                {
                    // check if seo already exist
                    if (_dataHelper.CheckForDuplicate(ServiceType.Category, Helpers.DataType.Seo, model.SeoUrl))
                    {
                        ModelState.AddModelError(string.Empty, "SEO Url already exist");
                        hasError = true;
                    }
                }
            }

            // if everything works
            if (!hasError)
            {
                // map model to entity
                var categoryEntity = _mapper.Map<CategoryCreateOrUpdateModel, Category>(model);
                categoryEntity.DateModified = DateTime.Now;

                // save
                _categoryService.UpdateCategory(categoryEntity);

                if (continueEditing)
                    return RedirectToAction("Edit", new { id = categoryEntity.Id, model.ActiveTab });

                return RedirectToAction("List");
            }
        }

        // something went wrong, redisplay form
        model.ParentCategorySelectList = _viewHelper.GetParentCategorySelectList();
        return View(model);
    }

    // Post: /Category/Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(List<Guid> ids)
    {
        if (ids == null || ids.Count == 0)
            return RedirectToAction("List");

        _categoryService.DeleteCategories(ids);

        return RedirectToAction("List");
    }

}


