namespace VetClinic;

using System;

/// <summary>
/// Вспомогательный класс для цветного вывода сообщений в консоль.
/// Используется для статусов: успех, ошибка, предупреждение, информация, заголовки.
/// </summary>
public static class ConsoleColors
{
    public static void WriteSuccess(string text)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(text);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteError(string text)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(text);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteWarning(string text)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(text);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteInfo(string text)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(text);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteHeader(string text)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(text);
        Console.ForegroundColor = originalColor;
    }

    public static void WriteLineSuccess(string text)
    {
        WriteSuccess(text);
        Console.WriteLine();
    }

    public static void WriteLineError(string text)
    {
        WriteError(text);
        Console.WriteLine();
    }

    public static void WriteLineWarning(string text)
    {
        WriteWarning(text);
        Console.WriteLine();
    }

    public static void WriteLineInfo(string text)
    {
        WriteInfo(text);
        Console.WriteLine();
    }
}
