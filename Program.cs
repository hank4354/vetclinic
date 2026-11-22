namespace VetClinic;

using System;
using System.Linq;

class Program
{
    static Clinic clinic = new Clinic();

    static void Main()
    {
        ConsoleColors.WriteLineInfo("Загрузка данных...");
        try
        {
            DataManager.LoadClinic(clinic);
            ConsoleColors.WriteLineSuccess("Данные загружены успешно.");
        }
        catch (Exception ex)
        {
            ConsoleColors.WriteLineWarning($"Не удалось загрузить данные: {ex.Message}");
            ConsoleColors.WriteLineInfo("Загружаются демо-данные...");
            SeedDemoData();
        }

        while (true)
        {
            PrintMainMenu();
            Console.Write("Выбор: ");
            var key = Console.ReadLine();

            Console.WriteLine();
            switch (key)
            {
                case "1": ListAnimals(); break;
                case "2": AddAnimalInteractive(); break;
                case "3": RemoveAnimalInteractive(); break;
                case "4": EditAnimalInteractive(); break;
                case "5": SearchAnimalsInteractive(); break;
                case "6": FilterAnimalsInteractive(); break;
                case "7": ListEmployees(); break;
                case "8": AddEmployeeInteractive(); break;
                case "9": RemoveEmployeeInteractive(); break;
                case "10": EditEmployeeInteractive(); break;
                case "11": SearchEmployeesInteractive(); break;
                case "12": RegisterVisitInteractive(); break;
                case "13": EditVisitInteractive(); break;
                case "14": SearchVisitsInteractive(); break;
                case "15": AnimalVisitHistoryInteractive(); break;
                case "16": AnimalMedicalCardInteractive(); break;
                case "17": VetWorkHistoryInteractive(); break;
                case "18": ListReceipts(); break;
                case "0": 
                    ConsoleColors.WriteLineInfo("Сохранение данных...");
                    try
                    {
                        DataManager.SaveClinic(clinic);
                        ConsoleColors.WriteLineSuccess("Данные сохранены успешно.");
                    }
                    catch (Exception ex)
                    {
                        ConsoleColors.WriteLineError($"Ошибка сохранения: {ex.Message}");
                    }
                    Console.WriteLine("Выход..."); 
                    return;
                default: ConsoleColors.WriteLineWarning("Неверный выбор. Попробуйте ещё."); break;
            }

            Console.WriteLine("\nНажмите Enter для продолжения...");
            Console.ReadLine();
            Console.Clear();
        }
    }

    static void PrintMainMenu()
    {
        ConsoleColors.WriteHeader("===========================");
        ConsoleColors.WriteHeader(" ВЕТЕРИНАРНАЯ КЛИНИКА - МЕНЮ");
        ConsoleColors.WriteHeader("===========================");
        Console.WriteLine("ЖИВОТНЫЕ:");
        Console.WriteLine("  1. Показать животных");
        Console.WriteLine("  2. Добавить животное");
        Console.WriteLine("  3. Удалить животное");
        Console.WriteLine("  4. Редактировать животное");
        Console.WriteLine("  5. Поиск животных");
        Console.WriteLine("  6. Фильтрация животных");
        Console.WriteLine();
        Console.WriteLine("СОТРУДНИКИ:");
        Console.WriteLine("  7. Показать сотрудников");
        Console.WriteLine("  8. Добавить сотрудника");
        Console.WriteLine("  9. Удалить сотрудника");
        Console.WriteLine("  10. Редактировать сотрудника");
        Console.WriteLine("  11. Поиск сотрудников");
        Console.WriteLine();
        Console.WriteLine("ВИЗИТЫ И КАРТЫ:");
        Console.WriteLine("  12. Оформить приём");
        Console.WriteLine("  13. Редактировать визит");
        Console.WriteLine("  14. Поиск визитов");
        Console.WriteLine("  15. История визитов животного");
        Console.WriteLine("  16. Медицинская карта животного");
        Console.WriteLine("  17. История работы ветеринара");
        Console.WriteLine("  18. Показать квитанции");
        Console.WriteLine();
        Console.WriteLine("  0. Выход");
        ConsoleColors.WriteHeader("===========================");
    }

