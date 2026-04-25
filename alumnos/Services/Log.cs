namespace Tup26.AlumnosApp;

static class Log {
    public static void Escribir(string mensaje, ConsoleColor color = ConsoleColor.Black) {
        ConsoleColor colorAnterior = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(mensaje);
        Console.ForegroundColor = colorAnterior;
    }

    public static void Debug(string mensaje)   => Escribir(mensaje, ConsoleColor.Blue);
    public static void Error(string mensaje)   => Escribir(mensaje, ConsoleColor.Red);
    public static void Info(string mensaje)    => Escribir(mensaje, ConsoleColor.Green);
    public static void Warning(string mensaje) => Escribir(mensaje, ConsoleColor.Yellow);
}