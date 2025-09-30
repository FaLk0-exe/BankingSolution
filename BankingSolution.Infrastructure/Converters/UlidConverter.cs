using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BankingSolution.Infrastructure.Converters;

internal sealed class UlidConverter : ValueConverter<Ulid, byte[]>
{
    public UlidConverter() :
        base(
            x => x.ToByteArray(),
            x => new Ulid(x))
    { }
}

internal sealed class NullableUlidConverter : ValueConverter<Ulid?, byte[]?>
{
    public NullableUlidConverter() :
        base(
            x => x != null ? x.Value.ToByteArray() : null,
            x => x != null ? new Ulid(x) : null)
    { }
}