    static void SeedDemoData()
    {
        // сотрудники
        clinic.AddEmployee(new Vet(clinic.GetNextEmployeeId(), "Иван Петров", "89000000000", "Хирург"));
        clinic.AddEmployee(new Vet(clinic.GetNextEmployeeId(), "Мария Соколова", "89001112233", "Терапевт"));
        clinic.AddEmployee(new Administrator(clinic.GetNextEmployeeId(), "Анна Смирнова", "89110000000"));

        // животные
        clinic.AddAnimal(new Dog(clinic.GetNextAnimalId(), "Рекс", new DateTime(2020, 3, 15), "Петров Иван Сергеевич", "89001234567", "Овчарка"));
        clinic.AddAnimal(new Cat(clinic.GetNextAnimalId(), "Мурка", new DateTime(2022, 7, 20), "Смирнова Анна Владимировна", "89007654321", "Британская"));
        clinic.AddAnimal(new Bird(clinic.GetNextAnimalId(), "Кеша", new DateTime(2023, 1, 10), "Иванов Петр Александрович", "89005555555", "Волнистый попугай"));
    }

    static DateTime? ParseDateOfBirth(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        
        // Попробуем разные форматы
        string[] formats = { "dd.MM.yyyy", "dd/MM/yyyy", "dd-MM-yyyy", "d.M.yyyy", "d/M/yyyy", "d-M-yyyy" };
        
        if (DateTime.TryParseExact(input, formats, null, System.Globalization.DateTimeStyles.None, out var date))
        {
            if (date > DateTime.Today)
            {
                Console.WriteLine("Дата рождения не может быть в будущем.");
                return null;
            }
            if (date < DateTime.Today.AddYears(-50))
            {
                Console.WriteLine("Дата рождения слишком давняя (более 50 лет назад).");
                return null;
            }
            return date;
        }
        
        Console.WriteLine("Некорректный формат даты. Используйте формат: дд.мм.гггг (например, 15.03.2020)");
        return null;
    }

    static bool CheckForBack(string? input)
    {
        if (input?.Trim().ToLower() == "0" || input?.Trim().ToLower() == "назад")
        {
            Console.WriteLine("Возврат в главное меню...");
            return true;
        }
        return false;
    }

    // -----------------------------
    // Animals
    // -----------------------------
    static void ListAnimals()
    {
        Console.WriteLine("СПИСОК ЖИВОТНЫХ:");
        var list = clinic.GetAnimals().ToList();
        if (!list.Any()) { Console.WriteLine("Список пуст."); return; }
        foreach (var a in list) Console.WriteLine(a);
    }

    static void AddAnimalInteractive()
    {
        Console.WriteLine("Добавление животного. (Введите '0' или 'назад' в любой момент для возврата в меню)");
        Console.WriteLine();
        Console.WriteLine("Выберите тип: 1 - Собака, 2 - Кошка, 3 - Птица");
        Console.Write("Тип: ");
        var t = Console.ReadLine();
        if (CheckForBack(t)) return;

        Console.Write("Имя животного: ");
        var name = Console.ReadLine();
        if (CheckForBack(name)) return;
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Имя не может быть пустым."); return; }

        Console.Write("Дата рождения (формат: дд.мм.гггг, например 15.03.2020): ");
        var dateInput = Console.ReadLine();
        if (CheckForBack(dateInput)) return;
        var dateOfBirth = ParseDateOfBirth(dateInput);
        if (!dateOfBirth.HasValue) return;

        string? breedOrSpecies = null;
        switch (t)
        {
            case "1":
                Console.Write("Порода: ");
                breedOrSpecies = Console.ReadLine();
                if (CheckForBack(breedOrSpecies)) return;
                break;
            case "2":
                Console.Write("Порода: ");
                breedOrSpecies = Console.ReadLine();
                if (CheckForBack(breedOrSpecies)) return;
                break;
            case "3":
                Console.Write("Вид птицы (например: Волнистый попугай, Канарейка, Воробей): ");
                breedOrSpecies = Console.ReadLine();
                if (CheckForBack(breedOrSpecies)) return;
                break;
            default:
                Console.WriteLine("Неизвестный тип.");
                return;
        }

        Console.Write("ФИО владельца: ");
        var ownerName = Console.ReadLine();
        if (CheckForBack(ownerName)) return;
        if (string.IsNullOrWhiteSpace(ownerName)) { Console.WriteLine("ФИО владельца не может быть пустым."); return; }

        Console.Write("Номер телефона владельца: ");
        var ownerPhone = Console.ReadLine();
        if (CheckForBack(ownerPhone)) return;
        if (string.IsNullOrWhiteSpace(ownerPhone)) { Console.WriteLine("Номер телефона не может быть пустым."); return; }

        int id = clinic.GetNextAnimalId();

        switch (t)
        {
            case "1":
                clinic.AddAnimal(new Dog(id, name, dateOfBirth.Value, ownerName, ownerPhone, breedOrSpecies ?? ""));
                ConsoleColors.WriteLineSuccess("Собака добавлена.");
                AutoSave();
                break;
            case "2":
                clinic.AddAnimal(new Cat(id, name, dateOfBirth.Value, ownerName, ownerPhone, breedOrSpecies ?? ""));
                ConsoleColors.WriteLineSuccess("Кошка добавлена.");
                AutoSave();
                break;
            case "3":
                clinic.AddAnimal(new Bird(id, name, dateOfBirth.Value, ownerName, ownerPhone, breedOrSpecies ?? ""));
                ConsoleColors.WriteLineSuccess("Птица добавлена.");
                AutoSave();
                break;
        }
    }

