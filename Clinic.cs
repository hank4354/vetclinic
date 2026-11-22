namespace VetClinic;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Центральный сервис приложения: управляет животными, сотрудниками, визитами и квитанциями,
/// выполняет поиск/фильтрацию и хранит счётчики идентификаторов.
/// </summary>
public class Clinic
{
    private readonly List<Animal> animals = new();
    private readonly List<Employee> employees = new();
    private readonly List<Receipt> receipts = new();
    private readonly List<Visit> visits = new();

    private int nextAnimalId = 1;
    private int nextEmployeeId = 1;
    private int nextVisitId = 1;
    private int nextReceiptId = 1;

    // --- Animal management ---
    public int GetNextAnimalId() => nextAnimalId++;
    public int GetNextEmployeeId() => nextEmployeeId++;
    public int GetNextVisitId() => nextVisitId++;
    public int GetNextReceiptId() => nextReceiptId++;

    public void SetNextIds(int animalId, int employeeId, int visitId, int receiptId)
    {
        nextAnimalId = animalId;
        nextEmployeeId = employeeId;
        nextVisitId = visitId;
        nextReceiptId = receiptId;
    }

    public (int animalId, int employeeId, int visitId, int receiptId) GetCurrentIds()
    {
        return (nextAnimalId, nextEmployeeId, nextVisitId, nextReceiptId);
    }

    public void AddVisit(Visit visit)
    {
        if (visit == null) throw new ArgumentNullException(nameof(visit));
        visits.Add(visit);
    }

    public void AddReceipt(Receipt receipt)
    {
        if (receipt == null) throw new ArgumentNullException(nameof(receipt));
        receipts.Add(receipt);
    }

    public void AddAnimal(Animal animal)
    {
        if (animal == null) throw new ArgumentNullException(nameof(animal));
        animals.Add(animal);
    }

    public bool RemoveAnimal(int id)
    {
        var a = animals.FirstOrDefault(x => x.Id == id);
        if (a == null) return false;
        animals.Remove(a);
        return true;
    }

    public Animal? FindAnimal(int id) => animals.FirstOrDefault(x => x.Id == id);

    public IEnumerable<Animal> GetAnimals() => animals.AsReadOnly();

    public bool UpdateAnimal(int id, string? name = null, DateTime? dateOfBirth = null, string? ownerName = null, string? ownerPhone = null)
    {
        var animal = FindAnimal(id);
        if (animal == null) return false;
        
        if (name != null) animal.UpdateName(name);
        if (dateOfBirth.HasValue) animal.UpdateDateOfBirth(dateOfBirth.Value);
        if (ownerName != null) animal.UpdateOwnerName(ownerName);
        if (ownerPhone != null) animal.UpdateOwnerPhone(ownerPhone);
        
        return true;
    }

    public IEnumerable<Animal> SearchAnimals(string? name = null, string? ownerName = null, string? ownerPhone = null)
    {
        var query = animals.AsEnumerable();
        
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(a => a.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        
        if (!string.IsNullOrWhiteSpace(ownerName))
            query = query.Where(a => a.OwnerName.Contains(ownerName, StringComparison.OrdinalIgnoreCase));
        
        if (!string.IsNullOrWhiteSpace(ownerPhone))
            query = query.Where(a => a.OwnerPhone.Contains(ownerPhone, StringComparison.OrdinalIgnoreCase));
        
        return query;
    }

    public IEnumerable<Animal> FilterAnimals(string? type = null, int? minAge = null, int? maxAge = null, string? breed = null)
    {
        var query = animals.AsEnumerable();
        
        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(a => a.GetTypeName().Equals(type, StringComparison.OrdinalIgnoreCase));
        }
        
        if (minAge.HasValue)
            query = query.Where(a => a.GetAge() >= minAge.Value);
        
        if (maxAge.HasValue)
            query = query.Where(a => a.GetAge() <= maxAge.Value);
        
        if (!string.IsNullOrWhiteSpace(breed))
        {
            query = query.Where(a => a.GetDescription().Contains(breed, StringComparison.OrdinalIgnoreCase));
        }
        
        return query;
    }

    // --- Employee management ---
    public void AddEmployee(Employee employee)
    {
        if (employee == null) throw new ArgumentNullException(nameof(employee));
        employees.Add(employee);
    }

    public bool RemoveEmployee(int id)
    {
        var e = employees.FirstOrDefault(x => x.Id == id);
        if (e == null) return false;
        employees.Remove(e);
        return true;
    }

    public Employee? FindEmployee(int id) => employees.FirstOrDefault(x => x.Id == id);

    public IEnumerable<Employee> GetEmployees() => employees.AsReadOnly();

    public bool UpdateEmployee(int id, string? fullName = null, string? phone = null, string? specialization = null)
    {
        var employee = FindEmployee(id);
        if (employee == null) return false;
        
        if (fullName != null) employee.UpdateFullName(fullName);
        if (phone != null) employee.UpdatePhone(phone);
        
        if (specialization != null && employee is Vet vet)
            vet.UpdateSpecialization(specialization);
        
        return true;
    }

