namespace VetClinic;

using System;

/// <summary>
/// Модель кошки: расширяет Animal и хранит породу.
/// </summary>
public class Cat : Animal
{
    /// <summary>Порода кошки.</summary>
    public string Breed { get; private set; }

    /// <summary>Обновить породу.</summary>
    public void UpdateBreed(string breed) => Breed = breed ?? "";

    public Cat(int id, string name, DateTime dateOfBirth, string ownerName, string ownerPhone, string breed)
        : base(id, name, dateOfBirth, ownerName, ownerPhone)
    {
        Breed = breed;
    }

    public override string GetTypeName() => "Кошка";

    public override string GetDescription() => $"Порода: {Breed}";
}
