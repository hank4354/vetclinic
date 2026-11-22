namespace VetClinic;

using System;

/// <summary>
/// Базовый абстрактный класс для всех животных в клинике.
/// Содержит общие атрибуты, необходимые для учёта и идентификации.
/// </summary>
public abstract class Animal
{
    /// <summary>Уникальный идентификатор животного.</summary>
    public int Id { get; private set; }

    /// <summary>Кличка животного.</summary>
    public string Name { get; protected set; }

    /// <summary>Дата рождения животного.</summary>
    public DateTime DateOfBirth { get; protected set; }

    /// <summary>ФИО владельца.</summary>
    public string OwnerName { get; protected set; }

    /// <summary>Контактный телефон владельца.</summary>
    public string OwnerPhone { get; protected set; }

    /// <summary>Обновить кличку.</summary>
    public void UpdateName(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

    /// <summary>Обновить дату рождения.</summary>
    public void UpdateDateOfBirth(DateTime dateOfBirth) => DateOfBirth = dateOfBirth;

    /// <summary>Обновить ФИО владельца.</summary>
    public void UpdateOwnerName(string ownerName) => OwnerName = ownerName ?? throw new ArgumentNullException(nameof(ownerName));

    /// <summary>Обновить телефон владельца.</summary>
    public void UpdateOwnerPhone(string ownerPhone) => OwnerPhone = ownerPhone ?? throw new ArgumentNullException(nameof(ownerPhone));

    /// <summary>
    /// Базовый конструктор животного.
    /// </summary>
    protected Animal(int id, string name, DateTime dateOfBirth, string ownerName, string ownerPhone)
    {
        Id = id;
        Name = name;
        DateOfBirth = dateOfBirth;
        OwnerName = ownerName;
        OwnerPhone = ownerPhone;
    }

    /// <summary>Рассчитать полный возраст на текущую дату.</summary>
    public int GetAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }

    /// <summary>Текстовое обозначение типа животного (собака/кошка/птица).</summary>
    public abstract string GetTypeName();

    /// <summary>Описание животного (порода/вид и др.).</summary>
    public abstract string GetDescription();

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Id}: {GetTypeName()} | {Name} | Дата рождения: {DateOfBirth:dd.MM.yyyy} (возраст: {GetAge()}) | {GetDescription()} | Владелец: {OwnerName} ({OwnerPhone})";
    }
}
