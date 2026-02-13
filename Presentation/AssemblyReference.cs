namespace Presentation
{
    // Marker class used to reference this assembly.
    //
    // It provides a stable way to access this assembly at runtime,
    // especially for Dependency Injection registration and assembly scanning.
    //
    // Example usage in the WebAPI host:
    // builder.Services.AddAutoMapper(typeof(AssemblyReference).Assembly);
    // builder.Services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);
    //
    // This avoids directly referencing concrete types
    // and helps keep layer dependencies clean in a Clean Architecture setup.
    public sealed class AssemblyReference
    {

    }
}