    public IEnumerable<Employee> SearchEmployees(string? name = null, string? role = null, string? specialization = null)
    {
        var query = employees.AsEnumerable();
        
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(e => e.FullName.Contains(name, StringComparison.OrdinalIgnoreCase));
        
        if (!string.IsNullOrWhiteSpace(role))
            query = query.Where(e => e.GetRole().Equals(role, StringComparison.OrdinalIgnoreCase));
        
        if (!string.IsNullOrWhiteSpace(specialization))
            query = query.Where(e => e is Vet v && v.Specialization.Contains(specialization, StringComparison.OrdinalIgnoreCase));
        
        return query;
    }

    // --- Visits and receipts ---
    public Visit RegisterVisit(Animal animal, Vet vet, string service)
    {
        if (animal == null) throw new ArgumentNullException(nameof(animal));
        if (vet == null) throw new ArgumentNullException(nameof(vet));
        if (string.IsNullOrWhiteSpace(service)) throw new ArgumentException("Услуга не задана");

        var visit = new Visit(GetNextVisitId(), animal, vet, service);
        visits.Add(visit);

        decimal price = PriceCalculator.Calculate(service);
        var receipt = new Receipt(GetNextReceiptId(), visit, price);
        receipts.Add(receipt);

        return visit;
    }

    public IEnumerable<Visit> GetVisits() => visits.AsReadOnly();
    public IEnumerable<Receipt> GetReceipts() => receipts.AsReadOnly();

    public Receipt? GetLatestReceiptForVisit(Visit visit) => receipts.LastOrDefault(r => r.Visit.Id == visit.Id);

    public Visit? FindVisit(int id) => visits.FirstOrDefault(v => v.Id == id);

    public bool UpdateVisit(int id, string? diagnosis = null, string? treatment = null)
    {
        var visit = FindVisit(id);
        if (visit == null) return false;
        
        if (diagnosis != null) visit.UpdateDiagnosis(diagnosis);
        if (treatment != null) visit.UpdateTreatment(treatment);
        
        return true;
    }

    public IEnumerable<Visit> SearchVisits(DateTime? date = null, int? animalId = null, int? vetId = null)
    {
        var query = visits.AsEnumerable();
        
        if (date.HasValue)
            query = query.Where(v => v.Date.Date == date.Value.Date);
        
        if (animalId.HasValue)
            query = query.Where(v => v.Animal.Id == animalId.Value);
        
        if (vetId.HasValue)
            query = query.Where(v => v.Vet.Id == vetId.Value);
        
        return query;
    }

    public IEnumerable<Visit> GetAnimalVisitHistory(int animalId) => visits.Where(v => v.Animal.Id == animalId).OrderByDescending(v => v.Date);

    public string GetAnimalMedicalCard(int animalId)
    {
        var animal = FindAnimal(animalId);
        if (animal == null) return "Животное не найдено.";
        
        var animalVisits = GetAnimalVisitHistory(animalId).ToList();
        if (!animalVisits.Any()) return $"Медицинская карта пуста для {animal.Name}.";
        
        var card = $"МЕДИЦИНСКАЯ КАРТА\n" +
                   $"Животное: {animal.Name} ({animal.GetTypeName()})\n" +
                   $"Владелец: {animal.OwnerName} ({animal.OwnerPhone})\n" +
                   $"Дата рождения: {animal.DateOfBirth:dd.MM.yyyy}\n" +
                   $"Возраст: {animal.GetAge()} лет\n" +
                   $"Всего визитов: {animalVisits.Count}\n\n" +
                   $"ИСТОРИЯ ВИЗИТОВ:\n";
        
        foreach (var visit in animalVisits)
        {
            card += $"\nВизит #{visit.Id} - {visit.Date:dd.MM.yyyy HH:mm}\n";
            card += $"Врач: {visit.Vet.FullName} ({visit.Vet.Specialization})\n";
            card += $"Услуга: {visit.Service}\n";
            if (!string.IsNullOrWhiteSpace(visit.Diagnosis))
                card += $"Диагноз: {visit.Diagnosis}\n";
            if (!string.IsNullOrWhiteSpace(visit.Treatment))
                card += $"Лечение: {visit.Treatment}\n";
        }
        
        return card;
    }

    public string GetVetWorkHistory(int vetId)
    {
        var vet = FindEmployee(vetId) as Vet;
        if (vet == null) return "Ветеринар не найден.";
        
        var vetVisits = visits.Where(v => v.Vet.Id == vetId).ToList();
        
        var history = $"ИСТОРИЯ РАБОТЫ ВЕТЕРИНАРА\n" +
                      $"Имя: {vet.FullName}\n" +
                      $"Специализация: {vet.Specialization}\n" +
                      $"Телефон: {vet.Phone}\n" +
                      $"Всего визитов: {vetVisits.Count}\n\n";
        
        if (vetVisits.Any())
        {
            history += "ПОСЛЕДНИЕ ВИЗИТЫ:\n";
            foreach (var visit in vetVisits.OrderByDescending(v => v.Date).Take(10))
            {
                history += $"  {visit.Date:dd.MM.yyyy HH:mm} - {visit.Animal.Name} ({visit.Animal.GetTypeName()}) - {visit.Service}\n";
            }
        }
        
        return history;
    }
}
