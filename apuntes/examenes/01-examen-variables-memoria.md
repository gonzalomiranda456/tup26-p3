# Examen de fundamentos de C#: variables, memoria y tipos básicos

## Tipado en C#

---

### Tipado estático

1) ¿Qué significa que C# tenga tipado estático?

- [ ] Que todas las variables deben declararse como `static`.
- [x] Que el tipo de cada variable se conoce y se controla antes de ejecutar el programa.
- [ ] Que el tipo de una variable puede cambiar libremente durante la ejecución.

---

### Tipado fuerte

2) ¿Qué describe mejor la idea de tipado fuerte?

```csharp
int numero = 10;
string texto = "20";
```

- [ ] `string` e `int` son equivalentes.
- [ ] `numero = texto;` funciona siempre porque C# convierte automáticamente.
- [x] Hay que convertir explícitamente si se quiere pasar de `string` a `int`.

---

## Variables e inferencia de tipos

### Asignación

3) En C#, ¿qué significa la instrucción `x = x + 1;` si antes `x` tenía el valor `5`?

- [ ] Que se intenta escribir una igualdad matemática imposible.
- [x] Que se toma el valor actual de `x`, se le suma `1` y se guarda el resultado en `x`.
- [ ] Que se crea una nueva variable llamada `x + 1`.

---

### Inferencia con `var`

4) ¿Qué significa realmente `var` en C#?

- [ ] Que la variable cambia de tipo según lo que se le asigne después.
- [ ] Que la variable no tiene tipo.
- [x] Que el compilador infiere el tipo a partir del valor inicial.

---

### Tipo inferido

5) ¿Cuál es la interpretación correcta de esta declaración?

```csharp
var activo = true;
```

- [ ] `activo` queda sin tipo hasta que el programa se ejecute.
- [ ] `activo` puede cambiar libremente entre tipos distintos durante la ejecución.
- [x] El compilador infiere que `activo` es de tipo `bool`.

---

### Uso de `var`

6) ¿En cuál de estos casos el uso de `var` resulta más claro?

- [x] `var alumnos = new List<string>();`
- [ ] `var dato = ObtenerResultado();`
- [ ] `var resultado = Procesar();`

---

### Alcance

7) ¿Qué pasa con una variable declarada dentro de un `if` cuando termina ese bloque?

- [x] Sale de alcance y ya no puede usarse fuera de ese bloque.
- [ ] Sigue existiendo en todo el programa.
- [ ] Se convierte automáticamente en variable global.

---

## Constantes, campos y miembros estáticos

### `const`

8) ¿Para qué caso corresponde usar `const`?

- [ ] Para un valor que se asigna por primera vez dentro del constructor.
- [x] Para un valor fijo conocido en tiempo de compilación.
- [ ] Para una variable local que cambia dentro de un bloque.

---

### `readonly`

9) ¿En qué caso tiene sentido usar `readonly`?

- [ ] Cuando el valor debe conocerse obligatoriamente en tiempo de compilación.
- [ ] Cuando un campo debe poder cambiarse desde cualquier método.
- [x] Cuando un campo puede asignarse al construir el objeto y luego no debería modificarse.

---

### `const` vs. `readonly`

10) ¿Cuál es la diferencia principal entre `const` y `readonly`?

- [ ] `readonly` solo sirve para variables locales y `const` solo para parámetros.
- [ ] No hay diferencia real entre ambas.
- [x] `const` se fija en compilación y `readonly` puede asignarse en la declaración o en el constructor.

---

### Clases estáticas

11) ¿Cuál es un uso razonable de una clase estática?

- [ ] Guardar todo el código del programa en una única clase gigante.
- [x] Agrupar constantes, configuraciones simples, funciones auxiliares o valores reutilizables.
- [ ] Crear objetos dinámicos que vivan siempre en la stack.

---

### Campo estático

12) En este código, ¿cómo se accede al contador compartido?

```csharp
static class Global {
    public static int ContadorGlobal = 0;
}
```

- [x] `Global.ContadorGlobal`
- [ ] `ContadorGlobal.Global`
- [ ] `new Global().ContadorGlobal`

---

### Tiempo de vida

13) ¿Cuál es la diferencia entre una variable local y una variable estática respecto de su tiempo de vida?

- [x] La variable local vive mientras se ejecuta su bloque; la variable estática vive durante toda la ejecución del programa.
- [ ] La variable estática vive solo dentro de un bloque; la variable local vive durante todo el programa.
- [ ] Ambas viven siempre durante toda la ejecución del programa.

---

### Namespace

14) ¿Para qué sirve un `namespace` en C#?

- [ ] Para convertir una variable local en una variable global.
- [ ] Para copiar objetos por valor automáticamente.
- [x] Para agrupar tipos relacionados y evitar conflictos de nombres.

---

## Memoria y copias

### Stack y heap

15) ¿Con qué se suele asociar el heap?

- [x] Con objetos creados dinámicamente y memoria administrada por el recolector de basura.
- [ ] Con llamadas a métodos, variables locales y datos de vida corta.
- [ ] Con constantes conocidas en tiempo de compilación únicamente.

---

### Tipos por valor

16) ¿Qué imprime este código?

```csharp
int a = 10;
int b = a;
b = 20;

Console.WriteLine(a);
```

- [ ] `20`, porque `a` y `b` apuntan al mismo dato.
- [x] `10`, porque al copiar un tipo por valor se copia el dato.
- [ ] Da error, porque no se puede asignar `a` a `b`.

---

### Tipos por referencia

17) ¿Qué imprime este código?

```csharp
int[] numeros = { 1, 2, 3 };
int[] copia = numeros;

copia[0] = 99;

Console.WriteLine(numeros[0]);
```

- [ ] `1`, porque `copia` es siempre un arreglo independiente.
- [ ] Da error, porque no se puede modificar un arreglo desde otra variable.
- [x] `99`, porque ambas variables apuntan al mismo arreglo.

---

### Copia independiente

18) Si `numeros` es un arreglo, ¿qué instrucción crea una copia independiente?

- [ ] `int[] copia = numeros;`
- [ ] `int[] copia = ref numeros;`
- [x] `int[] copia = (int[])numeros.Clone();`

---

## Tipos básicos y literales

### Literales numéricos

19) ¿Cómo interpreta C# normalmente el literal `19.99`?

- [ ] Como `decimal`.
- [x] Como `double`.
- [ ] Como `int`.

---

### Sufijos de literales

20) ¿Qué literal corresponde claramente a un `decimal`?

- [ ] `10.5`
- [x] `10.5m`
- [ ] `10.5f`

---

### Hexadecimal

21) ¿Qué representa `0xFF` en decimal?

- [x] 255
- [ ] 15
- [ ] 512

---

### Binario

22) ¿Qué valor decimal representa `0b1010`?

- [ ] 8
- [ ] 12
- [x] 10

---

### Separadores visuales

23) ¿Para qué sirve el carácter `_` en un literal como `1_000_000`?

- [ ] Para indicar que el número es `long`.
- [x] Para mejorar la legibilidad sin cambiar el valor numérico.
- [ ] Para separar decimales.