    static void RemoveAnimalInteractive()
    {
        Console.WriteLine("Удаление животного. (Введите '0' или 'назад' для возврата в меню)");
        Console.WriteLine();
        var list = clinic.GetAnimals().ToList();
        if (!list.Any()) 
        { 
            Console.WriteLine("Список пуст."); 
            return; 
        }
        
        Console.WriteLine("СПИСОК ЖИВОТНЫХ:");
        foreach (var a in list) Console.WriteLine(a);
        
        Console.Write("\nВведите ID животного для удаления: ");
        var input = Console.ReadLine();
        if (CheckForBack(input)) return;
        
        if (!int.TryParse(input, out var id)) 
        { 
            Console.WriteLine("Некорректный ID."); 
            return; 
        }
        
        var animal = clinic.FindAnimal(id);
        if (animal == null)
        {
            ConsoleColors.WriteLineError("Животное не найдено.");
            return;
        }

        ConsoleColors.WriteLineWarning($"Вы собираетесь удалить: {animal.Name} ({animal.GetTypeName()})");
        Console.Write("Подтвердите удаление (да/нет): ");
        var confirm = Console.ReadLine();
        if (confirm?.Trim().ToLower() != "да")
        {
            ConsoleColors.WriteLineInfo("Удаление отменено.");
            return;
        }

        if (clinic.RemoveAnimal(id))
        {
            ConsoleColors.WriteLineSuccess("Животное удалено.");
            AutoSave();
        }
        else
        {
            ConsoleColors.WriteLineError("Ошибка при удалении.");
        }
    }

