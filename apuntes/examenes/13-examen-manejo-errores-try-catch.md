# Examen de manejo de errores con try, catch y finally en C#

## Manejo de errores — `try / catch / finally`

---

### Excepciones

1) ¿Qué representan las excepciones en C#?

- [ ] Una forma de repetir un bloque mientras se cumpla una condición.
- [x] La forma de reportar fallos como archivos inexistentes, cortes de red o datos inválidos.
- [ ] Una conversión automática entre tipos numéricos.

---

### `try`

2) ¿Qué se coloca dentro de un bloque `try`?

- [ ] Solo código que nunca puede fallar.
- [ ] El código que se ejecuta después de todos los `catch`.
- [x] Código que podría lanzar una excepción.

---

### Orden de `catch`

3) ¿Por qué `catch (Exception ex)` debe ir al final?

- [x] Porque `Exception` es la más general y capturaría antes excepciones más específicas.
- [ ] Porque C# solo permite un `catch` por cada `try`.
- [ ] Porque `Exception` solo puede capturarse dentro de `finally`.

---

### Captura específica

4) ¿Qué bloque captura un archivo inexistente en el ejemplo del apunte?

```csharp
try {
    string contenido = File.ReadAllText("datos.txt");
} catch (FileNotFoundException ex) {
    Console.WriteLine($"Archivo no encontrado: {ex.FileName}");
}
```

- [ ] `try`
- [x] `catch (FileNotFoundException ex)`
- [ ] `finally`

---

### `finally`

5) ¿Cuándo se ejecuta un bloque `finally`?

- [ ] Solo cuando el `try` termina sin errores.
- [ ] Solo cuando se captura `Exception`.
- [x] Siempre, tanto si el `try` tuvo éxito como si lanzó una excepción.

---

### Liberación de recursos

6) ¿Para qué se usa comúnmente `finally`?

- [x] Para liberar recursos o ejecutar limpieza obligatoria.
- [ ] Para declarar una excepción personalizada.
- [ ] Para evitar que cualquier excepción exista.

---

### `using`

7) ¿Qué reemplaza normalmente `using` cuando se trabaja con recursos `IDisposable`?

- [ ] El patrón `if / else`.
- [x] El patrón `try / finally` usado para liberar recursos.
- [ ] La necesidad de capturar excepciones específicas.

---

### `throw`

8) ¿Qué hace esta línea?

```csharp
throw new ArgumentOutOfRangeException(nameof(n), "No se puede calcular la raíz de un número negativo.");
```

- [ ] Captura una excepción existente.
- [ ] Ignora una excepción y continúa.
- [x] Lanza una excepción porque el argumento no cumple una regla.

---

### Relanzar

9) ¿Por qué se usa `throw;` sin argumentos dentro de un `catch`?

- [x] Para relanzar la excepción original preservando su stack trace.
- [ ] Para crear una excepción nueva sin mensaje.
- [ ] Para finalizar el programa sin pasar por otros bloques.

---

### `throw` como expresión

10) ¿En cuál de estas situaciones el apunte muestra que `throw` puede usarse como expresión?

- [ ] Solo dentro de un `for`.
- [x] En ternarios, operadores `??` y switch expressions.
- [ ] Únicamente como primera línea de un método.

---

### Operador `??` con `throw`

11) ¿Qué ocurre si `ObtenerNombre()` devuelve `null`?

```csharp
string nombre = ObtenerNombre() ?? throw new InvalidOperationException("Nombre requerido.");
```

- [ ] `nombre` queda como cadena vacía.
- [ ] Se asigna la palabra `"null"`.
- [x] Se lanza `InvalidOperationException`.

---

### Excepciones personalizadas

12) ¿Cuándo conviene crear una excepción personalizada?

- [x] Cuando el error tiene un significado específico del dominio del programa.
- [ ] Cuando se quiere reemplazar cualquier `if` por `try/catch`.
- [ ] Cuando el error siempre debe ignorarse.

---

### Herencia de excepciones

13) ¿De qué debe heredar una excepción personalizada según el apunte?

- [ ] De `List<T>`.
- [x] De `Exception` o de una subclase de `Exception`.
- [ ] De `IEnumerable<T>`.

---

### Datos de dominio

14) ¿Qué ventaja tiene `SaldoInsuficienteException` en el ejemplo?

- [ ] Solo guarda un texto fijo sin más información.
- [ ] Evita que el método `Extraer` valide el saldo.
- [x] Puede transportar datos específicos como saldo actual y monto solicitado.

---

### Filtro `when`

15) ¿Para qué sirve `when` en un `catch`?

- [x] Para capturar una excepción solo si además se cumple una condición.
- [ ] Para ejecutar siempre el bloque `finally`.
- [ ] Para convertir cualquier excepción en `IOException`.

---

### `when` vs. `if`

16) ¿Qué pasa si la condición de un `catch (...) when (...)` es falsa?

- [ ] La excepción queda capturada y se ignora.
- [x] La excepción sigue propagándose al siguiente `catch` o hacia arriba.
- [ ] Se ejecuta automáticamente el mismo `catch` de nuevo.

---

### Jerarquía de excepciones

17) Según la jerarquía del apunte, ¿cuál de estas excepciones deriva de `IOException`?

- [ ] `ArgumentNullException`
- [ ] `DivideByZeroException`
- [x] `FileNotFoundException`

---

### `NullReferenceException`

18) ¿Cómo presenta el apunte a `NullReferenceException`?

- [x] Como una señal de null no manejado que conviene evitar.
- [ ] Como la excepción recomendada para validar parámetros.
- [ ] Como una excepción obsoleta igual que `ApplicationException`.

---

### Capturar `Exception`

19) ¿Por qué es peligroso capturar `Exception` genérica sin relanzar?

- [ ] Porque siempre impide compilar el programa.
- [x] Porque puede silenciar errores importantes.
- [ ] Porque solo captura errores de archivo.

---

### Validar parámetros

20) ¿Qué método muestra el apunte para validar que un parámetro no sea `null`?

- [ ] `File.ReadAllText`
- [x] `ArgumentNullException.ThrowIfNull`
- [ ] `GC.SuppressFinalize`

---

### Conversiones esperadas

21) ¿Qué recomienda el apunte para conversiones esperadas como texto a número?

- [x] Usar `TryParse` en lugar de `try/catch`.
- [ ] Usar siempre `catch (Exception) { }`.
- [ ] Lanzar `NullReferenceException`.

---

### Búsqueda en diccionarios

22) Si una clave puede no existir en un diccionario, ¿qué práctica recomienda el apunte?

- [ ] Acceder con `diccionario["clave"]` y capturar `KeyNotFoundException` como flujo normal.
- [ ] Usar `finally` para inventar la clave faltante.
- [x] Usar `TryGetValue` para manejar el caso esperado sin excepción.
