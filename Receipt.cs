namespace VetClinic;

using System;

/// <summary>
/// Квитанция за оказанную услугу на приёме.
/// </summary>
public class Receipt
{
    /// <summary>Номер квитанции.</summary>
    public int Id { get; private set; }

    /// <summary>Связанный визит.</summary>
    public Visit Visit { get; private set; }

    /// <summary>Итоговая стоимость услуги.</summary>
    public decimal TotalPrice { get; private set; }

    /// <summary>Дата формирования квитанции.</summary>
    public DateTime Date { get; private set; }

    public Receipt(int id, Visit visit, decimal price)
    {
        Id = id;
        Visit = visit;
        TotalPrice = price;
        Date = DateTime.Now;
    }

    /// <summary>Подробная строка с данными квитанции.</summary>
    public string GetInfo()
    {
        return $"Квитанция #{Id}\nДата: {Date}\nЖивотное: {Visit.Animal.Name} ({Visit.Animal.GetTypeName()})\nВрач: {Visit.Vet.FullName}\nУслуга: {Visit.Service}\nСтоимость: {TotalPrice} руб.";
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Квитанция #{Id} | {Visit.Animal.Name} | {Visit.Service} | {TotalPrice} руб. | {Date}";
    }
}
