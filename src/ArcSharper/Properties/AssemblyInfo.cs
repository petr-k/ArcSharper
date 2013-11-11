using System.Reflection;
using JetBrains.ActionManagement;
using JetBrains.Application.PluginSupport;
using JetBrains.ReSharper.Daemon;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ArcSharper")]
[assembly: AssemblyDescription("ArcSharper")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Petr Krebs")]
[assembly: AssemblyProduct("ArcSharper")]
[assembly: AssemblyCopyright("Copyright © Petr Krebs, 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: ActionsXml("ArcSharper.Actions.xml")]

// The following information is displayed by ReSharper in the Plugins dialog
[assembly: PluginTitle("ArcSharper")]
[assembly: PluginDescription("ArcSharper")]
[assembly: PluginVendor("Petr Krebs")]

[assembly: RegisterConfigurableSeverity("DangerousArcObjectSingletonInstantiation", null, HighlightingGroupIds.BestPractice, "ArcSharper: Potentially dangerous ArcObject singleton instantation", "Potentially dangerous ArcObject singleton instantation", Severity.WARNING, false)]