    static void EditAnimalInteractive()
    {
        Console.WriteLine("Редактирование животного. (Введите '0' или 'назад' для возврата в меню)");
        Console.WriteLine();
        var list = clinic.GetAnimals().ToList();
        if (!list.Any())
        {
            ConsoleColors.WriteLineWarning("Список пуст.");
            return;
        }

        Console.WriteLine("СПИСОК ЖИВОТНЫХ:");
        foreach (var a in list) Console.WriteLine(a);

        Console.Write("\nВведите ID животного для редактирования: ");
        var input = Console.ReadLine();
        if (CheckForBack(input)) return;
        if (!int.TryParse(input, out var id))
        {
            ConsoleColors.WriteLineError("Некорректный ID.");
            return;
        }

        var animal = clinic.FindAnimal(id);
        if (animal == null)
        {
            ConsoleColors.WriteLineError("Животное не найдено.");
            return;
        }

        Console.WriteLine($"\nТекущие данные: {animal}");
        Console.WriteLine("\nВведите новые данные (оставьте пустым, чтобы не изменять):");

        Console.Write("Имя: ");
        var name = Console.ReadLine();
        if (CheckForBack(name)) return;

        Console.Write("Дата рождения (формат: дд.мм.гггг): ");
        var dateInput = Console.ReadLine();
        if (CheckForBack(dateInput)) return;
        DateTime? dateOfBirth = null;
        if (!string.IsNullOrWhiteSpace(dateInput))
        {
            dateOfBirth = ParseDateOfBirth(dateInput);
            if (!dateOfBirth.HasValue) return;
        }

        Console.Write("ФИО владельца: ");
        var ownerName = Console.ReadLine();
        if (CheckForBack(ownerName)) return;

        Console.Write("Номер телефона владельца: ");
        var ownerPhone = Console.ReadLine();
        if (CheckForBack(ownerPhone)) return;

        string? breedOrSpecies = null;
        if (animal is Dog dog)
        {
            Console.Write("Порода: ");
            breedOrSpecies = Console.ReadLine();
            if (CheckForBack(breedOrSpecies)) return;
            if (!string.IsNullOrWhiteSpace(breedOrSpecies)) dog.UpdateBreed(breedOrSpecies);
        }
        else if (animal is Cat cat)
        {
            Console.Write("Порода: ");
            breedOrSpecies = Console.ReadLine();
            if (CheckForBack(breedOrSpecies)) return;
            if (!string.IsNullOrWhiteSpace(breedOrSpecies)) cat.UpdateBreed(breedOrSpecies);
        }
        else if (animal is Bird bird)
        {
            Console.Write("Вид птицы: ");
            breedOrSpecies = Console.ReadLine();
            if (CheckForBack(breedOrSpecies)) return;
            if (!string.IsNullOrWhiteSpace(breedOrSpecies)) bird.UpdateSpecies(breedOrSpecies);
        }

        if (clinic.UpdateAnimal(id, 
            string.IsNullOrWhiteSpace(name) ? null : name,
            dateOfBirth,
            string.IsNullOrWhiteSpace(ownerName) ? null : ownerName,
            string.IsNullOrWhiteSpace(ownerPhone) ? null : ownerPhone))
        {
            ConsoleColors.WriteLineSuccess("Данные животного обновлены.");
            AutoSave();
        }
        else
        {
            ConsoleColors.WriteLineError("Ошибка при обновлении.");
        }
    }

    static void SearchAnimalsInteractive()
    {
        Console.WriteLine("Поиск животных. (Введите '0' или 'назад' для возврата в меню)");
        Console.WriteLine();
        Console.WriteLine("Введите критерии поиска (оставьте пустым, чтобы пропустить):");

        Console.Write("Имя животного: ");
        var name = Console.ReadLine();
        if (CheckForBack(name)) return;

        Console.Write("ФИО владельца: ");
        var ownerName = Console.ReadLine();
        if (CheckForBack(ownerName)) return;

        Console.Write("Номер телефона владельца: ");
        var ownerPhone = Console.ReadLine();
        if (CheckForBack(ownerPhone)) return;

        var results = clinic.SearchAnimals(
            string.IsNullOrWhiteSpace(name) ? null : name,
            string.IsNullOrWhiteSpace(ownerName) ? null : ownerName,
            string.IsNullOrWhiteSpace(ownerPhone) ? null : ownerPhone
        ).ToList();

        Console.WriteLine($"\nНайдено животных: {results.Count}");
        if (results.Any())
        {
            foreach (var a in results) Console.WriteLine(a);
        }
        else
        {
            ConsoleColors.WriteLineWarning("Животные не найдены.");
        }
    }

    static void FilterAnimalsInteractive()
    {
        Console.WriteLine("Фильтрация животных. (Введите '0' или 'назад' для возврата в меню)");
        Console.WriteLine();
        Console.WriteLine("Введите критерии фильтрации (оставьте пустым, чтобы пропустить):");

        Console.WriteLine("Тип (Собака/Кошка/Птица): ");
        var type = Console.ReadLine();
        if (CheckForBack(type)) return;

        Console.Write("Минимальный возраст: ");
        var minAgeInput = Console.ReadLine();
        if (CheckForBack(minAgeInput)) return;
        int? minAge = null;
        if (!string.IsNullOrWhiteSpace(minAgeInput) && int.TryParse(minAgeInput, out var min))
            minAge = min;

        Console.Write("Максимальный возраст: ");
        var maxAgeInput = Console.ReadLine();
        if (CheckForBack(maxAgeInput)) return;
        int? maxAge = null;
        if (!string.IsNullOrWhiteSpace(maxAgeInput) && int.TryParse(maxAgeInput, out var max))
            maxAge = max;

        Console.Write("Порода/Вид: ");
        var breed = Console.ReadLine();
        if (CheckForBack(breed)) return;

        var results = clinic.FilterAnimals(
            string.IsNullOrWhiteSpace(type) ? null : type,
            minAge,
            maxAge,
            string.IsNullOrWhiteSpace(breed) ? null : breed
        ).ToList();

        Console.WriteLine($"\nНайдено животных: {results.Count}");
        if (results.Any())
        {
            foreach (var a in results) Console.WriteLine(a);
        }
        else
        {
            ConsoleColors.WriteLineWarning("Животные не найдены.");
        }
    }

