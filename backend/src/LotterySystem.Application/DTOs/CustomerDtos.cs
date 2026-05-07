namespace LotterySystem.Application.DTOs;

public sealed record CustomerDto(Guid Id, string Name, string? PhoneNumber);
public sealed record UpsertCustomerDto(string Name, string? PhoneNumber);
