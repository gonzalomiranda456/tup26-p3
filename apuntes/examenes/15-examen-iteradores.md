# Examen de iteradores en C#

## Iteradores en C#: `GetEnumerator`, `IEnumerable`, `IEnumerator` y `yield`

### Motivación

1) ¿Por qué no alcanza siempre con recorrer una colección usando `for` con índice?

- [ ] Porque `for` solo puede usarse con strings.
- [x] Porque muchas fuentes de datos no tienen acceso aleatorio indexado eficiente.
- [ ] Porque `for` no puede imprimir valores por consola.

---

### Encapsulamiento

2) ¿Qué problema tiene exponer directamente el arreglo interno de una clase como `Baraja.Cartas`?

- [ ] Hace imposible recorrer las cartas.
- [ ] Obliga a usar `foreach` siempre.
- [x] Rompe el encapsulamiento y acopla al consumidor con la implementación interna.

---

### Colección e iterador

3) ¿Por qué conviene separar la colección del iterador?

- [x] Porque cada recorrido necesita su propio estado y pueden existir recorridos simultáneos.
- [ ] Porque la colección no puede tener métodos.
- [ ] Porque el iterador debe guardar todos los elementos duplicados.

---

### `GetEnumerator`

4) ¿Qué debe devolver `GetEnumerator()`?

- [ ] El primer elemento de la colección.
- [x] Un iterador nuevo capaz de recorrer la colección.
- [ ] La cantidad total de elementos.

---

### `IEnumerable`

5) ¿Qué representa `IEnumerable`?

- [ ] El objeto que lleva la posición actual del recorrido.
- [ ] Un método que ordena automáticamente los datos.
- [x] Una colección enumerable, es decir, algo que puede recorrerse.

---

### `IEnumerator`

6) ¿Qué representa `IEnumerator`?

- [x] El iterador que lleva la cuenta del recorrido y sabe avanzar.
- [ ] La colección completa con todos sus datos públicos.
- [ ] Un tipo que solo sirve para arrays.

---

### `foreach` por dentro

7) ¿Qué hace `foreach` de forma aproximada?

- [ ] Accede siempre por índice desde `0` hasta `Length - 1`.
- [x] Obtiene un enumerador, llama a `MoveNext()` y lee `Current`.
- [ ] Convierte cualquier colección en `List<T>` antes de recorrerla.

---

### `MoveNext`

8) ¿Qué dos tareas combina `MoveNext()`?

- [ ] Ordenar la colección y eliminar duplicados.
- [ ] Crear la colección y liberar memoria.
- [x] Avanzar el iterador e indicar si todavía hay un elemento disponible.

---

### `Current`

9) ¿Cuándo es válido leer `Current`?

- [x] Después de que `MoveNext()` devuelve `true`.
- [ ] Antes de llamar por primera vez a `MoveNext()`.
- [ ] Después de que `MoveNext()` devuelve `false`.

---

### Enumerador nuevo

10) ¿Por qué `GetEnumerator()` debe crear un enumerador nuevo cada vez?

- [ ] Para que la colección cambie de tipo en cada recorrido.
- [x] Para permitir recorridos independientes sobre la misma colección.
- [ ] Para que `Current` sea siempre `null`.

---

### Interfaces genéricas

11) ¿Qué ventaja tienen `IEnumerable<T>` e `IEnumerator<T>` frente a las versiones no genéricas?

- [ ] Eliminan la necesidad de `foreach`.
- [ ] Hacen que `Current` sea siempre `object`.
- [x] Dan seguridad de tipos y evitan casts innecesarios.

---

### `IDisposable`

12) ¿Por qué `IEnumerator<T>` hereda de `IDisposable`?

- [x] Porque un iterador puede tener recursos que liberar, como archivos o conexiones.
- [ ] Porque todos los iteradores deben escribir en disco.
- [ ] Porque `Dispose()` reemplaza a `MoveNext()`.

---

### Implementación explícita

14) ¿Por qué al implementar `IEnumerable<T>` también aparece una implementación de `IEnumerable.GetEnumerator()` no genérica?

- [ ] Porque `IEnumerable<T>` no permite `foreach`.
- [ ] Porque `IEnumerator<T>` no tiene `Current`.
- [x] Porque `IEnumerable<T>` hereda de la interfaz no genérica.

---

### `yield return`

15) ¿Qué logra `yield return`?

- [x] Permite escribir un método iterador y deja que el compilador genere el enumerador.
- [ ] Ejecuta toda la secuencia inmediatamente y la guarda en memoria.
- [ ] Convierte cualquier método en `void`.

---

### Máquina de estados

16) ¿Qué genera internamente el compilador cuando encuentra `yield return`?

- [ ] Un array con todos los valores posibles.
- [x] Una máquina de estados que recuerda dónde quedó la ejecución.
- [ ] Un método `Main` nuevo.

