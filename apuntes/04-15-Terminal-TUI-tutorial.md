# Interfaces de usuario en la terminal con Terminal.Gui

> Tutorial progresivo para Programación III — UTN Tucumán  
> C# 14 / .NET 10 / Terminal.Gui v1.16

---

## 1. ¿Qué es Terminal.Gui?

Terminal.Gui es una librería que dibuja interfaces gráficas **dentro de la terminal**: teclado, foco, listas interactivas, cuadros de diálogo — todo sin abrir ventanas del sistema operativo. El resultado es una TUI (*Text User Interface*).

**Instalación** con la directiva `#:package` (dotnet-script):

```csharp
#:package Terminal.Gui@1.16.3
```

En un proyecto `.csproj` estándar:

```
dotnet add package Terminal.Gui --version 1.16.3
```

---

## 2. Estructura de una aplicación

Toda aplicación Terminal.Gui sigue el mismo esqueleto:

```csharp
using Terminal.Gui;

Application.Init();   // activa el modo TUI

var win = new Window("Mi App") { Width = Dim.Fill(), Height = Dim.Fill() };
win.Add(new Label { Text = "¡Hola!", X = 2, Y = 2 });

Application.Run(win);      // loop de eventos — bloquea hasta RequestStop()
Application.Shutdown();    // restaura la terminal
```

Los tres pasos son obligatorios y siempre van en ese orden. Olvidar `Shutdown()` deja la terminal corrupta.

---

## 3. Layout: `Pos` y `Dim`

El posicionamiento usa columnas y filas de caracteres, no píxeles.

**`Pos` — dónde empieza un widget:**

| Expresión | Significado |
|-----------|-------------|
| `1` | columna/fila fija |
| `Pos.Center()` | centrado en el padre |
| `Pos.Right(otro)` | a la derecha del widget `otro` |
| `Pos.Bottom(otro)` | debajo del widget `otro` |
| `Pos.Top(otro)` | alineado con el borde superior de `otro` |

**`Dim` — qué tamaño tiene un widget:**

| Expresión | Significado |
|-----------|-------------|
| `20` | tamaño fijo |
| `Dim.Fill()` | todo el espacio disponible |
| `Dim.Fill(n)` | todo el espacio disponible menos `n` caracteres |

**Ejemplo — tres botones alineados:**

```csharp
var btnNuevo  = new Button { Text = "_Nuevo",  X = 1, Y = Pos.Bottom(lista) + 1 };
var btnEditar = new Button { Text = "_Editar", X = Pos.Right(btnNuevo) + 2, Y = Pos.Top(btnNuevo) };
var btnSalir  = new Button { Text = "_Salir",  X = Pos.Right(btnEditar) + 2, Y = Pos.Top(btnNuevo) };
```

`Pos.Right(btnNuevo)` calcula en tiempo de ejecución la columna donde termina `btnNuevo`. Si el texto del botón cambia, el layout se ajusta solo.

> La `_` en el texto de un botón subraya el carácter siguiente como atajo de teclado (Alt+letra).

---

## 4. Widgets fundamentales

### `Window`
Raíz de la aplicación. Dibuja un borde con título y contiene todos los demás widgets.

```csharp
var win = new Window("Título") { Width = Dim.Fill(), Height = Dim.Fill() };
win.Add(widget1, widget2, widget3);
```

### `Label`
Texto estático. No acepta foco.

```csharp
new Label { Text = "Nombre:", X = 2, Y = 3 }
```

### `TextField`
Campo de texto editable de una sola línea.

```csharp
var campo = new TextField { X = 12, Y = 3, Width = 30, Text = "valor inicial" };

// Leer el valor — Text es ustring, siempre convertir:
string valor = campo.Text?.ToString()?.Trim() ?? "";
```

### `Button`
Botón clickeable.

```csharp
var btn = new Button { Text = "_Aceptar", X = 2, Y = 10 };
btn.Clicked += () => { /* lógica */ };
```

### `ListView`
Lista scrolleable con selección.

```csharp
var lista = new ListView { X = 1, Y = 3, Width = Dim.Fill(2), Height = Dim.Fill(4) };

lista.SetSource(new List<string> { "Ada Lovelace", "Alan Turing" });

int indice = lista.SelectedItem;  // -1 si no hay selección
```

`SetSource` reemplaza toda la lista; se llama cada vez que los datos cambian.

### `CheckBox`
Casilla de verificación con dos estados: marcado / desmarcado.

```csharp
var check = new CheckBox {
    Text    = "Activo",
    Checked = true,
    X = 2, Y = 5
};

bool estaActivo = check.Checked;

check.Toggled += valorAnterior => {
    bool ahora = check.Checked;
};
```