    // -----------------------------
    // Employees
    // -----------------------------
    static void ListEmployees()
    {
        Console.WriteLine("СПИСОК СОТРУДНИКОВ:");
        var list = clinic.GetEmployees().ToList();
        if (!list.Any()) { Console.WriteLine("Список пуст."); return; }
        foreach (var e in list) Console.WriteLine(e);
    }

    static void AddEmployeeInteractive()
    {
        Console.WriteLine("Добавление сотрудника. (Введите '0' или 'назад' в любой момент для возврата в меню)");
        Console.WriteLine();
        Console.WriteLine("Выберите тип: 1 - Ветеринар, 2 - Администратор");
        Console.Write("Тип: ");
        var t = Console.ReadLine();
        if (CheckForBack(t)) return;

        Console.Write("ФИО: ");
        var name = Console.ReadLine();
        if (CheckForBack(name)) return;
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Имя не может быть пустым."); return; }

        Console.Write("Телефон: ");
        var phone = Console.ReadLine();
        if (CheckForBack(phone)) return;

        int id = clinic.GetNextEmployeeId();

        switch (t)
        {
            case "1":
                Console.Write("Специализация: ");
                var spec = Console.ReadLine();
                if (CheckForBack(spec)) return;
                clinic.AddEmployee(new Vet(id, name, phone ?? "", spec ?? ""));
                ConsoleColors.WriteLineSuccess("Ветеринар добавлен.");
                AutoSave();
                break;
            case "2":
                clinic.AddEmployee(new Administrator(id, name, phone ?? ""));
                ConsoleColors.WriteLineSuccess("Администратор добавлен.");
                AutoSave();
                break;
            default:
                Console.WriteLine("Неизвестный тип.");
                break;
        }
    }

    static void RemoveEmployeeInteractive()
    {
        Console.WriteLine("Удаление сотрудника. (Введите '0' или 'назад' для возврата в меню)");
        Console.WriteLine();
        var list = clinic.GetEmployees().ToList();
        if (!list.Any()) 
        { 
            Console.WriteLine("Список пуст."); 
            return; 
        }
        
        Console.WriteLine("СПИСОК СОТРУДНИКОВ:");
        foreach (var e in list) Console.WriteLine(e);
        
        Console.Write("\nВведите ID сотрудника для удаления: ");
        var input = Console.ReadLine();
        if (CheckForBack(input)) return;
        
        if (!int.TryParse(input, out var id)) 
        { 
            Console.WriteLine("Некорректный ID."); 
            return; 
        }
        
        var employee = clinic.FindEmployee(id);
        if (employee == null)
        {
            ConsoleColors.WriteLineError("Сотрудник не найден.");
            return;
        }

        ConsoleColors.WriteLineWarning($"Вы собираетесь удалить: {employee.FullName} ({employee.GetRole()})");
        Console.Write("Подтвердите удаление (да/нет): ");
        var confirm = Console.ReadLine();
        if (confirm?.Trim().ToLower() != "да")
        {
            ConsoleColors.WriteLineInfo("Удаление отменено.");
            return;
        }

        if (clinic.RemoveEmployee(id))
        {
            ConsoleColors.WriteLineSuccess("Сотрудник удалён.");
            AutoSave();
        }
        else
        {
            ConsoleColors.WriteLineError("Ошибка при удалении.");
        }
    }

