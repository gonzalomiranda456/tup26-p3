# Examen de LINQ en C#

## LINQ — Language Integrated Query

### Propósito de LINQ

1) ¿Qué problema resuelve LINQ según el apunte?

- [ ] Reemplaza todos los tipos genéricos por arrays.
- [x] Eleva el nivel de abstracción al trabajar con colecciones, dejando visible qué resultado se busca.
- [ ] Permite que las clases tengan múltiples constructores.

---

### Programación declarativa

2) ¿Qué caracteriza al estilo declarativo de LINQ?

- [ ] Describe paso por paso cómo modificar variables auxiliares.
- [ ] Obliga a usar índices en todos los recorridos.
- [x] Describe qué resultado se quiere obtener.

---

### Conceptos funcionales

3) ¿Qué método LINQ equivale a `filter` en programación funcional?

- [x] `Where()`
- [ ] `Select()`
- [ ] `Aggregate()`

---

### Transformación

4) ¿Qué método LINQ equivale a `map`?

- [ ] `Where()`
- [x] `Select()`
- [ ] `Count()`

---

### Reducción

5) ¿Qué método representa el `reduce` o `fold` más general?

- [ ] `Distinct()`
- [ ] `Take()`
- [x] `Aggregate()`

---

### Lambdas

6) En esta expresión, ¿qué es `p => p.Precio > 1000`?

```csharp
productos.Where(p => p.Precio > 1000)
```

- [x] Una lambda que recibe un producto y devuelve `bool`.
- [ ] Una clase anónima con una propiedad `Precio`.
- [ ] Una instrucción que modifica todos los productos.

---

### Uniformidad

7) ¿Qué significa que LINQ tenga una sintaxis uniforme?

- [ ] Que solo funciona con listas en memoria.
- [x] Que puede usarse con distintas fuentes de datos, como colecciones, EF Core, XML u observables.
- [ ] Que todos los métodos LINQ devuelven `List<T>`.

---

### `IEnumerable<T>`

8) ¿Qué interfaz mínima habilita los métodos LINQ sobre una fuente de datos?

- [ ] `IDisposable`
- [ ] `IComparable<T>`
- [x] `IEnumerable<T>`

---

### Métodos de extensión

9) ¿Por qué se puede llamar `lista.Where(x => x > 0)`?

- [x] Porque `Where` es un método de extensión sobre `IEnumerable<T>`.
- [ ] Porque `Where` es una palabra reservada obligatoria del lenguaje.
- [ ] Porque todas las listas heredan de una clase `Where`.

---

### `Func<T, TResult>`

10) ¿Qué tipo tiene un selector como `p => p.Nombre` si recibe un `Producto` y devuelve un `string`?

- [ ] `Action<Producto>`
- [x] `Func<Producto, string>`
- [ ] `Predicate<string>`

---

### Evaluación diferida

11) ¿Qué ocurre al construir esta query?

```csharp
var query = productos
    .Where(p => p.Precio > 1000)
    .OrderBy(p => p.Nombre)
    .Select(p => p.Nombre);
```

- [ ] Se filtran y ordenan todos los productos inmediatamente.
- [ ] Se crea una lista mutable con los resultados finales.
- [x] Se construye un plan diferido; la ejecución ocurre al iterar o materializar.

---

### Ejecución real

12) ¿Cuál de estas acciones fuerza la ejecución de una query LINQ diferida?

- [x] `ToList()`
- [ ] Asignarla a una variable `query`.
- [ ] Encadenar otro `Where()`.

---

### Re-ejecución

13) ¿Qué sucede si se itera dos veces una query diferida?

- [ ] La segunda iteración siempre usa una copia guardada automáticamente.
- [x] La query se ejecuta de nuevo.
- [ ] La segunda iteración siempre lanza una excepción.

---

### Materialización

14) ¿Cuándo conviene usar `ToList()`?

- [ ] Cuando se quiere evitar guardar resultados y re-ejecutar siempre.
- [ ] Cuando la query no debe ejecutarse nunca.
- [x] Cuando se quiere ejecutar una vez y reutilizar los resultados materializados.

---

### `First`

15) ¿Qué diferencia hay entre `First()` y `FirstOrDefault()`?

- [x] `First()` lanza si no hay elemento; `FirstOrDefault()` devuelve el valor por defecto.
- [ ] `FirstOrDefault()` siempre lanza si no hay elemento.
- [ ] Ambos cuentan todos los elementos antes de devolver.

---

### `Single`

16) ¿Qué verifica `Single()`?

- [ ] Que haya cero o más elementos.
- [x] Que exista exactamente un elemento.
- [ ] Que todos los elementos sean distintos.

---

### `Any`

17) ¿Por qué es preferible `Any()` a `Count() > 0` para saber si hay elementos?

- [ ] Porque `Any()` ordena la colección primero.
- [ ] Porque `Any()` transforma los elementos en strings.
- [x] Porque puede detenerse al encontrar el primero.

