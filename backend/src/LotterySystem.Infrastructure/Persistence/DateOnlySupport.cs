using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LotterySystem.Infrastructure.Persistence;

public sealed class DateOnlyConverter() : ValueConverter<DateOnly, DateTime>(
    d => d.ToDateTime(TimeOnly.MinValue),
    d => DateOnly.FromDateTime(d));

public sealed class DateOnlyComparer() : ValueComparer<DateOnly>(
    (d1, d2) => d1.DayNumber == d2.DayNumber,
    d => d.GetHashCode());
