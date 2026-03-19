using SSAReplacement.Api.Domain;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Loader;

namespace SSAReplacement.Api.Features.Executables.Infrastructure;

public static class ExecutableLoadContext
{
    public static bool TryExtractExecutableParameters(string entryPointPath, out List<ExecutableParameter> parameters)
    {
        parameters = [];

        if (!File.Exists(entryPointPath))
            return false;

        var assemblyContext = new CollectibleAssemblyLoadContext();

        try
        {
            var assembly = assemblyContext.LoadFromAssemblyPath(entryPointPath);
            var settingsType = assembly.DefinedTypes.Where(x => x.Name == "Settings").FirstOrDefault();

            if (settingsType is null)
                return true;

            var properties = settingsType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanRead && x.CanWrite);
            var settingsObject = Activator.CreateInstance(settingsType);

            foreach (var property in properties)
            {
                var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                var isRequired = property.GetCustomAttributes().Any(attr => attr.GetType() == typeof(RequiredAttribute));
                var defaultValue = property.GetValue(settingsObject);
                var description = property.GetCustomAttributes().FirstOrDefault(attr => attr.GetType() == typeof(DescriptionAttribute));

                parameters.Add(new ExecutableParameter()
                {
                    Name = property.Name,
                    Description = description is DescriptionAttribute desc ? desc.Description : null,
                    TypeName = type.Name,
                    Required = isRequired,
                    DefaultValue = defaultValue?.ToString()
                });
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            assemblyContext.Unload();
        }
    }

    private sealed class CollectibleAssemblyLoadContext : AssemblyLoadContext
    {
        public CollectibleAssemblyLoadContext() : base(isCollectible: true) { }
        protected override Assembly? Load(AssemblyName assemblyName) => null;
    }
}
