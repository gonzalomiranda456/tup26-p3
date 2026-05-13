# Cómo hacer una TUI básica con funciones de Console

Este tutorial usa [23.5-consola-menu.cs](23.5-consola-menu.cs) para mostrar cómo construir una interfaz de texto básica sin librerías externas, solo con funciones de `Console`.

La idea no es hacer “pantallas bonitas” sino entender cómo se arma la interfaz con herramientas concretas:
- limpiar la pantalla
- dibujar un menú
- mover un cursor visual
- leer teclas y texto
- posicionar el cursor
- mostrar errores debajo del campo actual

## 1. Estructura general del programa

El programa corre en un bucle principal:

```csharp
while(true) {
    Clear();
    Titulo("MENÚ PRINCIPAL");
    int opcion = Menu("Listar Contactos", "Agregar Contacto", "Editar Contacto", "Borrar Contacto", "Salir");
    switch(opcion) {
        case 0: ListarContactos(); break;
        case 1: AgregarContacto(); break;
        case 2: EditarContacto();  break;
        case 3: BorrarContacto();  break;
        case 4:
        case -1:
            WriteLine("\nChau!");
            return;
    }
}
```

Acá aparecen dos ideas centrales de una TUI de consola:
- cada pantalla se dibuja de nuevo con `Clear()`
- cada acción importante se separa en su propia función

## 2. Qué funciones de Console construyen la interfaz

Este programa usa sobre todo estas funciones y propiedades:
- `Clear()`: limpia la pantalla actual
- `Write()` y `WriteLine()`: dibujan texto
- `ReadKey(true)`: lee una tecla sin mostrarla en pantalla
- `ReadLine()`: lee una línea de texto escrita por el usuario
- `SetCursorPosition(x, y)`: mueve el cursor a una posición exacta
- `CursorTop` y `CursorLeft`: permiten saber dónde está el cursor
- `ForegroundColor`: cambia el color del texto
- `ResetColor()`: vuelve al color normal
- `WindowWidth`: sirve para borrar una línea completa

Con eso solo ya alcanza para hacer un menú, formularios y mensajes de error.

## 3. Título de cada pantalla

Cada pantalla empieza con un título:

```csharp
void Titulo(string mensaje) {
    ForegroundColor = ConsoleColor.DarkBlue;
    WriteLine($"\n=== {mensaje} ===");
    ResetColor();
}
```

Acá se usan dos cosas de `Console`:
- `ForegroundColor` para destacar el título
- `WriteLine()` para imprimirlo

Esto no es obligatorio para que funcione, pero sí ayuda a que el usuario sepa en qué parte del programa está.

## 4. Menú con teclado

La función `Menu` dibuja las opciones y espera una tecla:

```csharp
int Menu(params string[] opciones) {
    int y = CursorTop;
    int pos = 0;

    while(true) {
        SetCursorPosition(0, y);
        for(int i = 0; i < opciones.Length; i++) {
            MostrarCursor($"{i + 1}. {opciones[i]}", i == pos);
        }

        var key = ReadKey(true).Key;

        if(key == ConsoleKey.Escape) return -1;
        if(key == ConsoleKey.Enter)  return pos;

        MoverCursor(key, opciones.Length, ref pos);
    }
}
```

Qué hace cada función de consola acá:
- `CursorTop` guarda la fila donde empieza el menú
- `SetCursorPosition(0, y)` vuelve siempre al mismo lugar para redibujar
- `ReadKey(true)` lee flechas, Enter o Escape sin imprimir caracteres raros

La función auxiliar que decide cómo moverse es esta:

```csharp
void MoverCursor(ConsoleKey key, int cantidad, ref int actual) {
    actual = key switch {
        ConsoleKey.UpArrow   => actual - 1,
        ConsoleKey.DownArrow => actual + 1,
        >= ConsoleKey.D0 and <= ConsoleKey.D9 => key - ConsoleKey.D1,
        _ => 0
    };
    actual = Math.Clamp(actual, 0, cantidad - 1);
}
```

Acá ya no hay dibujo, solo lógica de navegación.

## 5. Cómo se dibuja la opción seleccionada

El menú usa una función que muestra cada línea:

```csharp
void MostrarCursor(string texto, bool seleccionada) {
    ForegroundColor = seleccionada ? ConsoleColor.Green : ConsoleColor.Gray;
    WriteLine($" {(seleccionada ? ">" : " ")} {texto}");
    ResetColor();
}
```

Las funciones importantes acá son:
- `ForegroundColor` para marcar la opción activa
- `WriteLine()` para imprimir una línea completa
- `ResetColor()` para no “contagiar” el color a otras salidas

## 6. Formularios con ReadLine

La carga de datos se hace con una función genérica llamada `Leer<T>`:

