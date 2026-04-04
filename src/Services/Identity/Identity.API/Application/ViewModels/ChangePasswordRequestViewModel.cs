namespace Identity.API.Application.ViewModels;

public sealed record ChangePasswordRequestViewModel(
    string CurrentPassword,
    string NewPassword);
