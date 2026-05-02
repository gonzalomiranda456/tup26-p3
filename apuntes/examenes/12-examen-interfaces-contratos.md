# Examen de interfaces y contratos en C#

## Interfaces y contratos en C#

### Concepto de interfaz

1) ¿Qué define una interfaz en C#?

- [ ] El estado interno privado que todas las clases deben compartir.
- [x] Un contrato que indica qué puede hacer un tipo, sin decir necesariamente cómo lo hace.
- [ ] Un constructor común para varias clases.

---

### Interfaz vs. clase abstracta

2) Según el apunte, ¿qué expresa principalmente una interfaz?

- [ ] "Es un tipo de..."
- [ ] "Tiene los mismos campos que..."
- [x] "Puede hacer..." o "se comporta como..."

---

### Miembros de interfaz

3) ¿Cuál de estos miembros no puede tener una interfaz?

- [x] Campos o variables de instancia.
- [ ] Métodos sin implementación.
- [ ] Eventos.

---

### Implementación

4) Si una clase implementa `ILogger`, ¿qué garantiza?

```csharp
public interface ILogger {
    void Log(string mensaje);
    void LogError(string mensaje, Exception? ex = null);
    bool EstaActivo { get; }
}
```

- [ ] Que hereda automáticamente de una clase base llamada `Logger`.
- [x] Que provee los miembros exigidos por el contrato `ILogger`.
- [ ] Que no puede tener otros métodos propios.

---

### Uso polimórfico

5) ¿Qué ventaja tiene escribir código contra `ILogger` en lugar de contra `ConsoleLogger`?

- [ ] Obliga a que solo exista una implementación posible.
- [ ] Impide cambiar el comportamiento en tiempo de ejecución.
- [x] Permite trabajar con distintas implementaciones que cumplen el mismo contrato.

---

### Implementación múltiple

6) ¿Qué ventaja clave tienen las interfaces frente a la herencia de clases?

- [x] Una clase puede implementar múltiples interfaces.
- [ ] Una clase puede heredar de muchas clases base.
- [ ] Una interfaz puede guardar estado privado de instancia.

---

### Métodos por defecto

7) ¿Qué ocurre con un método con implementación por defecto en una interfaz?

- [ ] La clase que implementa la interfaz siempre está obligada a reescribirlo.
- [x] La clase puede usar esa implementación o sobreescribirla.
- [ ] El método se convierte automáticamente en constructor.

---

### Acceso a métodos por defecto

8) Según el apunte, ¿desde dónde se acceden los métodos por defecto de una interfaz?

- [ ] Solo desde el constructor de la clase concreta.
- [ ] Desde cualquier variable del tipo concreto aunque la clase no declare el método.
- [x] A través de una referencia del tipo de la interfaz.

---

### Cuándo usar interfaz

9) ¿Cuándo conviene usar una interfaz según la regla práctica?

- [x] Cuando los tipos comparten solo un contrato de comportamiento.
- [ ] Cuando los tipos comparten campos de instancia y lógica base.
- [ ] Cuando se necesita un constructor común obligatorio.

---

### Cuándo usar clase abstracta

10) ¿Cuándo conviene una clase abstracta en lugar de una interfaz?

- [ ] Cuando se necesitan múltiples contratos independientes.
- [x] Cuando los tipos comparten estado y lógica base.
- [ ] Cuando no se quiere permitir ningún método concreto.

---

### Interfaz genérica

11) ¿Qué permite una interfaz como `IRepositorio<T>`?

- [ ] Escribir un contrato que solo sirve para `int`.
- [ ] Crear campos de instancia dentro de la interfaz.
- [x] Definir un contrato reutilizable para distintos tipos de entidad.

---

### Restricciones `where`

12) ¿Qué significa esta restricción?

```csharp
public static T Maximo<T>(T a, T b) where T : IComparable<T>
```

- [x] Que `T` debe implementar `IComparable<T>`.
- [ ] Que `T` debe ser siempre `string`.
- [ ] Que `T` no puede tener métodos.

---

### Múltiples restricciones

13) ¿Qué exige `where T : IPersistible, IValidable`?

- [ ] Que `T` herede de dos clases base.
- [x] Que `T` implemente ambos contratos.
- [ ] Que `T` sea necesariamente un tipo por valor.

---

### Covarianza

14) ¿Qué permite la covarianza con `out T` en un productor?

- [ ] Usar el tipo genérico solo como entrada.
- [ ] Convertir cualquier interfaz en una clase abstracta.
- [x] Tratar, por ejemplo, un `IEnumerable<string>` como `IEnumerable<object>`.

---

### Contravarianza

15) ¿Qué describe `in T` en una interfaz consumidora?

- [x] El tipo entra o se consume, permitiendo ciertas conversiones hacia tipos más específicos.
- [ ] El tipo solo puede salir como resultado.
- [ ] El tipo debe ser siempre nullable.

