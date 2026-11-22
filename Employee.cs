namespace VetClinic;

/// <summary>
/// Абстрактный сотрудник клиники. Базовый класс для всех ролей.
/// </summary>
public abstract class Employee
{
    /// <summary>Уникальный идентификатор сотрудника.</summary>
    public int Id { get; protected set; }

    /// <summary>ФИО сотрудника.</summary>
    public string FullName { get; protected set; }

    /// <summary>Контактный телефон.</summary>
    public string Phone { get; protected set; }

    /// <summary>Обновить ФИО.</summary>
    public void UpdateFullName(string fullName) => FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));

    /// <summary>Обновить телефон.</summary>
    public void UpdatePhone(string phone) => Phone = phone ?? throw new ArgumentNullException(nameof(phone));

    protected Employee(int id, string fullName, string phone)
    {
        Id = id;
        FullName = fullName;
        Phone = phone;
    }

    /// <summary>Роль сотрудника (ветеринар/администратор).</summary>
    public abstract string GetRole();

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Id}: {GetRole()} | {FullName} | {Phone}";
    }
}
