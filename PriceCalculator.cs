namespace VetClinic;

using System;
using System.Collections.Generic;

/// <summary>
/// Статический калькулятор стоимости услуг клиники.
/// </summary>
public static class PriceCalculator
{
    /// <summary>Справочник базовых тарифов (можно расширять).</summary>
    private static readonly Dictionary<string, decimal> PriceList = new(StringComparer.OrdinalIgnoreCase)
    {
        { "осмотр", 500m },
        { "прививка", 800m },
        { "операция", 5000m },
        { "чистка зубов", 1200m },
        { "консультация", 400m }
    };

    /// <summary>
    /// Рассчитать стоимость услуги по названию.
    /// Неизвестная услуга получает базовую цену 300.
    /// </summary>
    public static decimal Calculate(string service)
    {
        if (string.IsNullOrWhiteSpace(service))
            throw new ArgumentException("Услуга не задана");

        return PriceList.TryGetValue(service.Trim(), out var price) ? price : 300m;
    }

    /// <summary>Получить список доступных услуг.</summary>
    public static IEnumerable<string> GetAvailableServices() => PriceList.Keys;
}
