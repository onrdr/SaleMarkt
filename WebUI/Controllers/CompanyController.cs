﻿using AutoMapper;
using Business.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;

namespace WebUI.Controllers;

public class CompanyController : BaseController
{
    private readonly ICompanyService _companyService;
    private readonly IMapper _mapper;

    public CompanyController(
        ICompanyService companyService,
        IWebHostEnvironment webHostEnvironment,
        IMapper mapper)
            : base(webHostEnvironment: webHostEnvironment)
    {
        _companyService = companyService;
        _mapper = mapper;
    }

    #region Company Details Info & Edit
    public async Task<IActionResult> Details()
    {
        var result = await _companyService.GetCompanyAsync();
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.Message;
            return View();
        }

        var companyViewModel = _mapper.Map<CompanyViewModel>(result.Data);
        return View(companyViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Details(CompanyViewModel model, IFormFile? file)
    {
        HandleImageUpload(model, file);

        var result = await _companyService.UpdateCompany(model);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.Message;
            return View();
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Details));
    }
    #endregion

    #region Private Functions
    private void HandleImageUpload(CompanyViewModel model, IFormFile? file)
    {
        var wwwRootPath = WebHostEnvironment.WebRootPath;

        if (file is not null && !string.IsNullOrEmpty(model.ImageUrl))
            DeleteOldImage(model.ImageUrl, wwwRootPath);

        if (file is not null)
            CreateNewImage(model, file, wwwRootPath);
    }

    private static void CreateNewImage(CompanyViewModel model, IFormFile file, string wwwRootPath)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var imagePath = Path.Combine(wwwRootPath, "images", "company", fileName);

        using var fileStream = new FileStream(imagePath, FileMode.Create);
        file.CopyTo(fileStream);

        model.ImageUrl = @"images\company\" + fileName;
    }

    private static void DeleteOldImage(string? imageUrl, string wwwRootPath)
    {
        if (string.IsNullOrEmpty(imageUrl)) { return; }

        var oldImagePath = Path.Combine(wwwRootPath, imageUrl.TrimStart('/'));
        if (System.IO.File.Exists(oldImagePath))
            System.IO.File.Delete(oldImagePath);
    }
    #endregion
}
