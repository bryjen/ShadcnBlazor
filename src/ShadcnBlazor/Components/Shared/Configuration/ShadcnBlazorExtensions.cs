using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

/*
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
 */

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ShadcnBlazor.Components.Shared.Configuration;

[AttributeUsage(AttributeTargets.Class)]
public class RegisterServiceAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; }
    public Type? ServiceType { get; }

    public RegisterServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped, Type? serviceType = null)
    {
        Lifetime = lifetime;
        ServiceType = serviceType;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class RegisterRootComponentAttribute : Attribute
{
    public string Identifier { get; }
    public string? JavaScriptInitializer { get; }

    public RegisterRootComponentAttribute(string identifier, string? javaScriptInitializer = null)
    {
        Identifier = identifier;
        JavaScriptInitializer = javaScriptInitializer;
    }
}

public static class ShadcnBlazorExtensions
{
    public static IServiceCollection ConfigureShadcnBlazorServices(this IServiceCollection services)
    {
        var assembly = typeof(ShadcnBlazorExtensions).Assembly;

        var decorated = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => (Type: t, Attr: t.GetCustomAttribute<RegisterServiceAttribute>()))
            .Where(x => x.Attr != null);

        foreach (var (type, attr) in decorated)
        {
            var serviceType = attr!.ServiceType ?? type;
            var descriptor = new ServiceDescriptor(serviceType, type, attr.Lifetime);
            services.Add(descriptor);
        }

        return services;
    }
}
