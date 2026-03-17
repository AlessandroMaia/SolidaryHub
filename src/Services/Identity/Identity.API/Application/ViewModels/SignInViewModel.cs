namespace Identity.API.Application.ViewModels;

public sealed record SignInViewModel(
    int UserId,
    string Email,
    string FullName,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiration,
    IReadOnlyList<string> Roles);