---

### Pausa y reanudación

17) ¿Qué ocurre cuando un método iterador llega a `yield return X`?

- [ ] Termina definitivamente el método y no puede continuar.
- [ ] Ignora `X` y sigue hasta el final.
- [x] Guarda `X` como `Current`, pausa el método y devuelve `true` desde `MoveNext()`.

---

### `yield break`

18) ¿Para qué sirve `yield break`?

- [x] Para cortar la iteración de golpe.
- [ ] Para devolver dos valores al mismo tiempo.
- [ ] Para reiniciar el enumerador desde el inicio.

---

### Método iterador

19) ¿Qué puede devolver un método que usa `yield return`?

- [ ] Solo `int`.
- [x] Una secuencia como `IEnumerable<T>`.
- [ ] Únicamente `void`.

---

### Ejecución diferida

20) ¿Qué significa que un iterador tenga ejecución diferida?

- [ ] Que todos los valores se calculan al llamar al método.
- [x] Que el código se ejecuta de a partes cuando el consumidor pide elementos.
- [ ] Que el código nunca se ejecuta.

---

### Momento de errores

21) ¿Cuándo aparece una excepción dentro de un método iterador con `yield`?

- [ ] Siempre al llamar al método que devuelve la secuencia.
- [ ] Nunca puede aparecer dentro de un iterador.
- [x] Al iterar la secuencia y llegar al punto donde ocurre el error.

---

### Archivos grandes

22) ¿Por qué un iterador puede procesar un archivo muy grande sin cargarlo entero en memoria?

- [x] Porque puede producir una línea por vez.
- [ ] Porque comprime el archivo automáticamente.
- [ ] Porque convierte el archivo en un array completo antes de empezar.

---

### Reiterar secuencias

23) ¿Qué ocurre si se recorre dos veces una secuencia diferida creada con un método iterador?

- [ ] La segunda vez siempre falla.
- [x] El método se ejecuta otra vez, con un nuevo enumerador.
- [ ] La segunda vez usa obligatoriamente una copia en memoria.

---

### Materialización

24) ¿Para qué sirve llamar `.ToList()` sobre una secuencia diferida?

- [ ] Para hacer que el iterador sea infinito.
- [ ] Para impedir cualquier recorrido posterior.
- [x] Para ejecutar la secuencia una vez y guardar los resultados en memoria.

---

### Secuencias infinitas

25) ¿Por qué una secuencia infinita como `Fibonacci()` no explota por sí sola?

- [x] Porque la ejecución es diferida y el consumidor decide cuándo dejar de pedir valores.
- [ ] Porque C# limita todos los `while(true)` a 10 vueltas.
- [ ] Porque `yield return` convierte los números infinitos en un único valor.

---

### `Take`

26) ¿Qué hace `Fibonacci().Take(10)`?

- [ ] Calcula todos los números de Fibonacci posibles.
- [x] Pide solo 10 valores y luego deja de llamar a `MoveNext()`.
- [ ] Convierte Fibonacci en una lista infinita en memoria.

---

### Composición de iteradores

27) ¿Qué patrón usan métodos como `Cuadrados(IEnumerable<int> fuente)`?

- [ ] Reciben una secuencia, la modifican en su lugar y no devuelven nada.
- [x] Reciben un `IEnumerable<T>`, transforman sus elementos y devuelven otro `IEnumerable<T>`.
- [ ] Solo funcionan si la fuente es un array.

---

### LINQ

28) ¿Qué relación tiene LINQ-to-Objects con los iteradores?

- [x] Sus operadores como `Where`, `Select` y `Take` son métodos sobre `IEnumerable<T>` que usan iteración diferida.
- [ ] LINQ siempre ejecuta todo inmediatamente.
- [ ] LINQ no puede trabajar con `IEnumerable<T>`.

---

### `Where`

29) ¿Qué hace un `Where` sobre una secuencia?

- [ ] Transforma cada elemento en otro tipo sin filtrar.
- [ ] Carga toda la base de datos en memoria siempre.
- [x] Devuelve solo los elementos que cumplen un predicado.

---

### `Select`

30) ¿Qué hace un `Select` sobre una secuencia?

- [x] Transforma cada elemento usando una función.
- [ ] Termina una secuencia con `yield break`.
- [ ] Libera recursos con `Dispose()`.

---

### `File.ReadLines`

31) ¿Qué diferencia clave tiene `File.ReadLines(ruta)` frente a `File.ReadAllLines(ruta)`?

- [ ] `ReadLines` carga todo el archivo en un `string[]`.
- [x] `ReadLines` devuelve una secuencia diferida y permite leer línea por línea.
- [ ] `ReadLines` solo funciona con archivos vacíos.
