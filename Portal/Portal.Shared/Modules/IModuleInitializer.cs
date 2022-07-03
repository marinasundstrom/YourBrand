using System;

using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Portal.Modules;

public interface IModuleInitializer
{
    static abstract void Initialize(IServiceCollection services);
}

