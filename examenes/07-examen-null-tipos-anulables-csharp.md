# Examen de null y tipos anulables en C#

## Null y tipos anulables en C#

### Significado de `null`

1) ¿Qué representa `null` en C#?

- [ ] El número cero.
- [x] La ausencia de valor o que una variable no apunta a ningún objeto.
- [ ] Una cadena vacía.

---

### Error común

2) ¿Qué error puede producirse al usar una referencia `null` como si apuntara a un objeto?

- [ ] `InvalidCastException`
- [ ] `DivideByZeroException`
- [x] `NullReferenceException`

---

### Tipos por referencia

3) En una variable de tipo por referencia, ¿qué significa `null`?

- [x] Que la variable no apunta a ningún objeto.
- [ ] Que la variable contiene el valor entero `0`.
- [ ] Que la variable contiene un objeto vacío creado automáticamente.

---

### Tipos por valor

4) ¿Qué ocurre con este código?

```csharp
int edad = null;
```

- [ ] `edad` queda con valor `0`.
- [x] Da error de compilación porque `int` no acepta `null` por defecto.
- [ ] `edad` se convierte automáticamente en `int?`.

---

### Nullable value types

5) ¿Qué permite la sintaxis `int?`?

- [ ] Declarar un `int` que solo puede valer `0`.
- [ ] Declarar un `int` que nunca puede ser `null`.
- [x] Declarar un valor que puede contener un `int` o `null`.

---

### `HasValue`

6) ¿Qué indica `edad.HasValue` si `edad` es `int?`?

- [x] Si `edad` contiene un valor distinto de `null`.
- [ ] Si `edad` es mayor que cero.
- [ ] Si `edad` fue inicializada con `new`.

---

### `.Value`

7) ¿Qué riesgo tiene usar `edad.Value` cuando `edad` es `int?`?

- [ ] Siempre devuelve `0` si `edad` es `null`.
- [x] Lanza `InvalidOperationException` si `edad` es `null`.
- [ ] Convierte `edad` en `string`.

---

### Comparaciones con null

8) ¿Qué resultado produce una comparación como `b < 10` si `b` es `int?` y vale `null`?

- [ ] `true`
- [ ] Lanza `NullReferenceException`.
- [x] `false`

---

### Nullable reference types

9) En C# con nullable reference types, ¿qué diferencia expresa `string` frente a `string?`?

- [x] `string` se asume no nulo; `string?` puede ser `null`.
- [ ] `string` puede ser `null`; `string?` solo puede ser cadena vacía.
- [ ] No hay diferencia para el compilador.

---

### Análisis estático

10) ¿Por qué el compilador advierte en `texto.Length` si `texto` es `string?`?

```csharp
string? texto = ObtenerTexto();
Console.WriteLine(texto.Length);
```

- [ ] Porque `Length` no existe en `string`.
- [x] Porque `texto` podría ser `null` en ese punto.
- [ ] Porque `ObtenerTexto()` siempre devuelve una cadena vacía.

---

### Operador `?.`

11) ¿Qué hace el operador `?.`?

- [ ] Lanza una excepción si el objeto es `null`.
- [ ] Convierte cualquier valor en non-nullable.
- [x] Accede al miembro solo si el objeto no es `null`; si es `null`, devuelve `null`.

---

### Operador `??`

12) ¿Qué valor queda en `display`?

```csharp
string? nombre = null;
string display = nombre ?? "Anónimo";
```

- [x] `"Anónimo"`
- [ ] `null`
- [ ] `""`

---

### Operador `??=`

13) ¿Qué hace `cache ??= "valor por defecto";`?

- [ ] Asigna siempre `"valor por defecto"`, aunque `cache` ya tenga valor.
- [x] Asigna `"valor por defecto"` solo si `cache` es `null`.
- [ ] Lanza una excepción si `cache` es `null`.

---

### Null-forgiving

14) ¿Qué hace el operador `!` en `texto!.Length`?

- [ ] Evita cualquier excepción en tiempo de ejecución.
- [ ] Verifica que `texto` no sea `null`.
- [x] Suprime el warning del compilador, pero no cambia el comportamiento en runtime.

---

### Pattern matching con null

15) ¿Cuál es la forma recomendada en el apunte para verificar si una referencia es `null`?

- [x] `objeto is null`
- [ ] `objeto = null`
- [ ] `objeto.Equals(null)`

---

### Switch con null

16) En este `switch`, ¿qué respuesta se devuelve si `input` es `""`?

```csharp
string respuesta = input switch
{
    null             => "No ingresaste nada",
    ""               => "Ingresaste una cadena vacía",
    { Length: > 20 } => "Texto demasiado largo",
    _                => $"Ingresaste: {input}"
};
```

- [ ] `"No ingresaste nada"`
- [x] `"Ingresaste una cadena vacía"`
- [ ] `"Texto demasiado largo"`

---

### Acceso profundo seguro

17) ¿Qué valor obtiene `display` si `pedido`, `Cliente`, `Direccion` o `Ciudad` son `null`?

```csharp
string display = pedido?.Cliente?.Direccion?.Ciudad ?? "Ciudad desconocida";
```

- [ ] `null`
- [ ] Lanza `NullReferenceException`.
- [x] `"Ciudad desconocida"`

---

### Validación de parámetros

18) ¿Para qué se usa `ArgumentNullException.ThrowIfNull(nombre)` al inicio de un método?

- [x] Para validar que el parámetro no sea `null` y que el compilador lo sepa luego.
- [ ] Para convertir `nombre` en cadena vacía.
- [ ] Para ignorar todos los warnings de nullable del método.

---

### Retornar null

19) ¿Cuál es una buena práctica al devolver resultados de búsqueda?

- [ ] Devolver `null` para representar siempre lista vacía, error y dato no encontrado.
- [x] Devolver lista vacía si no hay resultados y reservar `null` para casos con sentido semántico.
- [ ] Devolver `null` aunque el método prometa `List<Producto>`.

---

### Strings vacíos o blancos

20) ¿Qué método cubre `null`, `""` y strings con solo espacios?

- [ ] `string.IsNullOrEmpty`
- [ ] `GetValueOrDefault`
- [x] `string.IsNullOrWhiteSpace`

---

### Nullable en colecciones

21) ¿Qué significa `List<string?> nombres`?

- [x] Una lista no nullable cuyos elementos pueden ser `null`.
- [ ] Una lista que puede ser `null`, pero cuyos elementos nunca pueden serlo.
- [ ] Una lista de enteros anulables.

---

### Filtrar nulls

22) ¿Qué hace `nombres.OfType<string>().ToList()` si `nombres` es una colección con strings y `null`?

- [ ] Conserva solo los valores `null`.
- [x] Filtra y castea a `string`, descartando automáticamente los `null`.
- [ ] Convierte todos los `null` en `string.Empty`.