---

### Agregaciones

18) ¿Qué devuelve `MaxBy(p => p.Precio)`?

- [x] El objeto cuyo precio es máximo.
- [ ] Solo el valor numérico máximo.
- [ ] La cantidad de productos con precio máximo.

---

### `DistinctBy`

19) ¿Para qué sirve `DistinctBy(p => p.Categoria)`?

- [ ] Para ordenar por categoría descendente.
- [x] Para quedarse con un elemento por cada categoría, usando esa clave para eliminar duplicados.
- [ ] Para agrupar todos los productos en listas por categoría.

---

### `GroupBy`

20) ¿Qué produce `GroupBy(p => p.Categoria)`?

- [ ] Una única lista plana de precios.
- [ ] Un diccionario mutable obligatorio.
- [x] Una secuencia de grupos, cada uno con una clave y sus elementos.

---

### `SelectMany`

21) ¿Qué hace `SelectMany`?

- [x] Transforma cada elemento en una colección y aplana todas en una sola secuencia.
- [ ] Ordena una secuencia por múltiples claves.
- [ ] Devuelve solo el primer elemento de una colección.

---

### `Join`

22) ¿Qué tipo de unión representa `Join` en el ejemplo del apunte?

- [ ] Left join: todos los elementos de la izquierda aunque no coincidan.
- [x] Inner join: solo elementos con coincidencia en ambas colecciones.
- [ ] Unión de conjuntos sin claves.

---

### `GroupJoin`

23) ¿Qué representa `GroupJoin` en el apunte?

- [ ] Un filtro por texto.
- [ ] Un ordenamiento por varias columnas.
- [x] Un left join: todos los elementos de la izquierda con su grupo de coincidencias.

---

### `Zip`

24) ¿Qué hace `Zip`?

- [x] Combina dos secuencias elemento a elemento.
- [ ] Elimina duplicados exactos.
- [ ] Agrupa por una clave.

---

### Operaciones de conjuntos

25) ¿Qué resultado conceptual produce `Intersect`?

- [ ] Todos los elementos de ambas secuencias.
- [x] Los elementos comunes a ambas secuencias.
- [ ] Los elementos de la primera que no están en la segunda.

---

### `Chunk`

26) ¿Para qué sirve `Chunk(3)`?

- [ ] Para tomar únicamente los primeros tres elementos.
- [ ] Para saltear los primeros tres elementos.
- [x] Para dividir la secuencia en lotes de hasta 3 elementos.

---

### Inmutabilidad

27) ¿Qué ocurre con la colección original al aplicar `OrderBy` o `Where`?

- [x] No se modifica; LINQ devuelve una nueva secuencia.
- [ ] Se ordena o filtra en el mismo objeto original.
- [ ] Se borra automáticamente al finalizar la query.

---

### Efectos secundarios

28) ¿Por qué conviene evitar efectos secundarios dentro de lambdas LINQ?

- [ ] Porque las lambdas no pueden contener bloques.
- [x] Porque la evaluación diferida hace que no siempre sea obvio cuántas veces se ejecutarán.
- [ ] Porque `Select` exige modificar una lista externa.

---

### Filtrar temprano

29) ¿Por qué suele convenir filtrar antes de transformar?

- [ ] Porque `Where` solo puede ir antes de `Select` por regla del compilador.
- [ ] Porque `Select` elimina automáticamente los elementos inactivos.
- [x] Porque se transforman menos elementos y se evita trabajo innecesario.

---

### Método más específico

30) ¿Qué opción recomienda el apunte para encontrar el producto más barato?

- [x] `productos.MinBy(p => p.Precio)`
- [ ] `productos.OrderBy(p => p.Precio).First()` como opción preferida.
- [ ] `productos.Aggregate()` siempre, incluso para casos simples.

---

### Sintaxis de consulta

31) ¿Qué hace el compilador con la sintaxis de consulta?

- [ ] La ejecuta con un motor distinto y más lento.
- [x] La traduce mecánicamente a llamadas a métodos LINQ.
- [ ] La convierte en SQL siempre.

---

### `let`

32) ¿Para qué sirve `let` en sintaxis de consulta?

- [ ] Para terminar una consulta.
- [x] Para nombrar una subexpresión y reutilizarla dentro de la consulta.
- [ ] Para materializar la query como lista.

---

### Consulta vs. métodos

33) ¿En qué caso el apunte recomienda especialmente la sintaxis de consulta?

- [x] Consultas con `join` y `group by`, donde puede ser más legible.
- [ ] Operaciones como `Take`, `Any` y `DistinctBy`, que no tienen equivalente.
- [ ] Cualquier consulta simple de filtrado y proyección.

---

### Métodos sin equivalente

34) ¿Cuál de estos métodos no tiene equivalente directo en sintaxis de consulta según el apunte?

- [ ] `where`
- [ ] `select`
- [x] `Take(5)`
