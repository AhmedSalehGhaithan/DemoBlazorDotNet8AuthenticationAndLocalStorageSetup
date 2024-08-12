using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorWebAssemblyApp.Authentication
{
    public class CustomAuthenticationStateProvider(ILocalStorageService localStorageService) : AuthenticationStateProvider
    {
        //ASP.NET Core uses a ClaimsPrincipal object to represent the currently authenticated user,
        // which is accessed through the User property provided by the base classes for page models andcontrollers

        /*
            * A user is represented by a 
            * ClaimsPrincipal object. Each user can have multiple identities, which are represented by ClaimsIdentity
            * objects. An identity contains one or more pieces of information about the user, each of which is represented by a Claim
        */

        /*
         * this line of code creates a new ClaimsPrincipal object with an empty ClaimsIdentity. 
         * This means that the ClaimsPrincipal represents a user that has no claims associated with them,
         * which could be used to represent an anonymous or unauthenticated user within the application.
        */
        private ClaimsPrincipal anonmus = new ClaimsPrincipal(new ClaimsIdentity());

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var authenticationModel = await localStorageService.GetItemAsStringAsync("Authentication");
                if (authenticationModel == null)
                    return await Task.FromResult(new AuthenticationState(anonmus));

                return await Task.FromResult(new AuthenticationState(SetClaims(SerializeOrDeSerialize.Deserialize(authenticationModel).Username!)));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(anonmus));
            }

        }

        public async Task UpdateAuthenticationState(AuthenticationModel authenticationModel)
        {
            try
            {
                ClaimsPrincipal claimsPrincipal;
                if (authenticationModel != null)
                {
                    claimsPrincipal = SetClaims(authenticationModel.Username!);
                    await localStorageService.SetItemAsStringAsync("Authentication", SerializeOrDeSerialize.Serialize(authenticationModel));
                }
                else
                {
                    await localStorageService.RemoveItemAsync("Authentication");
                    claimsPrincipal = anonmus;
                }

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating authentication state: {ex.Message}");
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonmus)));
            }
        }

        private ClaimsPrincipal SetClaims(string email) => new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Name,email)}, "CustomAuth"));

    }
}