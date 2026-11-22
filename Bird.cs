namespace VetClinic;

using System;

/// <summary>
/// Модель птицы: расширяет Animal и хранит конкретный вид.
/// </summary>
public class Bird : Animal
{
    /// <summary>Вид птицы (например, волнистый попугай).</summary>
    public string Species { get; private set; }

    /// <summary>Обновить вид.</summary>
    public void UpdateSpecies(string species) => Species = species ?? "";

    public Bird(int id, string name, DateTime dateOfBirth, string ownerName, string ownerPhone, string species)
        : base(id, name, dateOfBirth, ownerName, ownerPhone)
    {
        Species = species;
    }

    public override string GetTypeName() => "Птица";

    public override string GetDescription() => $"Вид: {Species}";
}
