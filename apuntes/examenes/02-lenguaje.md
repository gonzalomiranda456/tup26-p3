# Examen de lenguaje C#

## Funciones, delegados y eventos en C#

---

### Uso de `out`

1. ¿Qué caracteriza al modificador `out`?
- [ ] La variable debe estar inicializada antes de llamar al método.
- [x] La variable no necesita estar inicializada antes de la llamada y el método debe asignarla.
- [ ] El método no puede devolver además un `bool`.

---

### Parámetros variables

2. ¿Para qué sirve `params` en una firma de método?
- [x] Para aceptar una cantidad variable de argumentos del mismo tipo.
- [ ] Para obligar a pasar los argumentos por referencia.
- [ ] Para declarar parámetros opcionales con nombre.

---

### Función local estática

3. ¿Qué ventaja tiene declarar una función local como `static`?
- [ ] Permite modificar cualquier variable externa sin restricciones.
- [x] Evita capturas accidentales del contexto externo y puede ser más eficiente.
- [ ] Hace que la función sea visible desde toda la clase.

---

### Delegados multicast

4. ¿Qué efecto produce usar `+=` sobre un delegado multicast?
- [ ] Reemplaza el método actual por el nuevo.
- [ ] Convierte el delegado en un evento automáticamente.
- [x] Agrega otro método a la lista de invocación del delegado.

---

## Clases y objetos en C#

### Constructor por defecto

5. ¿Qué pasa con el constructor por defecto cuando una clase declara al menos un constructor explícito?
- [ ] Sigue existiendo siempre de forma implícita.
- [ ] Se transforma automáticamente en `private`.
- [x] Deja de generarse implícitamente y hay que declararlo a mano si se necesita.

---

### Delegación entre constructores

6. ¿Para qué se usa `: this(...)` en un constructor?
- [x] Para llamar a otro constructor de la misma clase y reutilizar lógica de inicialización.
- [ ] Para invocar un método estático antes de crear el objeto.
- [ ] Para indicar que el constructor es opcional.

---

### Primary constructor

7. ¿Qué afirmación es correcta sobre el primary constructor en una clase?
- [ ] Crea propiedades públicas automáticamente igual que un `record`.
- [x] Hace que los parámetros estén disponibles en todo el cuerpo de la clase, pero no crea propiedades por sí solo.
- [ ] Sólo puede usarse en clases abstractas.

---

### Auto-propiedades

8. ¿Qué significa esta propiedad?

```csharp
public int Stock { get; private set; }
```

- [ ] Que nadie puede leer `Stock` desde afuera.
- [ ] Que `Stock` sólo puede asignarse en el constructor.
- [x] Que puede leerse desde afuera, pero sólo escribirse desde dentro de la clase.

---

### Propiedades requeridas

9. ¿Qué busca garantizar `required` en una propiedad con `init`?
- [ ] Que la propiedad pueda cambiarse libremente durante toda la vida del objeto.
- [x] Que la propiedad sea obligatoria en el inicializador del objeto y el compilador lo verifique.
- [ ] Que la propiedad sólo pueda declararse en una interfaz.

---

## Interfaces y contratos en C#

### Idea central de interfaz

10. ¿Qué expresa principalmente una interfaz en C#?
- [ ] Qué campos privados comparte una jerarquía de tipos.
- [ ] Cómo está implementada internamente una clase.
- [x] Un contrato de comportamiento: qué puede hacer un tipo.

---

### Miembros permitidos

11. ¿Cuál de estos elementos NO puede tener una interfaz?
- [ ] Métodos sin implementación.
- [x] Campos o variables de instancia.
- [ ] Propiedades.

---

### Implementación múltiple

12. ¿Cuál es una ventaja clave de las interfaces frente a las clases abstractas?
- [x] Un tipo puede implementar múltiples interfaces distintas.
- [ ] Una interfaz puede tener constructores con parámetros.
- [ ] Una interfaz puede guardar estado compartido en campos.

---

### Métodos por defecto

13. Según el apunte, ¿cómo se accede a un método por defecto definido en una interfaz?
- [ ] Siempre a través de la clase concreta, aunque no lo declare explícitamente.
- [ ] Sólo desde un constructor estático.
- [x] A través de una referencia tipada como la interfaz.

---

### Interfaz vs clase abstracta

14. Si varios tipos comparten sólo un contrato de comportamiento, pero no estado común, ¿qué conviene usar?
- [ ] Una clase abstracta con campos protegidos obligatorios.
- [ ] Un `record struct`.
- [x] Una interfaz.

---

## Tipos compuestos y colecciones en C#

### Inmutabilidad de string

15. ¿Qué pasa cuando se ejecuta `ToUpper()` sobre un `string`?
- [ ] Se modifica el mismo objeto original.
- [x] Se crea un nuevo string y el original queda igual.
- [ ] El resultado depende de si la variable fue declarada con `var`.

---

### StringBuilder

16. ¿Cuándo conviene usar `StringBuilder` según el apunte?
- [ ] Cuando quiero comparar strings ignorando mayúsculas y minúsculas.
- [ ] Cuando necesito acceder a un carácter por índice.
- [x] Cuando voy construyendo un texto de forma incremental y quiero evitar muchas asignaciones intermedias.

---

### List

17. ¿Qué garantiza escribir `List<string>`?
- [ ] Que la lista tendrá tamaño fijo.
- [x] Que la lista sólo podrá contener valores de tipo `string`.
- [ ] Que la lista se ordenará automáticamente.

---

### Dictionary<TKey, TValue>

18. ¿Qué ventaja tiene `TryGetValue` frente al acceso directo con `diccionario[clave]`?
- [ ] Permite modificar varias claves a la vez.
- [ ] Obliga a recorrer el diccionario completo antes de leer.
- [x] Evita una excepción si la clave no existe y permite consultar de forma segura.

---

### Record

19. ¿Qué diferencia clave muestra el apunte entre `class` y `record`?
- [x] `record` compara por valor y `class`, en ese ejemplo, compara por referencia.
- [ ] `record` no puede tener métodos.
- [ ] `class` siempre es inmutable y `record` siempre es mutable.

---

## Null y tipos anulables en C#

### Null en tipos por referencia y valor

20. ¿Cuál de estas declaraciones es válida según el apunte?

```csharp
string nombre = null;
int edad = null;
int? codigo = null;
```

- [ ] Sólo la segunda.
- [x] La primera y la tercera.
- [ ] Las tres.

---

### Nullable value type

21. ¿Qué representa `int?` en C#?
- [ ] Un entero que siempre vale 0 por defecto.
- [ ] Un alias para `string`.
- [x] Un tipo por valor que puede contener un `int` o `null`.

---

### Operador `?.`

22. ¿Qué hace el operador `?.`?
- [x] Accede a un miembro sólo si el objeto no es `null`; si es `null`, retorna `null`.
- [ ] Fuerza al compilador a ignorar warnings de null.
- [ ] Reemplaza automáticamente `null` por una cadena vacía.

---

### Operador `??`

23. ¿Qué devuelve esta expresión si `nombre` es `null`?

```csharp
string display = nombre ?? "Anónimo";
```

- [ ] `null`
- [ ] Lanza una excepción
- [x] `"Anónimo"`

---

### Buenas prácticas con null

24. Según el apunte, ¿qué conviene preferir para verificar null?
- [ ] `valor == null`, porque nunca puede ser sobrecargado
- [ ] `valor!`, porque resuelve el null en tiempo de ejecución
- [x] `valor is null` o `valor is not null`
