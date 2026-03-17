namespace Identity.API.Application.ViewModels;

public sealed record RegisterUserViewModel(
    int UserId,
    string Email,
    string FullName);
