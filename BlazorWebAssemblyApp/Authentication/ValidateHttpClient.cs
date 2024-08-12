using Blazored.LocalStorage;

namespace BlazorWebAssemblyApp.Authentication
{
    public class ValidateHttpClient(ILocalStorageService _localStorageService, HttpClient _httpClient)
    {
        public string? AccessToken { get; set; }

        public HttpClient GetPublicHttpClient()
        {
            //check if the injected httpClient has a section called Authentication,then remove cause the api is not protected
            _httpClient.DefaultRequestHeaders.Remove("Authentication");
            return _httpClient;
        }

        public async Task<HttpClient> GetPrivateHttpClient()
        {
            //return, do not get from the localStorage when access token has the token
            if (!string.IsNullOrEmpty(AccessToken)) return _httpClient;

            var token = await GetTokenFromLocalStorage();
            if (token == null || token.Token is null) return _httpClient;

            _httpClient.DefaultRequestHeaders.Remove("Authentication");

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Token);

            AccessToken = token.Token;
            return _httpClient;
        }
        public async Task<AuthenticationModel> GetTokenFromLocalStorage()
        {
            string? tokenString = await _localStorageService.GetItemAsStringAsync("Authentication");
            if (string.IsNullOrEmpty(tokenString)) return null!;
            return SerializeOrDeSerialize.Deserialize(tokenString);
        }

        public async Task<bool> SetTokenToLocalStorage(AuthenticationModel tokenModel)
        {
            await _localStorageService.SetItemAsStringAsync("Authentication", SerializeOrDeSerialize.Serialize(tokenModel));
            AccessToken = string.Empty;
            await GetPrivateHttpClient();
            return true;
        }
    }
}
