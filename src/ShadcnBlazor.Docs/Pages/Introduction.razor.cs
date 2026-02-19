using Microsoft.AspNetCore.Components;

namespace ShadcnBlazor.Docs.Pages;

public partial class Introduction : ComponentBase
{
    private const string ExampleSharedDirectoryAscii = 
"""
Components/Core/Shared/
├── Enums/
│   ├── Size.cs
│   └── Variant.cs
└── ShadcnComponentBase.cs
""";
    
    private const string ExampleButtonComponentDirectoryAscii = 
"""
Button/
├── Button.razor
├── ButtonGroup.razor
└── ButtonShared.cs
""";
    
    private const string ExamplePopoverComponentDirectoryAscii = 
"""
Popover/
├── Models/
│   ├── PopoverOptions.cs
│   ├── PopoverPlacement.cs
│   ├── PopoverRegistration.cs
│   └── PopoverWidthMode.cs
├── Services/
│   ├── IPopoverRegistry.cs
│   ├── IPopoverService.cs
│   ├── PopoverJsInterop.cs
│   ├── PopoverRegistry.cs
│   └── PopoverService.cs
├── Popover.razor
└── PopoverProvider.razor
""";
}