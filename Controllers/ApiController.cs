///<summary> Hear me out... a central API for error validation and implementation
///  I feel like this will be useful for larger scaling and maintenance
///</summary>


using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CentralPG.Controllers;

public class ApiController : ControllerBase
{
    public ApiController()
    {

    }
    // GET: ApiController
    public IActionResult Index()
    {
        return Ok("All was fetched successfully");
    }

    // GET: ApiController/Details/5
    public ActionResult Details(int id)
    {
        return Ok($"This is is the details for {id}");

    }

    // GET: ApiController/Create
    public ActionResult Create()
    {
        return Ok("Successfully Created");

    }

    // POST: ApiController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return Ok("failed to Create");

        }
    }

    // GET: ApiController/Edit/5
    public ActionResult Edit(int id)
    {
        return Ok($"Successfully Edited {id} ");

    }

    // POST: ApiController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return Ok($"Failed to Edit {id} ");
        }
    }

    // GET: ApiController/Delete/5
    public ActionResult Delete(int id)
    {
        return Ok("Successfully Deleted");

    }

    // POST: ApiController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return Ok("Successfully Deleted");

        }
    }
}
