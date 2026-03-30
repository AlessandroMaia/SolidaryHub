using System.Diagnostics.Metrics;

namespace Campaign.API.Observability;

internal static class CampaignMetrics
{
    public const string MeterName = "SolidarityHub.Campaign";

    private static readonly Meter Meter = new(MeterName);

    private static readonly Counter<long> DonationIntentsCreatedCounter = Meter.CreateCounter<long>(
        "solidarityhub_donation_intents_created_total",
        unit: "{intent}",
        description: "Total de intencoes de doacao criadas.");

    private static readonly Counter<long> DonationIntentsProcessedCounter = Meter.CreateCounter<long>(
        "solidarityhub_donation_intents_processed_total",
        unit: "{intent}",
        description: "Total de intencoes de doacao processadas com sucesso.");

    private static readonly Counter<long> DonationIntentsFailedCounter = Meter.CreateCounter<long>(
        "solidarityhub_donation_intents_failed_total",
        unit: "{intent}",
        description: "Total de intencoes de doacao com falha.");

    private static readonly Counter<double> DonationAmountProcessedCounter = Meter.CreateCounter<double>(
        "solidarityhub_donation_amount_processed_total",
        unit: "BRL",
        description: "Valor total processado nas doacoes.");

    public static void DonationIntentCreated(string currency, string source)
        => DonationIntentsCreatedCounter.Add(1,
        [
            new KeyValuePair<string, object?>("currency", currency),
            new KeyValuePair<string, object?>("source", source)
        ]);

    public static void DonationIntentProcessed(string currency, decimal amount)
    {
        DonationIntentsProcessedCounter.Add(1,
        [
            new KeyValuePair<string, object?>("currency", currency)
        ]);

        DonationAmountProcessedCounter.Add(Convert.ToDouble(amount),
        [
            new KeyValuePair<string, object?>("currency", currency)
        ]);
    }

    public static void DonationIntentFailed(string reason)
        => DonationIntentsFailedCounter.Add(1,
        [
            new KeyValuePair<string, object?>("reason", reason)
        ]);
}
