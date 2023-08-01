namespace IdentityService.WebAPI.Controllers.Login.Models;
public record UserResponse(Guid Id, string? Email, string? PhoneNumber, DateTimeOffset CreationTime);