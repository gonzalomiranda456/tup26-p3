# Examen de enumeraciones en C#

## Enumeraciones en C#

### Concepto de enum

1) ¿Qué representa un `enum` en C#?

- [ ] Un tipo dinámico que puede recibir cualquier valor durante la ejecución.
- [x] Un tipo que define un conjunto finito de constantes con nombre.
- [ ] Una colección que guarda objetos creados en el heap.

---

### Valores automáticos

2) En este enum, ¿qué valor entero tiene `Miercoles`?

```csharp
public enum DiaSemana {
    Lunes,
    Martes,
    Miercoles,
    Jueves
}
```

- [ ] `1`
- [ ] `3`
- [x] `2`

---

### Tipo subyacente

3) ¿Cuál es el tipo entero subyacente predeterminado de un enum en C#?

- [x] `int`
- [ ] `short`
- [ ] `byte`

---

### Uso de `var` con enums

4) ¿Qué tipo infiere el compilador para `mañana`?

```csharp
var mañana = DiaSemana.Jueves;
```

- [ ] `int`
- [x] `DiaSemana`
- [ ] `string`

---

### Conversión desde entero

5) ¿Qué ocurre con este código?

```csharp
EstadoOrden raro = (EstadoOrden)50;
```

- [ ] Lanza una excepción porque `50` no está definido.
- [ ] Convierte automáticamente `50` al primer valor definido del enum.
- [x] Es válido aunque `50` no corresponda a un miembro definido.

---

### Validación de valores

6) ¿Para qué sirve `Enum.IsDefined`?

- [x] Para verificar que un valor corresponda a un miembro definido del enum.
- [ ] Para convertir un enum en `string` sin usar `ToString`.
- [ ] Para obtener el tipo subyacente de un enum.

---

### Parseo seguro

7) ¿Cuál es la ventaja de `Enum.TryParse<T>()` frente a `Enum.Parse<T>()`?

- [ ] `TryParse` solo funciona con números enteros.
- [x] `TryParse` permite intentar la conversión sin lanzar excepción si falla.
- [ ] `TryParse` modifica el enum original si encuentra el valor.

---

### Reflexión sobre enums

8) ¿Qué devuelve `Enum.GetValues<DiaSemana>()`?

- [ ] Los nombres del enum como arreglo de `string`.
- [ ] El valor entero más alto definido en el enum.
- [x] Todos los valores definidos del enum `DiaSemana`.

---

### Flags

9) En un enum con `[Flags]`, ¿por qué los miembros principales deben usar potencias de 2?

- [x] Para que cada opción ocupe un bit distinto y las combinaciones no se superpongan.
- [ ] Para que el enum siempre se serialice como texto en JSON.
- [ ] Para que `Enum.Parse` ignore mayúsculas automáticamente.

---

### Operaciones con flags

10) Si `editor` contiene `Permiso.Leer | Permiso.Escribir`, ¿qué resultado produce esta expresión?

```csharp
bool puedeEscribir = (editor & Permiso.Escribir) != 0;
```

- [ ] `false`, porque `&` quita el permiso indicado.
- [x] `true`, porque `editor` incluye el permiso `Escribir`.
- [ ] Da error, porque los enums no pueden combinarse con operadores.

---

### Serialización JSON

11) ¿Qué efecto tiene usar `JsonStringEnumConverter` al serializar un enum con `System.Text.Json`?

- [ ] Hace que el enum se guarde siempre como número.
- [ ] Impide deserializar el valor nuevamente.
- [x] Permite serializar el enum como texto, por ejemplo `"Pasaporte"`.

---

### Buenas prácticas

12) ¿Por qué conviene definir un valor `0` significativo en un enum?

- [x] Porque `default` produce `0`, y es mejor que ese valor tenga un nombre explícito.
- [ ] Porque C# no permite enums cuyo primer valor sea distinto de `0`.
- [ ] Porque los enums con valor `0` se convierten automáticamente en `[Flags]`.

---

### Descripciones amigables

13) ¿Para qué se usa `[Description]` en miembros de un enum?

- [ ] Para cambiar el valor entero subyacente del miembro.
- [x] Para asociar un texto más amigable que el nombre del miembro.
- [ ] Para convertir automáticamente el enum en un conjunto abierto.

---

### Pattern matching

14) En este `switch`, ¿qué color devuelve `ColorIndicador(Prioridad.Alta)`?

```csharp
static string ColorIndicador(Prioridad p) => p switch {
    Prioridad.Critica or Prioridad.Alta => "rojo",
    Prioridad.Media                     => "amarillo",
    Prioridad.Baja                      => "verde",
    _                                   => "gris"
};
```

- [ ] `"amarillo"`
- [ ] `"gris"`
- [x] `"rojo"`

---

### Enums de .NET

15) ¿Qué enum de .NET se usa para indicar códigos HTTP como `OK`, `NotFound` o `InternalServerError`?

- [x] `HttpStatusCode`
- [ ] `TaskStatus`
- [ ] `SeekOrigin`

---

### Máquinas de estado

16) ¿Por qué un enum resulta útil para modelar una máquina de estados?

- [ ] Porque permite agregar estados nuevos desde datos externos sin cambiar el código.
- [ ] Porque evita tener que definir transiciones válidas.
- [x] Porque permite representar estados posibles con nombre y controlar transiciones entre ellos.

---

### Conjuntos abiertos

17) ¿Por qué no conviene usar un enum para una lista de países que cambia constantemente?

- [ ] Porque los enums no pueden tener nombres como `Argentina` o `Brasil`.
- [x] Porque el conjunto de valores es abierto y el enum se vuelve difícil de mantener.
- [ ] Porque un enum solo puede tener dos valores posibles.
