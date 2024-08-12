using BlazorWebAssemblyApp.Authentication;
using SharedLibrary.Models;
using System.Net.Http.Json;

namespace BlazorWebAssemblyApp.Services
{
    public class ClientService(ValidateHttpClient _validateHttpClient) : IClientService
    {

        public async Task<WeatherForecast[]> GetWeatherForecasts()
        {
            var result = await _validateHttpClient.GetPrivateHttpClient();
            var response = await result.GetAsync("api/WeatherForecast");

            var checkResponseIfUnauthorized = CheckResponse(response);
            if (!checkResponseIfUnauthorized)
                return await response.Content.ReadFromJsonAsync<WeatherForecast[]>();

            if (!await RequestAndSetNewToken())
                return null!;

            return await GetWeatherForecasts();

        }

        public async Task<bool> RequestAndSetNewToken()
        {
            var getToken = await _validateHttpClient.GetTokenFromLocalStorage();

            var result = await _validateHttpClient.GetPublicHttpClient().PostAsJsonAsync("refresh", new PostToken { RefreshToken = getToken.RefreshToken });
            var response = await result.Content.ReadFromJsonAsync<LoginResponse>();
            if (result is null)
                return false;

            await _validateHttpClient.SetTokenToLocalStorage(new AuthenticationModel { RefreshToken = response!.RefreshToken, Token = response.AccessToken, Username = getToken.Username });
            return true;
        }
        private bool CheckResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return true;
            else
                return false;
        }


    }
}