    static void EditEmployeeInteractive()
    {
        Console.WriteLine("Редактирование сотрудника. (Введите '0' или 'назад' для возврата в меню)");
        Console.WriteLine();
        var list = clinic.GetEmployees().ToList();
        if (!list.Any())
        {
            ConsoleColors.WriteLineWarning("Список пуст.");
            return;
        }

        Console.WriteLine("СПИСОК СОТРУДНИКОВ:");
        foreach (var e in list) Console.WriteLine(e);

        Console.Write("\nВведите ID сотрудника для редактирования: ");
        var input = Console.ReadLine();
        if (CheckForBack(input)) return;
        if (!int.TryParse(input, out var id))
        {
            ConsoleColors.WriteLineError("Некорректный ID.");
            return;
        }

        var employee = clinic.FindEmployee(id);
        if (employee == null)
        {
            ConsoleColors.WriteLineError("Сотрудник не найден.");
            return;
        }

        Console.WriteLine($"\nТекущие данные: {employee}");
        Console.WriteLine("\nВведите новые данные (оставьте пустым, чтобы не изменять):");

        Console.Write("ФИО: ");
        var fullName = Console.ReadLine();
        if (CheckForBack(fullName)) return;

        Console.Write("Телефон: ");
        var phone = Console.ReadLine();
        if (CheckForBack(phone)) return;

        string? specialization = null;
        if (employee is Vet vet)
        {
            Console.Write("Специализация: ");
            specialization = Console.ReadLine();
            if (CheckForBack(specialization)) return;
        }

        if (clinic.UpdateEmployee(id,
            string.IsNullOrWhiteSpace(fullName) ? null : fullName,
            string.IsNullOrWhiteSpace(phone) ? null : phone,
            string.IsNullOrWhiteSpace(specialization) ? null : specialization))
        {
            ConsoleColors.WriteLineSuccess("Данные сотрудника обновлены.");
            AutoSave();
        }
        else
        {
            ConsoleColors.WriteLineError("Ошибка при обновлении.");
        }
    }

    static void SearchEmployeesInteractive()
    {
        Console.WriteLine("Поиск сотрудников. (Введите '0' или 'назад' для возврата в меню)");
        Console.WriteLine();
        Console.WriteLine("Введите критерии поиска (оставьте пустым, чтобы пропустить):");

        Console.Write("Имя: ");
        var name = Console.ReadLine();
        if (CheckForBack(name)) return;

        Console.Write("Должность (Ветеринар/Администратор): ");
        var role = Console.ReadLine();
        if (CheckForBack(role)) return;

        Console.Write("Специализация (только для ветеринаров): ");
        var specialization = Console.ReadLine();
        if (CheckForBack(specialization)) return;

        var results = clinic.SearchEmployees(
            string.IsNullOrWhiteSpace(name) ? null : name,
            string.IsNullOrWhiteSpace(role) ? null : role,
            string.IsNullOrWhiteSpace(specialization) ? null : specialization
        ).ToList();

        Console.WriteLine($"\nНайдено сотрудников: {results.Count}");
        if (results.Any())
        {
            foreach (var e in results) Console.WriteLine(e);
        }
        else
        {
            ConsoleColors.WriteLineWarning("Сотрудники не найдены.");
        }
    }

    // -----------------------------
    // Visits / Receipts
    // -----------------------------
    static void RegisterVisitInteractive()
    {
        Console.WriteLine("Оформление приёма. (Введите '0' или 'назад' в любой момент для возврата в меню)");
        Console.WriteLine();

        var animals = clinic.GetAnimals().ToList();
        if (!animals.Any()) { Console.WriteLine("Нет животных в клинике."); return; }
        Console.WriteLine("Выберите животное по ID:");
        foreach (var a in animals) Console.WriteLine(a);
        Console.Write("ID животного: ");
        var animalInput = Console.ReadLine();
        if (CheckForBack(animalInput)) return;
        if (!int.TryParse(animalInput, out var animalId)) { Console.WriteLine("Некорректный ID."); return; }
        var animal = clinic.FindAnimal(animalId);
        if (animal == null) { Console.WriteLine("Животное не найдено."); return; }

        var vets = clinic.GetEmployees().OfType<Vet>().ToList();
        if (!vets.Any()) { Console.WriteLine("Нет ветеринаров в клинике."); return; }
        Console.WriteLine("Выберите ветеринара по ID:");
        foreach (var v in vets) Console.WriteLine(v);
        Console.Write("ID ветеринара: ");
        var vetInput = Console.ReadLine();
        if (CheckForBack(vetInput)) return;
        if (!int.TryParse(vetInput, out var vetId)) { Console.WriteLine("Некорректный ID."); return; }
        var vet = clinic.FindEmployee(vetId) as Vet;
        if (vet == null) { Console.WriteLine("Ветеринар не найден."); return; }

        Console.WriteLine("Доступные услуги:");
        foreach (var s in PriceCalculator.GetAvailableServices()) Console.WriteLine("- " + s);
        Console.Write("Введите название услуги (например, осмотр): ");
        var service = Console.ReadLine();
        if (CheckForBack(service)) return;
        if (string.IsNullOrWhiteSpace(service)) { Console.WriteLine("Услуга не указана."); return; }

        try
        {
            var visit = clinic.RegisterVisit(animal, vet, service);
            var receipt = clinic.GetLatestReceiptForVisit(visit);
            if (receipt != null)
            {
                ConsoleColors.WriteLineSuccess("\nПриём оформлен. Квитанция:");
                Console.WriteLine(receipt.GetInfo());
            }
            AutoSave();
        }
        catch (Exception ex)
        {
            ConsoleColors.WriteLineError("Ошибка оформления приёма: " + ex.Message);
        }
    }

