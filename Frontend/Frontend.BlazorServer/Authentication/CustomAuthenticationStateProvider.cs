using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;


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


        string token = await localStorageService.GetItemAsStringAsync("token");

        var identity = new ClaimsIdentity();
        http.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrEmpty(token))
        {
            identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
        }

        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);

        NotifyAuthenticationStateChanged(Task.FromResult(state));
        return state;



        //ClaimsPrincipal principal = new();
        //string token = await localStorageService.GetItemAsStringAsync("token");
        //http.DefaultRequestHeaders.Authorization = null;

        //if (!string.IsNullOrWhiteSpace(token))
        //{
        //    http.DefaultRequestHeaders.Authorization =
        //                      new AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
        //    JwtSecurityTokenHandler handler = new();
        //    var jwtToken = handler.ReadJwtToken(token);//这个有问题。。。。
        //    var claims = jwtToken.Claims;
        //    var identity = new ClaimsIdentity(claims, "api");
        //    principal.AddIdentity(identity);
        //}
        //var state = new AuthenticationState(principal);

        //NotifyAuthenticationStateChanged(Task.FromResult(state));


        //return new(principal);
    }
    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
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


