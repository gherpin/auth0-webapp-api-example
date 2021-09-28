using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace webapp.Controllers
{
  public class HomeController : Controller
  {
    public async Task<IActionResult> Index()
    {
      // If the user is authenticated, then this is how you can get the access_token and id_token
      if (User.Identity.IsAuthenticated)
      {
        string accessToken = await HttpContext.GetTokenAsync("access_token");

        // if you need to check the access token expiration time, use this value
        // provided on the authorization response and stored.
        // do not attempt to inspect/decode the access token
        DateTime accessTokenExpiresAt = DateTime.Parse(
            await HttpContext.GetTokenAsync("expires_at"),
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind);

        string idToken = await HttpContext.GetTokenAsync("id_token");

        // Now you can use them. For more info on when and how to use the
        // access_token and id_token, see https://auth0.com/docs/tokens

        var weatherService = new WeatherService();
        var result = await weatherService.GetWeatherForecast(accessToken);
      }

      return View();
    }

    public IActionResult Error()
    {
      return View();
    }
  }

  public class WeatherService {

    public WeatherService()
    {

    }

    public async Task<List<WeatherForecast>> GetWeatherForecast(string accessToken) {

      var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri("https://localhost:6001");
      httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

      var response = await httpClient.GetAsync("/weatherforecast");

      if (response.IsSuccessStatusCode) {
        return await response.Content.ReadAsAsync<List<WeatherForecast>>();
      }
      return new List<WeatherForecast>();
    }
  }

    public class WeatherForecast
    {
      public DateTime Date { get; set; }

      public int TemperatureC { get; set; }

      public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

      public string Summary { get; set; }
    }

}
