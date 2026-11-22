namespace VetClinic;

/// <summary>
/// Ветеринар: сотрудник с указанием специализации.
/// </summary>
public class Vet : Employee
{
    /// <summary>Специализация ветеринара.</summary>
    public string Specialization { get; private set; }

    /// <summary>Обновить специализацию.</summary>
    public void UpdateSpecialization(string specialization) => Specialization = specialization ?? "";

    public Vet(int id, string name, string phone, string specialization)
        : base(id, name, phone)
    {
        Specialization = specialization;
    }

    public override string GetRole() => "Ветеринар";
}
