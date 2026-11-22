namespace VetClinic;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

/// <summary>
/// DTO-обёртка для сериализации состояния клиники.
/// </summary>
public class ClinicData
{
    public List<AnimalData> Animals { get; set; } = new();
    public List<EmployeeData> Employees { get; set; } = new();
    public List<VisitData> Visits { get; set; } = new();
    public List<ReceiptData> Receipts { get; set; } = new();
    public int NextAnimalId { get; set; } = 1;
    public int NextEmployeeId { get; set; } = 1;
    public int NextVisitId { get; set; } = 1;
    public int NextReceiptId { get; set; } = 1;
}

public class AnimalData
{
    public string Type { get; set; } = "";
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public string OwnerName { get; set; } = "";
    public string OwnerPhone { get; set; } = "";
    public string BreedOrSpecies { get; set; } = "";
}

public class EmployeeData
{
    public string Type { get; set; } = "";
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Specialization { get; set; } = "";
}

public class VisitData
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public int VetId { get; set; }
    public DateTime Date { get; set; }
    public string Service { get; set; } = "";
    public string Diagnosis { get; set; } = "";
    public string Treatment { get; set; } = "";
}

public class ReceiptData
{
    public int Id { get; set; }
    public int VisitId { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime Date { get; set; }
}

/// <summary>
/// Класс для сохранения и загрузки данных клиники в/из JSON.
/// </summary>
public static class DataManager
{
    private const string DataFileName = "clinic_data.json";

    public static void SaveClinic(Clinic clinic)
    {
        try
        {
            // Получаем текущие счетчики
            var (animalId, employeeId, visitId, receiptId) = clinic.GetCurrentIds();

            var data = new ClinicData
            {
                NextAnimalId = animalId,
                NextEmployeeId = employeeId,
                NextVisitId = visitId,
                NextReceiptId = receiptId
            };

            // Сохраняем животных
            foreach (var animal in clinic.GetAnimals())
            {
                var animalData = new AnimalData
                {
                    Id = animal.Id,
                    Name = animal.Name,
                    DateOfBirth = animal.DateOfBirth,
                    OwnerName = animal.OwnerName,
                    OwnerPhone = animal.OwnerPhone,
                    Type = animal.GetTypeName()
                };

                if (animal is Dog dog)
                {
                    animalData.BreedOrSpecies = dog.Breed;
                }
                else if (animal is Cat cat)
                {
                    animalData.BreedOrSpecies = cat.Breed;
                }
                else if (animal is Bird bird)
                {
                    animalData.BreedOrSpecies = bird.Species;
                }

                data.Animals.Add(animalData);
            }

            // Сохраняем сотрудников
            foreach (var employee in clinic.GetEmployees())
            {
                var employeeData = new EmployeeData
                {
                    Id = employee.Id,
                    FullName = employee.FullName,
                    Phone = employee.Phone,
                    Type = employee.GetRole()
                };

                if (employee is Vet vet)
                {
                    employeeData.Specialization = vet.Specialization;
                }

                data.Employees.Add(employeeData);
            }

            // Сохраняем визиты
            foreach (var visit in clinic.GetVisits())
            {
                data.Visits.Add(new VisitData
                {
                    Id = visit.Id,
                    AnimalId = visit.Animal.Id,
                    VetId = visit.Vet.Id,
                    Date = visit.Date,
                    Service = visit.Service,
                    Diagnosis = visit.Diagnosis,
                    Treatment = visit.Treatment
                });
            }

            // Сохраняем квитанции
            foreach (var receipt in clinic.GetReceipts())
            {
                data.Receipts.Add(new ReceiptData
                {
                    Id = receipt.Id,
                    VisitId = receipt.Visit.Id,
                    TotalPrice = receipt.TotalPrice,
                    Date = receipt.Date
                });
            }

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(DataFileName, json);
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка сохранения данных: {ex.Message}", ex);
        }
    }

    public static void LoadClinic(Clinic clinic)
    {
        if (!File.Exists(DataFileName)) return;

        try
        {
            var json = File.ReadAllText(DataFileName);
            var data = JsonSerializer.Deserialize<ClinicData>(json);
            if (data == null) return;

            // Восстанавливаем животных
            foreach (var animalData in data.Animals)
            {
                Animal animal = animalData.Type switch
                {
                    "Собака" => new Dog(animalData.Id, animalData.Name, animalData.DateOfBirth, 
                                       animalData.OwnerName, animalData.OwnerPhone, animalData.BreedOrSpecies),
                    "Кошка" => new Cat(animalData.Id, animalData.Name, animalData.DateOfBirth, 
                                      animalData.OwnerName, animalData.OwnerPhone, animalData.BreedOrSpecies),
                    "Птица" => new Bird(animalData.Id, animalData.Name, animalData.DateOfBirth, 
                                       animalData.OwnerName, animalData.OwnerPhone, animalData.BreedOrSpecies),
                    _ => throw new Exception($"Неизвестный тип животного: {animalData.Type}")
                };
                clinic.AddAnimal(animal);
            }

            // Восстанавливаем сотрудников
            foreach (var employeeData in data.Employees)
            {
                Employee employee = employeeData.Type switch
                {
                    "Ветеринар" => new Vet(employeeData.Id, employeeData.FullName, employeeData.Phone, employeeData.Specialization),
                    "Администратор" => new Administrator(employeeData.Id, employeeData.FullName, employeeData.Phone),
                    _ => throw new Exception($"Неизвестный тип сотрудника: {employeeData.Type}")
                };
                clinic.AddEmployee(employee);
            }

            // Восстанавливаем визиты
            foreach (var visitData in data.Visits)
            {
                var animal = clinic.FindAnimal(visitData.AnimalId);
                var vet = clinic.FindEmployee(visitData.VetId) as Vet;
                if (animal == null || vet == null) continue;

                var visit = new Visit(visitData.Id, animal, vet, visitData.Service);
                visit.SetDate(visitData.Date);
                visit.UpdateDiagnosis(visitData.Diagnosis);
                visit.UpdateTreatment(visitData.Treatment);
                clinic.AddVisit(visit);
            }

            // Восстанавливаем квитанции
            foreach (var receiptData in data.Receipts)
            {
                var visit = clinic.FindVisit(receiptData.VisitId);
                if (visit == null) continue;

                var receipt = new Receipt(receiptData.Id, visit, receiptData.TotalPrice);
                clinic.AddReceipt(receipt);
            }

            // Устанавливаем счетчики ID
            clinic.SetNextIds(data.NextAnimalId, data.NextEmployeeId, data.NextVisitId, data.NextReceiptId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка загрузки данных: {ex.Message}", ex);
        }
    }
}

