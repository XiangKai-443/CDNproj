using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using CDNui.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CDNui.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _client;
        Uri baseAddress = new Uri("https://localhost:7083/api");

        public UserController()
        {
            HttpClientHandler handler = new HttpClientHandler();
            // Allow untrusted SSL certificates (for development purposes only)
            handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            _client = new HttpClient { BaseAddress = new Uri("https://localhost:7083/api") };
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<UserModel> userList = new List<UserModel>();

            try
            {
                //HttpResponseMessage response = await _client.GetAsync("/User");
                
                HttpResponseMessage response = _client.GetAsync($"{baseAddress}/User").Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    userList = JsonConvert.DeserializeObject<List<UserModel>>(data);
                }
                else
                {
                    ViewBag.Error = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Unexpected error: {ex.Message}";
            }

            return View(userList);
        }
        [HttpGet]
        public IActionResult Create() 
        { 
            return View();
        }
        [HttpPost]
        /*[Route("user/create")]*/
        public async Task<IActionResult> AddUser(/*[FromBody] */UserModel newUser, string submitButton)
        {
            if (submitButton == "Create")
            {
                if (_client != null)
                {
                    try
                    {
                        string userJson = JsonConvert.SerializeObject(newUser);
                        HttpContent content = new StringContent(userJson, Encoding.UTF8, "application/json");

                        // Send HTTP POST request to create a new user
                        HttpResponseMessage response = await _client.PostAsync($"{baseAddress}/User/Create", content);

                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ViewBag.Error = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                            return View("Error"); 
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = $"Unexpected error: {ex.Message}";
                        return View("Error");
                    }
                }
                else
                {
                    ViewBag.Error = "HTTP client is not initialized.";
                    return View("Error");
                }

            }
            return RedirectToAction("Index");



        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            UserModel user = new UserModel();
            HttpResponseMessage response = _client.GetAsync($"{baseAddress}/User/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                // User created successfully
                string data = response.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<UserModel>(data);
            }
            return View(user);
        }
        [HttpPost]
        /*[Route("User/UpdateUser/{id}")]*/
        public async Task<IActionResult> UpdateUser(/*int id, [FromBody]*/ UserModel updatedUser, string submitButton)
        {
            if (submitButton == "Save")
            {
                try
                {
                    string userJson = JsonConvert.SerializeObject(updatedUser);
                    HttpContent content = new StringContent(userJson, Encoding.UTF8, "application/json");

                    int id = updatedUser.UserId;

                    // Send HTTP PUT request to update an existing user
                    //HttpResponseMessage response = await _client.PutAsync($"{baseAddress}/User/UpdateUser/{id}", content);
                    HttpResponseMessage response = await _client.PutAsync($"{baseAddress}/user/{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["successMessage"] = "User details updated";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Error = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                        return View("Error"); 
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Unexpected error: {ex.Message}";
                    return View("Error"); 
                }
            }
            return RedirectToAction("Index");

        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                HttpResponseMessage response = await _client.DeleteAsync($"{baseAddress}/User/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                    return View("Error"); 
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Unexpected error: {ex.Message}";
                return View("Error"); 
            }
        }
    }
}