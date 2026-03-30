using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace DonationProcessor.Observability;

internal static class DonationProcessorMetrics
{
    public const string MeterName = "SolidarityHub.DonationProcessor";

    private static readonly Meter Meter = new(MeterName);

    private static readonly Counter<long> ConsumedMessagesCounter = Meter.CreateCounter<long>(
        "solidarityhub_worker_messages_consumed_total",
        unit: "{message}",
        description: "Total de mensagens consumidas pelo worker de doacoes.");

    private static readonly Counter<long> ProcessedMessagesCounter = Meter.CreateCounter<long>(
        "solidarityhub_worker_messages_processed_total",
        unit: "{message}",
        description: "Total de mensagens processadas com sucesso pelo worker de doacoes.");

    private static readonly Counter<long> FailedMessagesCounter = Meter.CreateCounter<long>(
        "solidarityhub_worker_messages_failed_total",
        unit: "{message}",
        description: "Total de mensagens que falharam no worker de doacoes.");

    private static readonly Histogram<double> ProcessingDurationHistogram = Meter.CreateHistogram<double>(
        "solidarityhub_worker_processing_duration_ms",
        unit: "ms",
        description: "Duracao do processamento de mensagens no worker de doacoes.");

    public static void MessageConsumed(string workerName)
        => ConsumedMessagesCounter.Add(1,
        [
            new KeyValuePair<string, object?>("worker", workerName)
        ]);

    public static void MessageProcessed(string workerName, Stopwatch stopwatch)
    {
        ProcessedMessagesCounter.Add(1,
        [
            new KeyValuePair<string, object?>("worker", workerName)
        ]);

        ProcessingDurationHistogram.Record(stopwatch.Elapsed.TotalMilliseconds,
        [
            new KeyValuePair<string, object?>("worker", workerName),
            new KeyValuePair<string, object?>("outcome", "processed")
        ]);
    }

    public static void MessageFailed(string workerName, Stopwatch stopwatch)
    {
        FailedMessagesCounter.Add(1,
        [
            new KeyValuePair<string, object?>("worker", workerName)
        ]);

        ProcessingDurationHistogram.Record(stopwatch.Elapsed.TotalMilliseconds,
        [
            new KeyValuePair<string, object?>("worker", workerName),
            new KeyValuePair<string, object?>("outcome", "failed")
        ]);
    }
}
