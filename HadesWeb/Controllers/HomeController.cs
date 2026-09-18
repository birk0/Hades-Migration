using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HadesWeb.Models;
using System.Text.Json;

namespace HadesWeb.Controllers
{
    [Authorize]
    public class HomeController(IWebHostEnvironment env) : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Account()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Account(AccountFormModel model)
        {
            if (ModelState.IsValid)
            {
                ModelState.AddModelError("", "Confirm your email address before performing this action");
                return View(model);
            }

            return View(model);
        }

        public ActionResult Mail()
        {
            string[] roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
            var Emails = GetEmails(roles);

            return View(Emails);
        }
        public ActionResult Downloads()
        {
            return View();
        }
        public ActionResult Security()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Security(SecurityModel model)
        {
            ViewBag.Message = "You must confirm your email before performing this action.";
            return View(model);
        }

        public ActionResult Forms()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Forms(UploadFormModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UploadedFile != null && model.UploadedFile.Length > 0)
                {
                    if (User.IsInRole("Administrators"))
                    {
                        int FileSize = (1 * 1024 * 1024);
                        if (model.UploadedFile.Length < FileSize)
                        {
                            string[] AllowedExtensions = [".docx",".odt",".pdf"];
                            string extension = Path.GetExtension(model.UploadedFile.FileName).ToLower();
                            
                            if (AllowedExtensions.Contains(extension))
                            {
                                try
                                {
                                    string fileName = $"{Guid.NewGuid()}{extension}";
                                    string path = Path.Combine("C:\\Users\\user\\Desktop", Path.GetFileName(fileName));

                                    using (var stream = new FileStream(path, FileMode.Create))
                                    {
                                        model.UploadedFile.CopyTo(stream);
                                    }
                                    ViewBag.Success = "Thank you for your report!";
                                }
                                catch (Exception) { }
                            }
                            else { ViewBag.Message = "File type is not supported."; }
                        }
                        else { ViewBag.Message = "File too large."; }
                    }
                    else { ViewBag.Message = "File Upload not permitted."; }
                }
                else { ViewBag.Success = "Thank you for your report!"; }
            }
            return View(model);
        }

        public ActionResult Download(string fileName)
        {
            try {
                string filePath = Path.Combine(env.ContentRootPath, "App_Data", "Downloads", fileName);
                string fileType = GetMimeType(fileName);

                return PhysicalFile(filePath, fileType, fileName);
            }
            catch (Exception) {
                return new StatusCodeResult(500);
            }
        }

        //grab serialised email objects matching login address
        private List<Emails> GetEmails(string[] roles)
        {
            var Path = System.IO.Path.Combine(env.ContentRootPath, "App_Data", "emails.json");
            using var Reader = new StreamReader(System.IO.File.OpenRead(Path));
            var Objects = JsonSerializer.Deserialize<List<Emails>>(Reader.ReadToEnd());

            return Objects.Where(e => roles.Contains(e.Roles)).ToList();
        }

        private static string GetMimeType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".odt" => "application/vnd.oasis.opendocument.text",
                ".doc" => "application/msword",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}