```csharp
T Leer<T>(string mensaje, Convertidor<T> convertir) {
    ForegroundColor = ConsoleColor.Cyan;
    Write($"{mensaje.PadLeft(12)}: ");
    ResetColor();

    int x = CursorLeft;
    var y = CursorTop;

    Console.SetCursorPosition(0, y + 1);
    Write(new string(' ', WindowWidth - 1));

    while (true) {
        Console.SetCursorPosition(x, y);
        Write("                                ");
        Console.SetCursorPosition(x, y);
        var linea = ReadLine() ?? "";
        if (convertir(linea, out var valor)) {
            Console.SetCursorPosition(0, y + 1);
            Write(new string(' ', WindowWidth - 1));
            Console.SetCursorPosition(0, y + 1);
            return valor;
        }
    }
}
```

Acá está la parte más interesante de la interfaz.

### Qué pasa paso a paso

1. `Write(...)` dibuja la etiqueta del campo.
2. `CursorLeft` guarda dónde empieza la entrada del usuario.
3. `CursorTop` guarda la fila actual.
4. `SetCursorPosition(0, y + 1)` reserva la línea de abajo para errores.
5. Antes de cada intento, vuelve con `SetCursorPosition(x, y)` al lugar exacto donde se escribe el dato.
6. `ReadLine()` captura lo que escribió el usuario.
7. Si el valor es válido, la función devuelve el resultado convertido.

Con este patrón se puede hacer un formulario bastante ordenado usando solo consola.

## 7. Cómo se muestran los errores debajo del campo actual

Cuando la validación falla, el programa muestra el error en la línea inferior:

```csharp
void MostrarError(string mensaje = "") {
    int y = CursorTop;
    ForegroundColor = ConsoleColor.Red;
    Console.SetCursorPosition(14, y);
    Write(mensaje.PadRight(WindowWidth - 1));
    ResetColor();
}
```

Las funciones de consola importantes acá son:
- `CursorTop` para saber en qué línea quedó el cursor después del `ReadLine()`
- `SetCursorPosition(14, y)` para escribir el mensaje justo debajo del campo
- `ForegroundColor = ConsoleColor.Red` para marcar visualmente el error

Eso hace que el error aparezca donde corresponde, no perdido al final de la pantalla.

## 8. Validadores que también construyen la experiencia

Los validadores no solo convierten datos: también interactúan con la interfaz.

Ejemplo:

```csharp
bool TryParseEdad(string texto, out int valor) {
    if (int.TryParse(texto, out valor) && valor >= 18 && valor <= 65) { return true; }
    MostrarError("Por favor, ingrese una edad válida (entre 18 y 65).");
    return false;
}
```

El patrón es este:
- intentás convertir
- si sale bien, devolvés `true`
- si sale mal, mostrás un mensaje con `MostrarError(...)`

Así la validación queda integrada en la interfaz.

## 9. Cómo se usa el lector genérico en un formulario real

La función `AgregarContacto` muestra cómo se arma un formulario con varios campos:

```csharp
void AgregarContacto() {
    Clear();
    Titulo("AGREGAR CONTACTO");

    Contacto nuevo = new(
        Nombre:   Leer<string>("Nombre",   TryParseNombre),
        Apellido: Leer<string>("Apellido", TryParseNombre),
        Telefono: Leer<int>("Teléfono",    TryParseNumero),
        Edad:     Leer<int>("Edad",        TryParseEdad)
    );

    agenda.Add(nuevo);
    WriteLine();
    MostrarContacto(nuevo);
    Pausa("Contacto agregado. Presione una tecla para continuar...");
}
```

Lo importante es que la interfaz no está “hardcodeada” campo por campo. La misma función `Leer<T>` sirve para texto y para números.

## 10. Mostrar datos también forma parte de la interfaz

Después de cargar un contacto, el programa lo vuelve a mostrar:

```csharp
void MostrarContacto(Contacto contacto) {
    WriteLine($"      Nombre: {contacto.Nombre}");
    WriteLine($"    Apellido: {contacto.Apellido}");
    WriteLine($"    Teléfono: {contacto.Telefono.ToTelefono()}");
    WriteLine($"        Edad: {contacto.Edad}");
}
```

Y para eso usa una extensión que formatea el teléfono:

```csharp
public static string ToTelefono(this int valor) {
    return valor.ToString("000-0000");
}
```

Acá el formateo no es la interfaz en sí, pero ayuda a que la salida sea clara y consistente.

## 11. Resumen de la interfaz

Si querés hacer una TUI básica con `Console`, este programa muestra un camino muy claro:
- `Clear()` para cambiar de pantalla
- `WriteLine()` para dibujar contenido
- `ReadKey(true)` para navegación
- `ReadLine()` para formularios
- `SetCursorPosition()` para redibujar en posiciones fijas
- `ForegroundColor` y `ResetColor()` para resaltar información
- `CursorTop` y `CursorLeft` para recordar dónde seguir dibujando

Con eso ya podés construir:
- un menú
- una lista navegable
- una pantalla de alta
- errores contextualizados

## 12. Cómo ejecutarlo

```bash
dotnet 23.5-consola-menu.cs
```

Para probar la interfaz:
- movete por el menú con flechas o números
- entrá en `Agregar Contacto`
- escribí un valor inválido para ver el error debajo del campo
- terminá la carga y observá cómo se muestra el contacto formateado

Si querés ver el ejemplo completo, está en [23.5-consola-menu.cs](23.5-consola-menu.cs).