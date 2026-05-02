# Examen de strings en C#

## Strings en C#

--- 

### Tipo `string`

1) ¿Qué representa un `string` en C#?

- [ ] Un número entero usado para almacenar códigos Unicode.
- [x] Texto, aunque internamente sea un tipo de referencia con comportamiento especial.
- [ ] Una lista mutable de caracteres que se modifica siempre en el mismo objeto.

---

### Interpolación

2) ¿Qué valor queda en `mensaje`?

```csharp
string nombre = "Ada";
int edad = 20;
string mensaje = $"{nombre} tiene {edad} años";
```

- [ ] `"{nombre} tiene {edad} años"`
- [ ] `"Ada tiene edad años"`
- [x] `"Ada tiene 20 años"`

---

### Verbatim strings

3) ¿Para qué sirve el prefijo `@` en este string?

```csharp
string ruta = @"C:\Users\Ada\Documentos";
```

- [x] Para escribir barras invertidas sin escaparlas.
- [ ] Para convertir la ruta en una interpolación automática.
- [ ] Para hacer que el string sea mutable.

---

### Raw string literals

4) ¿Cuál es una ventaja de usar `"""` para escribir JSON en C#?

- [ ] Obliga a escribir todas las comillas dobles con `\"`.
- [x] Permite escribir JSON con saltos de línea y comillas dobles sin escaparlas.
- [ ] Convierte automáticamente el JSON en un objeto.

---

### Inmutabilidad

5) ¿Qué ocurre en este código?

```csharp
string nombre = "Ana";
nombre = nombre + " María";
```

- [ ] Se modifica el objeto original `"Ana"` carácter por carácter.
- [ ] C# prohíbe concatenar strings porque son inmutables.
- [x] Se crea un nuevo string y la variable `nombre` pasa a apuntar a ese nuevo valor.

---

### `StringBuilder`

6) ¿Cuándo conviene usar `StringBuilder`?

- [x] Cuando se construye mucho texto con muchas concatenaciones, por ejemplo dentro de un bucle.
- [ ] Siempre que haya que comparar dos strings.
- [ ] Cuando se necesita convertir un número a texto una sola vez.

---

### Conversión a texto

7) En una interpolación como `$"El punto es {p}"`, ¿qué permite que un objeto propio tenga una representación textual personalizada?

- [ ] Definir una propiedad llamada `Text`.
- [x] Sobrescribir el método `ToString()`.
- [ ] Convertir el objeto a `char`.

---

### Igualdad

8) ¿Qué imprime este código?

```csharp
string a = "hola";
string b = "hola";

Console.WriteLine(a == b);
```

- [ ] `false`, porque `string` siempre compara referencias con `==`.
- [ ] Da error, porque los strings no se comparan con `==`.
- [x] `true`, porque en `string` `==` compara el contenido.

---

### Comparación sin mayúsculas

9) ¿Qué opción es adecuada para comparar textos ignorando mayúsculas en un caso técnico como claves o comandos?

- [x] `StringComparison.OrdinalIgnoreCase`
- [ ] `StringComparison.CurrentCulture`
- [ ] `CompareTo` sin parámetros

---

### Null y vacío

10) ¿Qué método devuelve `true` para `null`, `""` y `"   "`?

- [ ] `string.IsNullOrEmpty`
- [x] `string.IsNullOrWhiteSpace`
- [ ] `TrimStart`

---

### Métodos comunes

11) ¿Qué devuelve `IndexOf` cuando no encuentra el texto buscado?

- [ ] `0`
- [ ] `null`
- [x] `-1`

---

### Unicode y emojis

12) ¿Por qué `"😀".Length` devuelve `2` en C#?

- [x] Porque `Length` cuenta unidades `char` de 16 bits, y ese emoji usa un par sustituto.
- [ ] Porque el emoji contiene dos caracteres visibles.
- [ ] Porque todos los strings en C# duplican su longitud.

---

### Recorrido Unicode

13) ¿Qué herramienta permite recorrer code points Unicode completos, uniendo correctamente los pares sustitutos?

- [ ] `foreach (char c in texto)`
- [x] `texto.EnumerateRunes()`
- [ ] `texto.Trim()`

---

### Longitud visible

14) ¿Qué permite obtener `new StringInfo(texto).LengthInTextElements`?

- [ ] La cantidad de bytes ocupados por el string.
- [ ] La cantidad de líneas del string.
- [x] La cantidad de caracteres visibles reales.

---

### `Trim`

15) ¿Qué imprime este código?

```csharp
string texto = "  hola  ";
Console.WriteLine(texto.Trim());
```

- [x] `"hola"`
- [ ] `"hola  "`
- [ ] `"  hola"`

---

### `Split`

16) ¿Qué resultado produce `datos.Split(',')`?

```csharp
string datos = "Ana,Luis,Pedro";
string[] nombres = datos.Split(',');
```

- [ ] Un único string: `"Ana Luis Pedro"`.
- [x] Un array con `"Ana"`, `"Luis"` y `"Pedro"`.
- [ ] Un número con la cantidad de comas encontradas.

---

### `string.Join`

17) ¿Qué valor queda en `resultado`?

```csharp
string[] nombres = { "Ana", "Luis", "Pedro" };
string resultado = string.Join(", ", nombres);
```

- [ ] `"AnaLuisPedro"`
- [ ] `"Ana,Luis,Pedro"`
- [x] `"Ana, Luis, Pedro"`

---

### Rangos

18) ¿Qué imprime este código?

```csharp
string texto = "Programación III";
Console.WriteLine(texto[13..]);
```

- [ ] `"Programación"`
- [x] `"III"`
- [ ] `"n III"`

---

### Normalización Unicode

19) ¿Para qué puede servir `Normalize()` al comparar strings?

- [ ] Para quitar todos los acentos del texto.
- [ ] Para convertir siempre el texto a mayúsculas.
- [x] Para unificar representaciones Unicode distintas que se ven igual.
