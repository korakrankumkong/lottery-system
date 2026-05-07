using FluentValidation;
using LotterySystem.Application.DTOs;

namespace LotterySystem.Application.Validators;

public sealed class CreateTicketRequestValidator : AbstractValidator<CreateTicketRequestDto>
{
    public CreateTicketRequestValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.LotteryRoundId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new TicketItemRequestValidator());
    }
}

public sealed class UpdateTicketRequestValidator : AbstractValidator<UpdateTicketRequestDto>
{
    public UpdateTicketRequestValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.LotteryRoundId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new TicketItemRequestValidator());
    }
}

public sealed class TicketItemRequestValidator : AbstractValidator<TicketItemRequestDto>
{
    public TicketItemRequestValidator()
    {
        RuleFor(x => x.Number).NotEmpty().Matches("^\\d{2,6}$");
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
