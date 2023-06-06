using System.Diagnostics.Tracing;

namespace Microshaoft;

[EventSource(Name = $"{nameof(CommonMetricsEventSource)}")]
public sealed class CommonMetricsEventSource : AbstractCommonMetricsEventSource
{
    public static readonly CommonMetricsEventSource Logger = new ();
}