    static void EditVisitInteractive()
    {
        Console.WriteLine("Редактирование визита. (Введите '0' или 'назад' для возврата в меню)");
        Console.WriteLine();
        var visits = clinic.GetVisits().ToList();
        if (!visits.Any())
        {
            ConsoleColors.WriteLineWarning("Список визитов пуст.");
            return;
        }

        Console.WriteLine("СПИСОК ВИЗИТОВ:");
        foreach (var v in visits.OrderByDescending(v => v.Date).Take(20))
            Console.WriteLine(v);

        Console.Write("\nВведите ID визита для редактирования: ");
        var input = Console.ReadLine();
        if (CheckForBack(input)) return;
        if (!int.TryParse(input, out var id))
        {
            ConsoleColors.WriteLineError("Некорректный ID.");
            return;
        }

        var visit = clinic.FindVisit(id);
        if (visit == null)
        {
            ConsoleColors.WriteLineError("Визит не найден.");
            return;
        }

        Console.WriteLine($"\nТекущие данные визита:");
        Console.WriteLine($"  Животное: {visit.Animal.Name}");
        Console.WriteLine($"  Врач: {visit.Vet.FullName}");
        Console.WriteLine($"  Услуга: {visit.Service}");
        Console.WriteLine($"  Диагноз: {(string.IsNullOrWhiteSpace(visit.Diagnosis) ? "не указан" : visit.Diagnosis)}");
        Console.WriteLine($"  Лечение: {(string.IsNullOrWhiteSpace(visit.Treatment) ? "не указано" : visit.Treatment)}");

        Console.WriteLine("\nВведите новые данные (оставьте пустым, чтобы не изменять):");

        Console.Write("Диагноз: ");
        var diagnosis = Console.ReadLine();
        if (CheckForBack(diagnosis)) return;

        Console.Write("Лечение: ");
        var treatment = Console.ReadLine();
        if (CheckForBack(treatment)) return;

        if (clinic.UpdateVisit(id,
            string.IsNullOrWhiteSpace(diagnosis) ? null : diagnosis,
            string.IsNullOrWhiteSpace(treatment) ? null : treatment))
        {
            ConsoleColors.WriteLineSuccess("Данные визита обновлены.");
            AutoSave();
        }
        else
        {
            ConsoleColors.WriteLineError("Ошибка при обновлении.");
        }
    }

    static void SearchVisitsInteractive()
    {
        Console.WriteLine("Поиск визитов. (Введите '0' или 'назад' для возврата в меню)");
        Console.WriteLine();
        Console.WriteLine("Введите критерии поиска (оставьте пустым, чтобы пропустить):");

        Console.Write("Дата (формат: дд.мм.гггг): ");
        var dateInput = Console.ReadLine();
        if (CheckForBack(dateInput)) return;
        DateTime? date = null;
        if (!string.IsNullOrWhiteSpace(dateInput))
        {
            var parsed = ParseDateOfBirth(dateInput);
            if (parsed.HasValue) date = parsed.Value;
        }

        Console.Write("ID животного: ");
        var animalIdInput = Console.ReadLine();
        if (CheckForBack(animalIdInput)) return;
        int? animalId = null;
        if (!string.IsNullOrWhiteSpace(animalIdInput) && int.TryParse(animalIdInput, out var aId))
            animalId = aId;

        Console.Write("ID ветеринара: ");
        var vetIdInput = Console.ReadLine();
        if (CheckForBack(vetIdInput)) return;
        int? vetId = null;
        if (!string.IsNullOrWhiteSpace(vetIdInput) && int.TryParse(vetIdInput, out var vId))
            vetId = vId;

        var results = clinic.SearchVisits(date, animalId, vetId).OrderByDescending(v => v.Date).ToList();

        Console.WriteLine($"\nНайдено визитов: {results.Count}");
        if (results.Any())
        {
            foreach (var v in results) Console.WriteLine(v);
        }
        else
        {
            ConsoleColors.WriteLineWarning("Визиты не найдены.");
        }
    }

