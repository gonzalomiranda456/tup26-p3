# Examen de tipos compuestos y colecciones en C#

## Tipos compuestos y colecciones en C#

---

### Tipos compuestos

1) ¿Para qué sirven los tipos compuestos en C#?

- [ ] Para guardar únicamente números enteros.
- [x] Para agrupar o coleccionar datos que van más allá de un único valor simple.
- [ ] Para reemplazar todos los métodos por variables globales.

---

### Arrays

2) ¿Cuál es una característica de un array `T[]`?

- [ ] Puede cambiar de tamaño automáticamente al agregar elementos.
- [ ] Permite guardar valores de cualquier tipo mezclados sin restricciones.
- [x] Guarda elementos del mismo tipo y su tamaño queda fijo al crearlo.

---

### Índices de arrays

3) Si un array tiene 5 elementos, ¿cuál es su última posición válida?

- [x] `4`
- [ ] `5`
- [ ] `-1`

---

### Índice desde el final

4) ¿Qué valor queda en `penultimo`?

```csharp
int[] numeros = [10, 20, 30, 40, 50];
int penultimo = numeros[^2];
```

- [ ] `20`
- [x] `40`
- [ ] `50`

---

### Rangos en arrays

5) ¿Qué contiene `primerosTres`?

```csharp
int[] numeros = [10, 20, 30, 40, 50, 60];
var primerosTres = numeros[0..3];
```

- [ ] `[10, 20]`
- [ ] `[10, 20, 30, 40]`
- [x] `[10, 20, 30]`

---

### Recorrido

6) ¿Cuándo conviene usar un `for` en lugar de `foreach` al recorrer una colección?

- [x] Cuando hace falta trabajar con la posición o índice del elemento.
- [ ] Cuando la colección tiene elementos repetidos.
- [ ] Cuando se quiere impedir que el compilador conozca el tipo.

---

### `List<T>`

7) ¿Cuál es una diferencia clave entre `List<T>` y un array?

- [ ] `List<T>` usa `Length` y el array usa `Count`.
- [x] `List<T>` puede crecer o reducirse en ejecución; el array tiene tamaño fijo.
- [ ] `List<T>` no permite acceder por índice.

---

### Métodos de lista

8) ¿Qué hace `items.Insert(0, "Omega")` en una lista?

- [ ] Elimina el elemento de la posición `0`.
- [ ] Reemplaza todos los elementos por `"Omega"`.
- [x] Inserta `"Omega"` en la posición `0`.

---

### LINQ

9) ¿Qué significa la evaluación diferida en una consulta LINQ?

- [x] Que la consulta no se ejecuta al definirla, sino al iterar el resultado o pedir valores como `ToList()` o `Count()`.
- [ ] Que la consulta se ejecuta dos veces automáticamente.
- [ ] Que LINQ solo puede usarse con arrays, no con listas.

---

### Tipos genéricos

10) En `List<string>`, ¿qué representa `string`?

- [ ] El nombre interno de la lista.
- [x] El parámetro de tipo que indica que la lista contiene strings.
- [ ] El valor inicial de todos los elementos.

---

### Tuplas

11) ¿Para qué sirve una tupla como `(Nombre: "Ada", Edad: 20)`?

- [ ] Para definir una clase pública reutilizable entre muchas capas.
- [ ] Para crear una colección dinámica con tamaño variable.
- [x] Para agrupar varios valores rápidamente sin definir una clase.

---

### Deconstrucción

12) ¿Qué valor queda en `soloY`?

```csharp
var punto = (X: 10.5, Y: 3.2);
var (_, soloY) = punto;
```

- [x] `3.2`
- [ ] `10.5`
- [ ] `_`

---

### Tipos anónimos

13) ¿Por qué una variable con tipo anónimo debe declararse con `var`?

- [ ] Porque sus propiedades son modificables después de creado.
- [x] Porque el compilador genera internamente el tipo y el código fuente no ve su nombre.
- [ ] Porque los tipos anónimos solo pueden guardar strings.

