namespace TP2_GONZALO_CALCULADORA;

static class Program 
{
    static void Main(string[] args) 
    {
        Console.Title = "Calculadora de Expresiones - TP2";
        Console.WriteLine("--- Intérprete de Expresiones Aritméticas ---");

        if (args.Length == 0)
        {
            
            Console.WriteLine("Iniciando modo interactivo... (Escribe 'fin' para salir)");
        }   
        else
        {
            
            Console.WriteLine($"Procesando {args.Length} argumentos de entrada...");
        }
    }
}