    static void AnimalVisitHistoryInteractive()
    {
        Console.WriteLine("История визитов животного. (Введите '0' или 'назад' для возврата в меню)");
        Console.WriteLine();
        var animals = clinic.GetAnimals().ToList();
        if (!animals.Any())
        {
            ConsoleColors.WriteLineWarning("Нет животных в клинике.");
            return;
        }

        Console.WriteLine("Выберите животное по ID:");
        foreach (var a in animals) Console.WriteLine(a);
        Console.Write("ID животного: ");
        var input = Console.ReadLine();
        if (CheckForBack(input)) return;
        if (!int.TryParse(input, out var animalId))
        {
            ConsoleColors.WriteLineError("Некорректный ID.");
            return;
        }

        var history = clinic.GetAnimalVisitHistory(animalId).ToList();
        if (!history.Any())
        {
            ConsoleColors.WriteLineWarning("История визитов пуста.");
            return;
        }

        Console.WriteLine($"\nИСТОРИЯ ВИЗИТОВ:");
        foreach (var visit in history)
        {
            Console.WriteLine($"  {visit.Date:dd.MM.yyyy HH:mm} - {visit.Vet.FullName} - {visit.Service}");
            if (!string.IsNullOrWhiteSpace(visit.Diagnosis))
                Console.WriteLine($"    Диагноз: {visit.Diagnosis}");
            if (!string.IsNullOrWhiteSpace(visit.Treatment))
                Console.WriteLine($"    Лечение: {visit.Treatment}");
        }
    }

    static void AnimalMedicalCardInteractive()
    {
        Console.WriteLine("Медицинская карта животного. (Введите '0' или 'назад' для возврата в меню)");
        Console.WriteLine();
        var animals = clinic.GetAnimals().ToList();
        if (!animals.Any())
        {
            ConsoleColors.WriteLineWarning("Нет животных в клинике.");
            return;
        }

        Console.WriteLine("Выберите животное по ID:");
        foreach (var a in animals) Console.WriteLine(a);
        Console.Write("ID животного: ");
        var input = Console.ReadLine();
        if (CheckForBack(input)) return;
        if (!int.TryParse(input, out var animalId))
        {
            ConsoleColors.WriteLineError("Некорректный ID.");
            return;
        }

        var card = clinic.GetAnimalMedicalCard(animalId);
        Console.WriteLine("\n" + card);
    }

    static void VetWorkHistoryInteractive()
    {
        Console.WriteLine("История работы ветеринара. (Введите '0' или 'назад' для возврата в меню)");
        Console.WriteLine();
        var vets = clinic.GetEmployees().OfType<Vet>().ToList();
        if (!vets.Any())
        {
            ConsoleColors.WriteLineWarning("Нет ветеринаров в клинике.");
            return;
        }

        Console.WriteLine("Выберите ветеринара по ID:");
        foreach (var v in vets) Console.WriteLine(v);
        Console.Write("ID ветеринара: ");
        var input = Console.ReadLine();
        if (CheckForBack(input)) return;
        if (!int.TryParse(input, out var vetId))
        {
            ConsoleColors.WriteLineError("Некорректный ID.");
            return;
        }

        var history = clinic.GetVetWorkHistory(vetId);
        Console.WriteLine("\n" + history);
    }

    static void AutoSave()
    {
        try
        {
            DataManager.SaveClinic(clinic);
        }
        catch (Exception ex)
        {
            ConsoleColors.WriteLineWarning($"Автосохранение не удалось: {ex.Message}");
        }
    }

    static void ListReceipts()
    {
        Console.WriteLine("СПИСОК КВИТАНЦИЙ:");
        var list = clinic.GetReceipts().ToList();
        if (!list.Any()) { Console.WriteLine("Список пуст."); return; }
        foreach (var r in list) Console.WriteLine(r);
    }
}
