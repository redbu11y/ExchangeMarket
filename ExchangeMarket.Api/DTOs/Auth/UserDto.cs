namespace ExchangeMarket.Api.DTOs.Auth;

public record UserDto(string Id, string Email, string FullName, IList<string> Roles);