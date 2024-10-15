using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
namespace BlazorApp1Task1.Authentication
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private const string UserStorageKey = "user";
        private readonly BrowserStorageService _browserStorageService;
        private const string? AuthenticationType="CustomAuth";

        public CustomAuthStateProvider(BrowserStorageService browserStorageService)
        {
            _browserStorageService = browserStorageService;
            AuthenticationStateChanged += CustomAuthStateProvider_AuthenticationStateChanged;
        }

        private async void CustomAuthStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            var authState = await task;
            if (authState is not null) {
                var idStr = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!String.IsNullOrEmpty(idStr) && int.TryParse(idStr,out int id) && id>0)
                {
                    CurrentUser = new User
                    {
                        Id = id,
                        Name = authState.User.FindFirst(ClaimTypes.Name)!.Value,
                        Role = authState.User.FindFirst(ClaimTypes.Role)!.Value
                    };
                    return;
                }
            }
            CurrentUser = new();
        }

        public User CurrentUser { get; set; } = new();
        private AuthenticationState EmptyAuthState => new(new ClaimsPrincipal());
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = await _browserStorageService.GetFromStorage<User?>(UserStorageKey);
            if (user == null)
            {
                CurrentUser = new();
                return EmptyAuthState;
            }
            else
            {
                CurrentUser = user;
                var authState=GenerateAuthState(user);
                return authState;

            }
        }

        //public async Task LoginAsync(string username, string password)
        //{
        //    var user = new User { Id = 1, Name = "Srikanth", Role = "Admin" };
        //    await _browserStorageService.SaveToStorage(UserStorageKey, user);
        //    var authState = GenerateAuthState(user);
        //    NotifyAuthenticationStateChanged(Task.FromResult(authState));
        //}
        public async Task<bool> LoginAsync(string username, string password)
        {
            // Simulate user validation. Replace with real authentication logic.
            if (username == "admin" && password == "admin123")
            {
                var user = new User { Id = 1, Name = "Srikanth", Role = "Admin" };
                await _browserStorageService.SaveToStorage(UserStorageKey, user);
                var authState = GenerateAuthState(user);
                NotifyAuthenticationStateChanged(Task.FromResult(authState));
                return true; // Login successful
            }
            return false; // Invalid credentials
        }

        public async Task LogoutAsync()
        {
            await _browserStorageService.RemoveFromStorage<string>(UserStorageKey);
            NotifyAuthenticationStateChanged(Task.FromResult(EmptyAuthState));
        }

        public static AuthenticationState GenerateAuthState(User user)
        {
            Claim[] claims = [new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role,user.Role)];
            var identity = new ClaimsIdentity(claims, AuthenticationType);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(claimsPrincipal);
            return authState;
        }
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }

    public string Role { get; set; }
}
