#!/usr/bin/env -S dotnet run
#:package System.CommandLine@2.0.0-beta4.22272.1
using System.CommandLine;

var raiz = new RootCommand("Ejemplo de aplicación de consola con System.CommandLine");

var saludar = new Command("saludar", "Saluda a una persona");
var nombre  = new Argument<string>("nombre", "El nombre de la persona a saludar");
saludar.AddArgument(nombre);
saludar.SetHandler((string nombre) => Console.WriteLine($"Hola {nombre}!"), nombre);

var sumar = new Command("sumar", "Suma dos números");
var num1  = new Argument<int>("numero1", "El primer número a sumar");
var num2  = new Argument<int>("numero2", "El segundo número a sumar");
sumar.AddArgument(num1);
sumar.AddArgument(num2);
sumar.SetHandler((int a, int b) => Console.WriteLine($"{a} + {b} = {a + b}") , num1, num2);

raiz.AddCommand(saludar);
raiz.AddCommand(sumar);

return await raiz.InvokeAsync(args);
