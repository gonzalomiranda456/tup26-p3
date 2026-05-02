# Fundamentos de C#

---

## Variables, memoria y tipos básicos en C#

### Tipado estatico

11) ¿Que significa que C# tenga tipado estatico?
- [ ] Que todas las variables deben ser `static`.
- [x] Que el tipo de cada variable se conoce y controla antes de ejecutar, principalmente en compilacion.
- [ ] Que el tipo de una variable puede cambiar durante la ejecucion.

---

### Tipado fuerte

12) ¿Que muestra mejor la idea de tipado fuerte?
```csharp
int numero = 10;
string texto = "20";
```
- [ ] `string` e `int` son equivalentes.
- [ ] `numero = texto;` funciona siempre porque C# convierte automaticamente.
- [x] Hay que convertir explicitamente si se quiere pasar de `string` a `int`.

---

### Uso de var

13) ¿Que significa realmente `var` en C#?
- [ ] Que la variable cambia de tipo segun lo que se le asigne despues.
- [ ] Que la variable no tiene tipo.
- [x] Que el compilador infiere el tipo a partir del valor inicial.

---

### Const y readonly

14) ¿Cual es la diferencia principal entre `const` y `readonly`?
- [ ] `readonly` solo sirve para variables locales y `const` solo para parametros.
- [ ] No hay diferencia real entre ambas.
- [x] `const` se fija en compilacion y `readonly` se puede asignar en la declaracion o en el constructor.

---

### Alcance

15) ¿Que pasa con una variable declarada dentro de un `if` cuando termina ese bloque?
- [x] Sale de alcance y ya no puede usarse fuera de ese bloque.
- [ ] Sigue existiendo en todo el programa.
- [ ] Se convierte automaticamente en variable global.

---

## Tipos de datos y conversiones en C#

### Literales numericos

16) ¿Como interpreta C# normalmente el literal `19.99`?
- [ ] Como `decimal`.
- [x] Como `double`.
- [ ] Como `int`.

---

### Sufijos de literales

17) ¿Que literal corresponde claramente a un `decimal`?
- [ ] `10.5`
- [x] `10.5m`
- [ ] `10.5f`

---

### Hexadecimal

18) ¿Que representa `0xFF` en decimal?
- [x] 255
- [ ]  15
- [ ] 512

---

### Binario

19) ¿Que valor decimal representa `0b1010`?
- [ ]  8
- [ ] 12
- [x] 10

---

### Separadores visuales

20) ¿Para que sirve el caracter `_` en un literal como `1_000_000`?
- [ ] Para indicar que el numero es `long`.
- [x] Para mejorar la legibilidad sin cambiar el valor numerico.
- [ ] Para separar decimales.

