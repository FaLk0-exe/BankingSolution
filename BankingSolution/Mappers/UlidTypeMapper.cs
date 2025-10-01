using NJsonSchema;
using NJsonSchema.Generation.TypeMappers;

namespace BankingSolution.Mappers;

internal sealed class UlidTypeMapper : ITypeMapper
{
    public Type MappedType => typeof(Ulid);

    public bool UseReference => false;

    public void GenerateSchema(JsonSchema schema, TypeMapperContext context)
    {
        schema.Type = JsonObjectType.String;
        schema.Example = "00000000000000000000000000";
    }
}