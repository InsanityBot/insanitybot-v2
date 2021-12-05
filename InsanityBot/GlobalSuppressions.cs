// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Nested Types are required by the Discord API Wrapper")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Discord API Wrapper requires non-static methods")]
[assembly: SuppressMessage("Design", "CA1031:Catch specific exception types", Justification = "No point. Just no point.")]
[assembly: SuppressMessage("Design", "CA2254")]