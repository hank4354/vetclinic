namespace VetClinic;

using System;

/// <summary>
/// Модель собаки: расширяет Animal и добавляет информацию о породе.
/// </summary>
public class Dog : Animal
{
    /// <summary>Порода собаки.</summary>
    public string Breed { get; private set; }

    /// <summary>Обновить породу.</summary>
    public void UpdateBreed(string breed) => Breed = breed ?? "";

    public Dog(int id, string name, DateTime dateOfBirth, string ownerName, string ownerPhone, string breed)
        : base(id, name, dateOfBirth, ownerName, ownerPhone)
    {
        Breed = breed;
    }

    public override string GetTypeName() => "Собака";

    public override string GetDescription() => $"Порода: {Breed}";
}
