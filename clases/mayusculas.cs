using System;
using static System.Console;

Console.Out.WriteLine("¿Cuál es tu nombre? ");
string? nombre;
while((nombre = Console.In.ReadLine()) is not null) {
    Console.Out.WriteLine($"{nombre.ToUpper()}");
}

