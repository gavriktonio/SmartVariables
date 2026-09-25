# SmartVariables
Smart Variables is a system for Unity based on a [Presentation about Game Architecture by Ryan Hipple](https://www.youtube.com/watch?v=raQ3iHhE_Kk&t=2110s)

It lets the user connect scripts to each other through a system of Scriptable Objects in a modular and convenient way

The system is being used in and developed alongside [Jello](https://gavriktonio.com/jello)

## Adding Package to your Unity Project

```Window -> Package Manager -> + -> Add package from git URL...```

```https://github.com/gavriktonio/SmartVariables.git```

Samples are available through the package manager

For explanation of the system go to https://gavriktonio.com/smartvars

When an enum value changes, Enable Based on Smart Enum disables every listed object outside the new value before enabling any object in the new value. Shared objects remain active. If an activation callback changes the enum again, the component applies the latest value before enabling targets from the old transition.