### `RadioGroup`
Grupo de opciones mutuamente excluyentes. Solo una puede estar seleccionada a la vez.

```csharp
var radio = new RadioGroup {
    RadioLabels = new[] { "Opción A", "Opción B", "Opción C" },
    X = 2, Y = 7
};

int opcion = radio.SelectedItem;  // índice de la opción marcada

radio.SelectedItemChanged += args => {
    int anterior = args.PreviousSelectedItem;
    int actual   = args.SelectedItem;
};
```

---

## 5. Eventos

| Widget | Evento | Cuándo se dispara |
|--------|--------|--------------------|
| `Button` | `Clicked` | Al presionar el botón |
| `TextField` | `TextChanged` | Cada vez que cambia el texto |
| `ListView` | `SelectedItemChanged` | Al cambiar la selección |
| Cualquiera / contenedor | `KeyPress` | Al presionar una tecla |

**`TextChanged` — búsqueda en tiempo real:**

```csharp
buscar.TextChanged += _ => {
    var q = buscar.Text?.ToString()?.Trim().ToLowerInvariant() ?? "";
    lista.SetSource(datos.Where(c => c.Nombre.Contains(q)).ToList());
};
```

**`KeyPress` — atajos de teclado:**

```csharp
win.KeyPress += e => {
    if (e.KeyEvent.Key == (Key.CtrlMask | Key.N)) {
        NuevoContacto();
        e.Handled = true;  // consume el evento
    }
};
```

**Foco:**

```csharp
campo.SetFocus();   // da el foco a un widget
win.FocusNext();    // mueve al siguiente (como Tab)
win.FocusPrev();    // mueve al anterior (como Shift+Tab)
```

---

## 6. Diálogos modales

Un `Dialog` bloquea la interacción con el resto de la UI hasta que se cierra.

```csharp
var dlg = new Dialog("Título", 60, 18);
//                             ^ancho  ^alto

var nombre = new TextField { X = 12, Y = 2, Width = Dim.Fill(2) };
dlg.Add(new Label { Text = "Nombre:", X = 2, Y = 2 }, nombre);

var btnOk = new Button { Text = "Aceptar",  X = 10, Y = 14 };
var btnCx = new Button { Text = "Cancelar", X = Pos.Right(btnOk) + 2, Y = 14 };

btnOk.Clicked += () => Application.RequestStop();
btnCx.Clicked += () => Application.RequestStop();

dlg.Add(btnOk, btnCx);
Application.Run(dlg);  // bloquea aquí

// Después de Run: leer los valores
string valor = nombre.Text?.ToString() ?? "";
```

**Patrón: diálogo que retorna un valor**

La técnica es capturar el resultado en una variable antes de llamar `RequestStop`. Si se cancela, la variable queda `null`:

```csharp
string? resultado = null;

btnOk.Clicked += () => {
    resultado = campo.Text?.ToString()?.Trim();
    Application.RequestStop();
};
btnCx.Clicked += () => Application.RequestStop();

Application.Run(dlg);

if (resultado is not null) {
    // el usuario aceptó
}
```

**Diálogos rápidos con `MessageBox`:**

```csharp
// Confirmación — retorna el índice del botón elegido
int r = MessageBox.Query("Confirmar", "¿Eliminar contacto?", "Sí", "No");
if (r == 0) { /* eligió Sí */ }

// Error
MessageBox.ErrorQuery("Error", "No se pudo guardar.", "Cerrar");
```

---

## 7. Resumen

| Concepto | Elemento | Clave |
|----------|----------|-------|
| Ciclo de vida | `Init` → `Run` → `Shutdown` | Siempre los tres en orden |
| Posición relativa | `Pos.Right`, `Pos.Bottom` | Se recalcula si el widget referenciado cambia |
| Dimensión dinámica | `Dim.Fill(n)` | Todo el espacio menos `n` |
| Texto editable | `TextField` | `.Text` es `ustring`; llamar `.ToString()` siempre |
| Lista de ítems | `ListView` + `SetSource` | `SelectedItem` es un índice |
| Reacción en tiempo real | `TextChanged` | Disparado por cada carácter |
| Atajos globales | `KeyPress` en `Window` | `e.Handled = true` consume el evento |
| Formulario modal | `Dialog` + `Application.Run` | Bloquea hasta `RequestStop` |
| Resultado del diálogo | Variable capturada en el closure | `null` = cancelado |

---

> **UTN Tucumán · Programación III · C#/.NET 10**
