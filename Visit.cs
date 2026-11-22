namespace VetClinic;

using System;

/// <summary>
/// Визит (приём) в клинике: содержит животное, ветеринара, услугу, диагноз и лечение.
/// </summary>
public class Visit
{
    /// <summary>Уникальный номер визита.</summary>
    public int Id { get; private set; }

    /// <summary>Животное, которому оказывается услуга.</summary>
    public Animal Animal { get; private set; }

    /// <summary>Ветеринар, который проводит приём.</summary>
    public Vet Vet { get; private set; }

    /// <summary>Дата и время визита.</summary>
    public DateTime Date { get; private set; }

    /// <summary>Название услуги (например, осмотр, вакцинация).</summary>
    public string Service { get; private set; }

    /// <summary>Диагноз, установленный по результатам визита.</summary>
    public string Diagnosis { get; private set; } = "";

    /// <summary>Назначенное лечение.</summary>
    public string Treatment { get; private set; } = "";

    /// <summary>Установить дату (используется при загрузке данных).</summary>
    public void SetDate(DateTime date) => Date = date;

    /// <summary>Обновить диагноз.</summary>
    public void UpdateDiagnosis(string diagnosis) => Diagnosis = diagnosis ?? "";

    /// <summary>Обновить лечение.</summary>
    public void UpdateTreatment(string treatment) => Treatment = treatment ?? "";

    public Visit(int id, Animal animal, Vet vet, string service)
    {
        Id = id;
        Animal = animal;
        Vet = vet;
        Service = service;
        Date = DateTime.Now;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Приём #{Id} | Животное: {Animal.Name} ({Animal.GetTypeName()}) | Врач: {Vet.FullName} | Услуга: {Service} | Дата: {Date}";
    }
}
