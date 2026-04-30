# Examen de control de flujo y manejo de excepciones en C#

## Control de flujo y manejo de excepciones en C#

### Control de flujo

1) ¿Qué determina el control de flujo en un programa?

- [ ] El tamaño final del ejecutable.
- [x] El orden en que se ejecutan las instrucciones.
- [ ] El tipo de dato de todas las variables.

---

### Familias de estructuras

2) ¿Qué familia de estructuras elige qué ejecutar según una condición?

- [ ] Iterativas.
- [ ] De salto.
- [x] Condicionales.

---

### `if` / `else if` / `else`

3) En una cadena `if / else if / else`, ¿qué ocurre cuando una condición verdadera se encuentra primero?

- [x] Se ejecuta ese bloque y no se siguen evaluando los siguientes `else if`.
- [ ] Se ejecutan todos los bloques aunque sus condiciones sean falsas.
- [ ] El programa siempre entra en el `else`.

---

### Llaves en `if`

4) ¿Por qué el apunte recomienda usar llaves incluso cuando el cuerpo del `if` tiene una sola instrucción?

- [ ] Porque C# no permite `if` sin llaves.
- [x] Porque evita errores al agregar líneas y mantiene una convención más segura.
- [ ] Porque hace que la condición se evalúe dos veces.

---

### Cortocircuito

5) ¿Por qué esta condición es segura si `texto` vale `null`?

```csharp
if (texto != null && texto.Length > 0)
    Console.WriteLine(texto.ToUpper());
```

- [ ] Porque `Length` devuelve `0` automáticamente cuando el string es `null`.
- [ ] Porque `&&` evalúa siempre ambas partes.
- [x] Porque si la primera parte de `&&` es falsa, la segunda no se evalúa.

---

### `else` colgado

6) En `if` anidados sin llaves, ¿a qué `if` corresponde un `else`?

- [x] Al `if` interior más cercano, sin importar la indentación visual.
- [ ] Siempre al primer `if` del bloque.
- [ ] A todos los `if` anteriores al mismo tiempo.

---

### Pattern matching en `if`

7) ¿Qué hace esta condición?

```csharp
if (valor is int numero)
    Console.WriteLine(numero * 2);
```

- [ ] Convierte cualquier objeto a `int`, aunque no sea compatible.
- [x] Verifica si `valor` es `int` y declara `numero` si la verificación tiene éxito.
- [ ] Declara `numero` aunque `valor` sea `null` o de otro tipo.

---

### Operador ternario

8) ¿Qué valor queda en `categoria` si `edad` es `20`?

```csharp
string categoria = edad >= 18 ? "adulto" : "menor";
```

- [ ] `"menor"`
- [ ] `true`
- [x] `"adulto"`

---

### Ternarios anidados

9) ¿Por qué conviene evitar ternarios con muchos niveles anidados?

- [x] Porque son técnicamente válidos, pero dificultan la lectura.
- [ ] Porque C# los ejecuta siempre de derecha a izquierda y da error.
- [ ] Porque solo funcionan con números enteros.

---

### `switch` instrucción

10) En un `switch` tradicional, ¿para qué se usa `break` al final de un `case`?

- [ ] Para repetir el mismo `case`.
- [x] Para terminar ese caso y salir del `switch`.
- [ ] Para lanzar una excepción automáticamente.

---

### `switch` expression

11) ¿Qué caracteriza a un `switch expression`?

- [ ] No puede producir valores.
- [ ] Siempre necesita `break` en cada rama.
- [x] Es una expresión que produce un valor y no necesita `break`.

---

### Caso por defecto

12) En un `switch expression`, ¿qué representa `_`?

- [x] El caso por defecto o wildcard.
- [ ] Un error de compilación obligatorio.
- [ ] Un alias para `null` únicamente.

---

### Pattern matching en `switch`

13) ¿Qué permite un pattern de propiedad como `{ Total: > 10_000, EsUrgente: false }`?

- [ ] Comparar solo el tipo del objeto, ignorando sus propiedades.
- [x] Ramificar según valores de propiedades del objeto.
- [ ] Modificar el objeto dentro del patrón.

---

### Guardia `when`

14) ¿Para qué sirve `when` en un patrón de `switch`?

- [ ] Para repetir una rama varias veces.
- [ ] Para reemplazar todos los `case` por `if`.
- [x] Para agregar una condición adicional al patrón.

---

### `while`

15) ¿Qué caracteriza a un bucle `while`?

- [x] Evalúa la condición antes de ejecutar el cuerpo, por lo que puede no ejecutarse nunca.
- [ ] Ejecuta el cuerpo siempre al menos una vez.
- [ ] Solo sirve para recorrer diccionarios.

---

### `do while`

16) ¿Cuándo conviene usar `do while`?

- [ ] Cuando el cuerpo podría no ejecutarse nunca.
- [x] Cuando el cuerpo debe ejecutarse al menos una vez, como en validación de entrada interactiva.
- [ ] Cuando se necesita salir del método con `return`.

---

### `for`

17) ¿Cuándo es especialmente útil un `for`?

- [ ] Cuando no existe ninguna condición de repetición.
- [ ] Cuando se quiere recorrer una colección sin índice y sin contador.
- [x] Cuando se conoce la cantidad de iteraciones o se necesita trabajar con el índice.

---

### `foreach`

18) ¿Qué recorre idiomáticamente un `foreach` en C#?

- [x] Cualquier colección o secuencia que pueda enumerarse como `IEnumerable<T>`.
- [ ] Solo arrays de enteros.
- [ ] Solo valores `null`.

---

### Modificación durante `foreach`

19) ¿Qué indica el apunte sobre modificar una colección mientras se itera con `foreach`?

- [ ] Es la forma recomendada de eliminar elementos.
- [x] No se debe modificar la colección mientras se itera; si hay que eliminar, usar copia o `RemoveAll`.
- [ ] Solo está prohibido en diccionarios, pero no en listas.

---

### `await foreach`

20) ¿Para qué se usa `await foreach`?

- [ ] Para ejecutar un `switch expression` sin `break`.
- [ ] Para recorrer solamente arrays multidimensionales.
- [x] Para recorrer colecciones asíncronas.

---

### `break`

21) ¿Qué hace `break` dentro de un bucle?

- [x] Sale del bucle.
- [ ] Salta solo a la siguiente iteración.
- [ ] Sale siempre de todo el programa.

---

### `continue`

22) ¿Qué hace `continue` dentro de un bucle?

- [ ] Termina definitivamente el método.
- [x] Salta el resto del cuerpo actual y continúa con la siguiente iteración.
- [ ] Ejecuta el bloque `catch`.

---

### `return`

23) ¿Qué efecto tiene `return` dentro de un método?

- [ ] Sale solo del `if`, pero no del método.
- [ ] Salta a la siguiente iteración de un `foreach`.
- [x] Sale del método, y por lo tanto también de cualquier bucle donde esté.

---

### Bucles anidados

24) En bucles anidados, ¿a qué bucle afecta un `break`?

- [x] Solo al bucle más interno donde aparece.
- [ ] A todos los bucles externos automáticamente.
- [ ] Solo al primer bucle escrito en el método.

---

### Guard clauses

25) ¿Qué ventaja tienen las guard clauses al inicio de un método?

- [ ] Aumentan el anidamiento del código.
- [x] Resuelven casos inválidos temprano y evitan anidamiento excesivo.
- [ ] Impiden usar `return`.

---

