using Boutique.Controllers;
using Boutique.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Boutique.Data;
using Boutique.Entity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Boutique.Areas.Editor.Services;
using Boutique.Areas.Editor.Models;
using Boutique.Helpers;
using Newtonsoft.Json;

namespace Boutique.Areas.Editor.Controllers;

[Area("Editor"), Authorize(Roles = "Administrator")]
public class EditorController : BaseController
{
    private readonly IDisplayService _displayService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DataHelper _dataHelper;
    private readonly IMapper _mapper;

    private ISession Session => _httpContextAccessor.HttpContext.Session;
    private readonly string _sessionKey = "ContentViewModel";

    public EditorController(
        ILanguageService languageService,
        ILocalizationService localizationService,
        IDisplayService displayService,
        IHttpContextAccessor httpContextAccessor,
        DataHelper dataHelper,
        IMapper mapper) : base(languageService, localizationService)
    {
        _displayService = displayService;
        _httpContextAccessor = httpContextAccessor;
        _dataHelper = dataHelper;
        _mapper = mapper;
    }

    public IActionResult Index()
    {
        var contentEntities = _displayService.GetAllContents();
        var contentList = new List<ContentViewModel>();

        foreach (var content in contentEntities)
        {
            var contentModel = _mapper.Map<Content, ContentViewModel>(content);

            contentList.Add(contentModel);
        }

        return View(contentList);
    }
    public IActionResult Index2()
    {
        return View();
    }
    public IActionResult Index3()
    {
        return View();
    }

    // GET: /Category/Create
    public IActionResult Create()
    {
        var model = new ContentViewModel();

        return View(model);
    }

    // POST: /Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ContentViewModel model, bool continueEditing)
    {
        var hasError = false;

        if (ModelState.IsValid)
        {
            // check if name exist
            if (_dataHelper.CheckForDuplicate(ServiceType.Content, Helpers.DataType.Name, model.Name))
            {
                ModelState.AddModelError(string.Empty, "Content name already exist");
                hasError = true;
            }

            // if everything is valid
            if (!hasError)
            {
                // map model to entity
                var contentEntity = _mapper.Map<ContentViewModel, Content>(model);

                // save
                _displayService.InsertContent(contentEntity);

                if (continueEditing)
                {
                    return RedirectToAction("Edit", new { id = contentEntity.Id });
                }


                return RedirectToAction("Index");
            }
        }

        return View(model);
    }

    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }


        // check category id exist
        var contentEntity = _displayService.GetContentById(id ?? 0);
        if (contentEntity == null)
        {
            return RedirectToAction("Index");
        }


        // map entity to model
        var model = _mapper.Map<Content, ContentViewModel>(contentEntity);

        // add model to session
        Session.SetString(_sessionKey, JsonConvert.SerializeObject(model));

        return View(model);
    }


    // Post: /Category/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(ContentViewModel model, bool continueEditing)
    {
        var hasError = false;

        if (ModelState.IsValid)
        {
            // get model from session
            var sessionModel = JsonConvert.DeserializeObject<ContentViewModel>(Session.GetString(_sessionKey));
            model.Id = sessionModel.Id;

            // check if user edit the name
            if (model.Name.ToLower() != sessionModel.Name.ToLower())
            {
                // check if name exist
                if (_dataHelper.CheckForDuplicate(ServiceType.Content, Helpers.DataType.Name, model.Name))
                {
                    ModelState.AddModelError(string.Empty, "Content name already exist");
                    hasError = true;
                }
            }

            // if everything works
            if (!hasError)
            {
                // map model to entity
                var contentEntity = _mapper.Map<ContentViewModel, Content>(model);

                // save
                _displayService.UpdateContent(contentEntity);

                if (continueEditing)
                {
                    Session.SetString(_sessionKey, JsonConvert.SerializeObject(model));
                    return RedirectToAction("Edit", new { id = contentEntity.Id });
                }

                return RedirectToAction("Index");
            }
        }
        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(List<int> ids)
    {
        if (ids == null || ids.Count == 0)
            return RedirectToAction("Index");

        _displayService.DeleteContents(ids);

        return RedirectToAction("Index");
    }

}