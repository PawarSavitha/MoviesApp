using FinalProject_MoviesApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FinalProject_MoviesApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;


        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7025/api/");
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Search(string query)
        {
            return await SearchMovies(query);
        }

        [HttpGet]
        public IActionResult SearchResults(List<Movie> movies)
        {
            return View(movies);
        }

        private async Task<IActionResult> SearchMovies(string query)
        {
            string apiKey = "2ffc355e";
            string apiUrl = $"http://www.omdbapi.com/?apikey={apiKey}&s={query}";

            using (var client = new HttpClient())
            {

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var movieSearchResponse = JsonConvert.DeserializeObject<MovieSearchResponse>(jsonResponse);

                    return View("SearchResults", movieSearchResponse.Search);
                }
                else
                {
                    return View("Error");
                }
            }
        }


        [HttpGet("Movies/{imdbID}")]
        public async Task<IActionResult> Movies(string imdbID)
        {
            string apiKey = "2ffc355e";
            string apiUrl = $"http://www.omdbapi.com/?apikey={apiKey}&i={imdbID}";
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var movieDetails = JsonConvert.DeserializeObject<MovieDetails>(jsonResponse);

                    return View("Movies", movieDetails);
                }
                else
                {
                    return View("Error");
                }
            }
        }

        [HttpPost("SaveMovie/{imdbID}")]
        public async Task<IActionResult> SaveMovie(string imdbID)
        {
            try

            {
                MovieAPI movieData = new MovieAPI();

                string apiKey = "2ffc355e";
                string apiUrl = $"http://www.omdbapi.com/?apikey={apiKey}&i={imdbID}";

                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var movieDetails = JsonConvert.DeserializeObject<MovieDetails>(jsonResponse);

                        if (movieData != null)
                        {
                            movieData.Id = 0;
                            movieData.Year = Convert.ToInt32(movieDetails.Year);
                            movieData.Name = movieDetails.Title;
                            movieData.Actors = movieDetails.Actors;
                        }

                    }
                }
                var jsonContent = new StringContent(JsonConvert.SerializeObject(movieData), Encoding.UTF8, "application/json");

                var response2 = await _httpClient.PostAsync("Movie", jsonContent);

                if (response2.IsSuccessStatusCode)
                {
                    return View("SaveMovie");
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpPost("DeleteMovie/{imdbID}")]
        public async Task<IActionResult> DeleteMovie(string imdbID)
        {
            try
            {
                List<MovieAPI> movieData = new List<MovieAPI>();
                MovieDetails movieDetails = new MovieDetails();

                string apiKey = "2ffc355e";
                string apiUrl = $"http://www.omdbapi.com/?apikey={apiKey}&i={imdbID}";

                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        movieDetails = JsonConvert.DeserializeObject<MovieDetails>(jsonResponse);

                    }                  
                }

                using (var httpClient = new HttpClient())
                {
                    HttpResponseMessage response3 = await httpClient.GetAsync("https://localhost:7025/api/Movie");
                    if (response3.IsSuccessStatusCode)
                    {
                        string jsonResponse2 = await response3.Content.ReadAsStringAsync();
                        movieData = JsonConvert.DeserializeObject<List<MovieAPI>>(jsonResponse2);
                    }
                }

                int id = movieData.Where(p => p.Name == movieDetails.Title).FirstOrDefault().Id;
                string apiGetMovies = string.Empty;

                if (id > 0)
                   apiGetMovies = $"https://localhost:7025/api/Movie/{id}";

                var response2 = await _httpClient.DeleteAsync(apiGetMovies);

                if (response2.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Movie successfuly deleted!!!";
                    return View("DeleteMovie");
                }
                else
                {
                    ViewBag.Message = "Movie Doesn't exist!!!";
                    return View("DeleteMovie");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Movie Doesn't exist!!!";
                return View("DeleteMovie");
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}