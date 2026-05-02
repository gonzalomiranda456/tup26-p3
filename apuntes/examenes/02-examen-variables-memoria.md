# Examen de variables y manejo de memoria en C#

## Variables y manejo de memoria en C#

### Asignación

1) En C#, ¿qué significa la instrucción `x = x + 1;` si antes `x` tenía el valor `5`?

- [ ] Que se intenta escribir una igualdad matemática imposible.
- [x] Que se toma el valor actual de `x`, se le suma `1` y se guarda el resultado en `x`.
- [ ] Que se crea una nueva variable llamada `x + 1`.

---

### Inferencia con `var`

2) ¿Cuál es la interpretación correcta de esta declaración?

```csharp
var activo = true;
```

- [ ] `activo` queda sin tipo hasta que el programa se ejecute.
- [ ] `activo` puede cambiar libremente entre tipos distintos durante la ejecución.
- [x] El compilador infiere que `activo` es de tipo `bool`.

---

### Uso de `var`

3) ¿En cuál de estos casos el uso de `var` resulta más claro según el criterio del apunte?

- [x] `var alumnos = new List<string>();`
- [ ] `var dato = ObtenerResultado();`
- [ ] `var resultado = Procesar();`

---

### `const`

4) ¿Para qué caso corresponde usar `const`?

- [ ] Para un valor que se asigna por primera vez dentro del constructor.
- [x] Para un valor fijo conocido en tiempo de compilación.
- [ ] Para una variable local que cambia dentro de un bloque.

---

### `readonly`

5) En el siguiente código, ¿por qué tiene sentido usar `readonly`?

```csharp
class Persona {
    public readonly DateTime FechaCreacion;

    public Persona() {
        FechaCreacion = DateTime.Now;
    }
}
```

- [ ] Porque `FechaCreacion` es un valor conocido en tiempo de compilación.
- [ ] Porque `readonly` permite cambiar el campo desde cualquier método.
- [x] Porque el valor se fija al construir el objeto y luego no debería modificarse.

---

### Stack y heap

6) Según la aproximación del apunte, ¿con qué se suele asociar el heap?

- [x] Con objetos creados dinámicamente y memoria administrada por el recolector de basura.
- [ ] Con llamadas a métodos, variables locales y datos de vida corta.
- [ ] Con constantes conocidas en tiempo de compilación únicamente.

---

### Tipos por valor

7) ¿Qué imprime este código?

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

8) ¿Qué imprime este código?

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

### Alcance

9) En este código, ¿qué ocurre con la última línea?

```csharp
void Metodo() {
    if (true) {
        int y = 20;
    }

    Console.WriteLine(y);
}
```

- [x] Da error, porque `y` solo existe dentro del bloque `if`.
- [ ] Imprime `20`, porque `y` existe en todo el método.
- [ ] Imprime `0`, porque las variables fuera de alcance vuelven a su valor inicial.

---

### Clases estáticas

10) ¿Cuál es un uso razonable de una clase estática según el apunte?

- [ ] Guardar todo el código del programa en una única clase gigante.
- [x] Agrupar constantes, configuraciones simples, funciones auxiliares o valores reutilizables.
- [ ] Crear objetos dinámicos que vivan siempre en la stack.

---

### Namespace

11) ¿Para qué sirve un `namespace` en C#?

- [ ] Para convertir una variable local en una variable global.
- [ ] Para copiar objetos por valor automáticamente.
- [x] Para agrupar tipos relacionados y evitar conflictos de nombres.

---

### Tiempo de vida

12) ¿Cuál es la diferencia entre una variable local y una variable estática respecto de su tiempo de vida?

- [x] La variable local vive mientras se ejecuta su bloque; la variable estática vive durante toda la ejecución del programa.
- [ ] La variable estática vive solo dentro de un bloque; la variable local vive durante todo el programa.
- [ ] Ambas viven siempre durante toda la ejecución del programa.

---

### Campo estático

13) En este código, ¿cómo se accede al contador compartido?

```csharp
static class Global {
    public static int ContadorGlobal = 0;
}
```

- [x] `Global.ContadorGlobal`
- [ ] `ContadorGlobal.Global`
- [ ] `new Global().ContadorGlobal`

---

### Copia independiente

14) Si `numeros` es un arreglo, ¿qué instrucción crea una copia independiente según el apunte?

- [ ] `int[] copia = numeros;`
- [ ] `int[] copia = ref numeros;`
- [x] `int[] copia = (int[])numeros.Clone();`
