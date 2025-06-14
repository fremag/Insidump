namespace Insidump.Core;

public static class ClassHelper
{
    public static List<Type> GetGenericInterfaceArguments(object? obj, Type genericInterface)
    {
        var types = new List<Type>();

        if (obj == null)
        {
            return types;
        }

        var type = obj.GetType();
        var interfaces = type.GetInterfaces();

        foreach (var typeInterface in interfaces)
        {
            if (typeInterface.IsGenericType && typeInterface.GetGenericTypeDefinition().IsAssignableFrom(genericInterface))
            {
                var genArgs = typeInterface.GetGenericArguments();
                types.AddRange(genArgs);
            }
        }

        return types;
    }
}