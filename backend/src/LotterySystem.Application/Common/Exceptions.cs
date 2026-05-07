namespace LotterySystem.Application.Common;

public sealed class NotFoundException(string message) : Exception(message);
public sealed class BadRequestException(string message) : Exception(message);
