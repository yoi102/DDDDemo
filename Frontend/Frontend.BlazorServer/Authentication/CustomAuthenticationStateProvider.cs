using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using Blazored.LocalStorage;
using Frontend.BlazorServer.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using static System.Net.WebRequestMethods;


public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient http;
    private readonly ILocalStorageService localStorageService;

    public CustomAuthenticationStateProvider(IHttpClientFactory httpClient, ILocalStorageService localStorageService)
    {
        this.http = httpClient.CreateClient("nginx-server");
        this.localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {

        ClaimsPrincipal principal = new();
        string token = await localStorageService.GetItemAsStringAsync("token");
        http.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrWhiteSpace(token))
        {
            http.DefaultRequestHeaders.Authorization =
                              new AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
            JwtSecurityTokenHandler handler = new();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims;
            var identity = new ClaimsIdentity(claims, "api");
            principal.AddIdentity(identity);
        }
        var state = new AuthenticationState(principal);

        NotifyAuthenticationStateChanged(Task.FromResult(state));


        return new(principal);
    }


    public virtual async Task StoreTokenAsync(string token)
    {
        await localStorageService.SetItemAsStringAsync("token", token);
    }

    public virtual async Task<string> RestoreTokenAsync()
    {
        return await localStorageService.GetItemAsStringAsync("token");
    }



}


