// using MassTransit;
//
// namespace Common.Api.BaseConfiguration;
//
// public class CustomEntityNameFormatter : IEntityNameFormatter
// {
//     private readonly string _prefix;
//
//     public CustomEntityNameFormatter(string prefix)
//     {
//         _prefix = prefix;
//     }
//
//     public string FormatEntityName<T>()
//     {
//         return $"{_prefix}{typeof(T).Name}";
//     }
// }