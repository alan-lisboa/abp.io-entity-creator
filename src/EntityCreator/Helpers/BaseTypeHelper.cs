namespace EntityCreator.Helpers;

public class BaseTypeHelper
{
    public const string AggregateRoot = "AggregateRoot";
    public const string Entity = "Entity";
    public const string ValueObject = "ValueObject";
    public const string String = "string";

    public static bool IsNullable(string type)
    {
        return type == AggregateRoot ||
               type == Entity ||
               type == ValueObject ||
               type == String;
    }

    public static bool IsEntityType(string type)
    {
        return type == AggregateRoot ||
               type == Entity ||
               type == ValueObject;
    }

    public static bool IsAggregatedChild(string type)
    {
        return type == ValueObject || type == Entity;
    }
}
