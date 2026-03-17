namespace Identity.API.Application.ViewModels;

public record TokenValidationViewModel(
    string? UserId,
    string? UserName,
    string? Email,
    IEnumerable<string> Roles,
    bool IsAuthenticated
);
