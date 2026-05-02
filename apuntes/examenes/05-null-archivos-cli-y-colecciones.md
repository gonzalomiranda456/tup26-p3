# Examen de null, archivos, CLI y colecciones en C#

## Null y tipos anulables en C#

---

### Tipos que aceptan null

1) ¿Cuál de estas declaraciones es válida según el apunte?
```csharp
string texto = null;
int numero = null;
int? edad = null;
```
- [ ] Sólo la segunda.
- [ ] Las tres.
- [x] La primera y la tercera.

---

### Nullable value types

2) ¿Qué significa `T?` cuando `T` es un tipo por valor?
- [x] Que ese tipo puede contener un valor de `T` o `null`.
- [ ] Que el compilador lo transforma siempre en `string`.
- [ ] Que el valor se vuelve obligatorio al construir el objeto.

---

### Acceso condicional

3) ¿Qué hace el operador `?.`?
- [ ] Convierte automáticamente un valor `null` en cadena vacía.
- [x] Accede a un miembro sólo si el objeto no es `null`; si es `null`, devuelve `null`.
- [ ] Obliga al compilador a tratar la variable como no anulable.

---

### Asignación con coalescencia nula

4) ¿Qué hace `??=` en C#?
- [ ] Reemplaza siempre el valor de la variable por el de la derecha.
- [ ] Compara dos variables y devuelve `true` si ambas son `null`.
- [x] Asigna el valor de la derecha sólo si la variable de la izquierda es `null`.

---

### Buenas prácticas

5) Según el apunte, ¿qué conviene preferir para verificar si algo es `null`?
- [x] `valor is null` o `valor is not null`
- [ ] `valor!`
- [ ] `valor = null`

---

## Archivos de texto, rutas y encoding en C#

### Lectura por defecto

6) ¿Qué encoding usa por defecto `File.ReadAllText("datos.txt")` según el apunte?
- [ ] ASCII
- [ ] Unicode UTF-16
- [x] UTF-8

---

### Escritura al final

7) Si querés agregar texto al final de un archivo sin reemplazar su contenido, ¿qué método conviene usar?
- [x] `File.AppendAllText(...)`
- [ ] `File.ReadAllText(...)`
- [ ] `Directory.GetFiles(...)`

---

### Existencia de archivos

8) ¿Qué devuelve `File.Exists("entrada.txt")`?
- [ ] El contenido del archivo si existe.
- [x] Un `bool` indicando si el archivo existe o no.
- [ ] Una excepción cuando el archivo no existe.

---

### Listado de archivos

9) ¿Para qué sirve `Directory.GetFiles("mis-archivos", "*.txt")`?
- [ ] Para borrar todos los `.txt` de la carpeta.
- [ ] Para crear archivos `.txt` nuevos.
- [x] Para obtener un array con las rutas de los archivos `.txt` de esa carpeta.

---

### Crear carpetas

10) ¿Qué comportamiento destaca el apunte sobre `Directory.CreateDirectory("datos/salidas/2026")`?
- [ ] Sólo funciona si la carpeta padre ya existe.
- [ ] Borra la carpeta anterior y la crea de nuevo.
- [x] Puede crear una ruta completa y no falla si la carpeta ya existe.

---

## Tutorial de CLI en C#: `sumx` en un solo archivo

### Hoja de ruta

11) En la hoja de ruta de `sumx`, ¿qué se hace antes de construir el reporte?
- [ ] Escribir el output final.
- [ ] Mostrar la ayuda del programa.
- [x] Calcular las sumas.

---

### Modelo de datos

12) ¿Por qué el tutorial usa un `record AppConfig(...)`?
- [x] Porque es una forma compacta e inmutable de representar la configuración del comando.
- [ ] Porque un `record` permite heredar de múltiples clases.
- [ ] Porque un `record` sólo puede contener números y strings.

---

### Parseo de argumentos

13) ¿Para qué se pasa `ref int i` a la función `Next`?
- [ ] Para que `Next` convierta el argumento en `int`.
- [x] Para que `Next` pueda avanzar el índice del recorrido de argumentos.
- [ ] Para que `Next` detecte automáticamente `--help`.

---

### Entrada y salida

14) ¿Qué hace `ReadInput` cuando `filePath == null`?
- [ ] Lanza siempre una excepción.
- [ ] Crea un archivo temporal vacío.
- [x] Lee desde `Console.In`, lo que permite usar stdin y redirección.

---

### Parseo de CSV

15) ¿Qué devuelve `ParseCsv(string content)` en el tutorial?
- [x] Una tupla con los encabezados y una lista de filas representadas como diccionarios.
- [ ] Un único string ya formateado como reporte.
- [ ] Sólo un `Dictionary<string, double>` con las sumas.

---

## Tipos compuestos y colecciones en C# (copia de trabajo)

### Inmutabilidad de string

16) ¿Qué pasa cuando se ejecuta `ToUpper()` sobre un `string`?
- [ ] El string original se modifica en el mismo objeto.
- [x] Se crea un nuevo string y el original permanece igual.
- [ ] Sólo funciona si el string fue declarado con `var`.

---

### Tuplas

17) ¿En qué caso recomienda el apunte usar tuplas?
- [ ] Cuando el dato va a circular por muchas capas del sistema.
- [x] Cuando querés agrupar datos temporalmente o devolver múltiples valores de un método.
- [ ] Cuando necesitás comportamiento complejo y métodos propios.

---

### Arrays

18) ¿Cuál es una característica clave de un array en C#?
- [x] Tiene tamaño fijo definido al momento de crearlo.
- [ ] Puede crecer y reducirse automáticamente como `List<T>`.
- [ ] Sólo puede contener `string`.

---

### List<T>

19) ¿Qué ventaja principal tiene `List<T>` frente a un array?
- [ ] Guarda pares clave → valor.
- [ ] Es siempre inmutable.
- [x] Puede crecer o reducirse en tiempo de ejecución.

---

### Record

20) ¿Qué resalta el apunte sobre `record` frente a `class` en el ejemplo dado?
- [ ] `record` no puede tener propiedades calculadas.
- [ ] `record` compara por referencia igual que `class`.
- [x] `record` ofrece igualdad por valor, mientras que la `class` del ejemplo compara por referencia.