---

### `IEnumerable<T>`

16) ¿Qué garantiza `IEnumerable<T>`?

- [ ] Acceso por índice y modificación.
- [x] Que el tipo puede recorrerse, por ejemplo con `foreach`.
- [ ] Que el tipo tiene claves y valores.

---

### `ICollection<T>`

17) ¿Qué agrega `ICollection<T>` sobre `IEnumerable<T>`?

- [ ] Solo formato con interpolación.
- [ ] Herencia múltiple de clases.
- [x] Tamaño y operaciones como `Add`, `Remove`, `Contains` y `Clear`.

---

### `IList<T>`

18) ¿Qué permite `IList<T>`?

- [x] Acceso por índice, `IndexOf`, `Insert` y `RemoveAt`.
- [ ] Liberar recursos externos con `Dispose`.
- [ ] Comparar objetos con `GetHashCode`.

---

### Colecciones de solo lectura

19) ¿Para qué sirve exponer una colección como `IReadOnlyList<T>`?

- [ ] Para permitir que el cliente llame `Add` y `Remove`.
- [x] Para permitir lectura sin modificación externa.
- [ ] Para convertir la lista en un diccionario.

---

### `ISet<T>`

20) ¿Qué tipo de operaciones modela `ISet<T>`?

- [ ] Formatos de texto con `ToString`.
- [ ] Conversión desde `string`.
- [x] Operaciones de conjuntos como diferencia, unión o intersección.

---

### `foreach` por dentro

21) ¿Qué método usa el enumerador para avanzar al siguiente elemento?

- [x] `MoveNext()`
- [ ] `Add()`
- [ ] `DisposeNow()`

---

### `yield return`

22) ¿Qué hace `yield return`?

- [ ] Ejecuta toda la secuencia de inmediato y la guarda completa.
- [x] Convierte un método en un generador y produce elementos de forma lazy.
- [ ] Obliga a implementar manualmente `IEnumerator<T>`.

---

### `yield break`

23) ¿Para qué sirve `yield break`?

- [ ] Para reiniciar la secuencia desde el comienzo.
- [x] Para terminar la secuencia condicionalmente.
- [ ] Para convertir una secuencia sincrónica en asíncrona.

---

### `IAsyncEnumerable<T>`

24) ¿Cuándo es útil `IAsyncEnumerable<T>`?

- [x] Para enumerar fuentes lentas como red, base de datos o archivos grandes con `await foreach`.
- [ ] Para ordenar listas por nombre.
- [ ] Para implementar igualdad por valor.

---

### `IComparable<T>`

25) ¿Para qué sirve implementar `IComparable<T>`?

- [ ] Para que el tipo sea descartado automáticamente por el GC.
- [ ] Para definir múltiples criterios externos de orden.
- [x] Para darle al tipo un orden natural usado por `Sort`, `Min`, `Max` u `OrderBy`.

---

### `IComparer<T>`

26) ¿Cuándo conviene usar `IComparer<T>`?

- [x] Cuando se necesita un criterio de orden externo, por ejemplo ordenar productos por precio o por nombre.
- [ ] Cuando se quiere que un objeto se use con `using`.
- [ ] Cuando el tipo no debe poder recorrerse.

---

### `IEquatable<T>`

27) ¿Qué piezas deben mantenerse consistentes al implementar igualdad por valor según el apunte?

- [ ] Solo `ToString`.
- [x] `IEquatable<T>`, `object.Equals`, `GetHashCode` y los operadores `==` / `!=`.
- [ ] Solo `CompareTo` y `Sort`.

---

### `IDisposable`

28) ¿Para qué existe `IDisposable`?

- [ ] Para que el GC libere memoria más rápido.
- [x] Para liberar recursos externos como archivos, conexiones o sockets cuando se termina de usarlos.
- [ ] Para comparar dos instancias por valor.

---

### `IFormattable`

29) ¿Qué permite implementar `IFormattable`?

- [x] Responder a especificadores de formato como `$"{valor:F}"`.
- [ ] Hacer que un tipo sea recorrible con `foreach`.
- [ ] Permitir herencia múltiple de clases.

---

### `INumber<T>`

30) ¿Para qué se usa una restricción como `where T : INumber<T>`?

- [ ] Para aceptar solamente strings parseables.
- [ ] Para impedir operaciones aritméticas sobre `T`.
- [x] Para escribir algoritmos genéricos que funcionen con tipos numéricos.

---

### Diseño con interfaces

31) En el ejemplo de notificaciones, ¿por qué `ServicioNotificacion` recibe `IEnumerable<INotificador>`?

- [x] Para trabajar con cualquier conjunto de notificadores que implementen el contrato, sin depender de clases concretas.
- [ ] Para impedir que haya más de un canal de notificación.
- [ ] Para obligar a que todos los notificadores hereden de la misma clase base.
