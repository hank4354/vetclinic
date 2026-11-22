namespace VetClinic;

/// <summary>
/// Администратор клиники: базовая роль без доп. полей.
/// </summary>
public class Administrator : Employee
{
    public Administrator(int id, string name, string phone)
        : base(id, name, phone)
    {
    }

    public override string GetRole() => "Администратор";
}
