#:property PublishAot=false

using System.Text.Json;

var n = 10;
var s = "alejandro";
var a = new[] { 1, 2, 3, 4, 5 };
var b = true;

var o = new {
    numero = n,
    cadena = s,
    array = a,
    booleano = b
};

var datos = new {
    numero = 10,
    cadena = "alejandro",
    array = new[] { 1, 2, 3, 4, 5 },
    booleano = true
};

string json = JsonSerializer.Serialize(datos, new JsonSerializerOptions {
    WriteIndented = true
});

Console.WriteLine(json);
File.WriteAllText("datos.json", json);

var xx = $"""
Esto es un 
texto que ocupa
multiples lineas {1+2}
puende tener "comillas" y 'comillas simples' sin problemas.
""";
var nombre = "Alejandro";
var persona = """
{
    "nombre": {nombre},
    "apellido": "Battista",
    "edad": 30,
    "hobbies": ["programar", "leer", "viajar"]
}
""";