---

### Records

14) ¿Cuál es una diferencia clave entre `record` y `class` según el ejemplo del apunte?

- [ ] `record` compara siempre por referencia y `class` por valor.
- [ ] `record` no puede tener propiedades.
- [x] `record` compara por valor, mientras que una `class` común compara por referencia.

---

### Expresión `with`

15) ¿Qué ocurre con `original` en este código?

```csharp
var original = new Persona("Ana", 25);
var cumpleaños = original with { Edad = 26 };
```

- [x] `original` conserva `Edad = 25` y se crea una copia con `Edad = 26`.
- [ ] `original` cambia su edad a `26`.
- [ ] El código es inválido porque los records no admiten copias.

---

### Diccionarios

16) ¿Qué almacena un `Dictionary<TKey, TValue>`?

- [ ] Una secuencia de valores sin orden y sin claves.
- [x] Pares clave → valor, donde cada clave es única.
- [ ] Solo valores numéricos ordenados por índice.

---

### Acceso seguro en diccionarios

17) ¿Qué ventaja tiene `TryGetValue` frente al acceso directo con `telefonos["Pedro"]`?

- [ ] Ordena automáticamente el diccionario por clave.
- [ ] Elimina la clave si no existe.
- [x] Permite consultar sin lanzar excepción cuando la clave no existe.

---

### `HashSet<T>`

18) ¿Qué caracteriza a un `HashSet<T>`?

- [x] Almacena elementos únicos y permite verificar pertenencia de forma eficiente.
- [ ] Guarda pares clave → valor.
- [ ] Mantiene siempre los elementos ordenados por inserción.

---

### Operaciones de conjuntos

19) ¿Qué resultado produce `IntersectWith` entre `{ 1, 2, 3, 4 }` y `{ 3, 4, 5, 6 }`?

- [ ] `{ 1, 2, 5, 6 }`
- [x] `{ 3, 4 }`
- [ ] `{ 1, 2, 3, 4, 5, 6 }`

---

### Métodos de `Array`

20) ¿Por qué se escribe `Array.Sort(numeros)` en lugar de `numeros.Sort()`?

- [ ] Porque los arrays no pueden ordenarse en C#.
- [x] Porque muchas operaciones sobre arrays están concentradas como métodos estáticos de `Array`.
- [ ] Porque `Sort` solo funciona con `List<T>`.

---

### Arrays multidimensionales

21) ¿Qué imprime este código?

```csharp
int[,] matriz =
{
    { 1, 2, 3 },
    { 4, 5, 6 },
    { 7, 8, 9 }
};

Console.WriteLine(matriz[1, 2]);
```

- [ ] `5`
- [ ] `8`
- [x] `6`

---

### Jagged arrays

22) ¿Qué caracteriza a un jagged array como `int[][] triangulo`?

- [x] Es un array de arrays, donde cada fila puede tener distinto largo.
- [ ] Es un array rectangular donde todas las filas deben tener el mismo largo.
- [ ] Es un diccionario que usa enteros como claves.

---

### Igualdad en tipos anónimos

23) ¿Qué imprime este código?

```csharp
var a = new { Nombre = "Ana", Edad = 25 };
var b = new { Nombre = "Ana", Edad = 25 };

Console.WriteLine(a.Equals(b));
```

- [ ] `False`, porque los tipos anónimos siempre comparan por referencia.
- [x] `True`, porque tienen las mismas propiedades con el mismo nombre, tipo y orden.
- [ ] Da error, porque los tipos anónimos no tienen `Equals`.

---

### Variantes de diccionario

24) ¿Qué variante de diccionario conviene cuando se necesitan claves ordenadas?

- [ ] `ConcurrentDictionary<K,V>`
- [x] `SortedDictionary<K,V>`
- [ ] `OrderedDictionary<K,V>`
