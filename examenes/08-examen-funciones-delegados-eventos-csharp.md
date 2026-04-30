# Examen de funciones, delegados y eventos en C#

## Funciones, Delegados y Eventos en C#

### Funciones

1) ¿Qué es una función o método en C#?

- [ ] Una variable que solo puede guardar números.
- [x] Un bloque de código con nombre que realiza una tarea, puede recibir datos y devolver un resultado.
- [ ] Un tipo especial que no puede reutilizarse.

---

### Expresión de cuerpo

2) ¿Cuál de estas declaraciones usa expresión de cuerpo para una función de una sola expresión?

- [ ] `static int Sumar(int a, int b) { return a + b; }`
- [ ] `static void Sumar = a + b;`
- [x] `static int Sumar(int a, int b) => a + b;`

---

### Parámetros por valor

3) ¿Qué imprime la última línea?

```csharp
static void Duplicar(int n) {
    n *= 2;
}

int x = 10;
Duplicar(x);
Console.WriteLine(x);
```

- [x] `10`, porque el parámetro recibió una copia del valor.
- [ ] `20`, porque todos los parámetros se pasan por referencia.
- [ ] Da error, porque `int` no puede pasarse a una función.

---

### Referencias copiadas

4) En este código, ¿qué imprime `nums.Count`?

```csharp
static void Modificar(List<int> lista) {
    lista.Add(99);
    lista = new List<int>();
}

var nums = new List<int> { 1, 2, 3 };
Modificar(nums);
Console.WriteLine(nums.Count);
```

- [ ] `0`, porque la reasignación local reemplaza la lista original.
- [x] `4`, porque se modificó el objeto original al agregar `99`.
- [ ] `3`, porque ninguna operación dentro del método afecta a la lista.

---

### `ref`

5) ¿Qué permite `ref` en un parámetro?

- [ ] Pasar una copia de solo lectura.
- [ ] Declarar una variable de salida sin inicializar.
- [x] Hacer que el parámetro sea un alias de la variable original.

---

### `out`

6) ¿Qué caracteriza a un parámetro `out`?

- [x] No necesita estar inicializado antes de la llamada y el método debe asignarlo antes de retornar.
- [ ] Siempre se pasa por valor y no puede modificarse.
- [ ] Solo puede usarse con strings.

---

### Patrón `TryXxx`

7) ¿Qué representa el patrón `TryXxx(out T resultado)` en .NET?

- [ ] Una forma de lanzar excepciones obligatoriamente cuando falla una operación.
- [x] Una forma estándar de intentar una operación que puede fallar sin lanzar excepción.
- [ ] Una forma de declarar constructores opcionales.

---

### `in`

8) ¿Para qué se usa `in` en un parámetro?

- [ ] Para modificar obligatoriamente el valor original.
- [ ] Para devolver dos resultados desde una función.
- [x] Para pasar por referencia de solo lectura, evitando copias en casos útiles.

---

### `params`

9) ¿Qué permite `params` en una función?

- [x] Recibir un número variable de argumentos como un array.
- [ ] Obligar a que todos los argumentos sean nombrados.
- [ ] Evitar que la función tenga retorno.

---

### Sobrecarga

10) ¿Cómo elige el compilador qué sobrecarga llamar?

- [ ] Por el nombre de la variable que recibe el resultado.
- [x] Por los tipos y la cantidad de argumentos.
- [ ] Siempre llama a la primera sobrecarga escrita en el archivo.

---

### Funciones locales

11) ¿Qué caracteriza a una función local?

- [ ] Debe declararse fuera de cualquier clase.
- [ ] Es visible desde todo el proyecto.
- [x] Se declara dentro de otro método y solo es visible en ese método.

---

### Captura de contexto

12) ¿Qué puede hacer una función local no estática?

- [x] Capturar variables del método donde está declarada.
- [ ] Ejecutarse antes de que exista el método que la contiene.
- [ ] Convertirse automáticamente en evento.

---

### Función local `static`

13) ¿Cuál es una ventaja de declarar una función local como `static` cuando no necesita variables externas?

- [ ] Permite capturar todas las variables del contexto.
- [x] Previene capturas accidentales y puede ser más eficiente.
- [ ] Hace que la función sea visible desde cualquier clase.

---

### Recursión

14) ¿Qué necesita siempre una función recursiva?

- [ ] Un parámetro `out`.
- [ ] Un delegado multicast.
- [x] Un caso base para evitar recursión infinita.

---

### Memoización

15) ¿Para qué sirve la memoización en una función recursiva costosa?

- [x] Para almacenar resultados previos y evitar recalcular los mismos valores.
- [ ] Para forzar que la función lance excepción en cada llamada.
- [ ] Para convertir la función en `void`.

---

### Delegados

16) ¿Qué es un delegado en C#?

- [ ] Una clase que solo puede tener campos estáticos.
- [x] Un tipo que representa una referencia a un método con una firma determinada.
- [ ] Un operador para comparar strings.

---

### Delegados multicast

17) ¿Qué permite hacer `+=` sobre un delegado compatible?

- [ ] Borrar todas las funciones asociadas al delegado.
- [ ] Convertir el delegado en `null`.
- [x] Agregar otro método a la lista de invocación del delegado.

---

### `Func<>`

18) En `Func<int, int, int> sumar`, ¿qué indica el último `int`?

- [x] El tipo de retorno.
- [ ] El nombre del método.
- [ ] La cantidad máxima de llamadas.

---

### `Action<>`

19) ¿Cuándo corresponde usar `Action<string>`?

- [ ] Cuando se necesita una función que recibe `string` y retorna `bool`.
- [x] Cuando se necesita una función que recibe `string` y no retorna valor.
- [ ] Cuando se necesita una función sin parámetros que retorna `string`.

---

### `Predicate<T>`

20) ¿A qué equivale conceptualmente `Predicate<int>`?

- [ ] `Action<int>`
- [ ] `Func<int, string>`
- [x] `Func<int, bool>`

---

### Lambdas

21) ¿Qué es una lambda?

- [x] Una función anónima escrita de forma compacta.
- [ ] Un constructor que siempre recibe `params`.
- [ ] Un evento que no admite suscriptores.

---

### Captura en lambdas

22) ¿Qué significa que una lambda captura la variable y no el valor?

- [ ] Que la lambda no puede ver cambios posteriores en esa variable.
- [x] Que si la variable cambia después, la lambda puede ver el nuevo valor.
- [ ] Que la variable capturada se vuelve automáticamente `readonly`.

---

### Funciones de orden superior

23) ¿Qué es una función de orden superior?

- [ ] Una función que solo puede estar en una clase abstracta.
- [ ] Una función que tiene más de diez parámetros.
- [x] Una función que recibe funciones como parámetros o devuelve una función como resultado.

---

### Callback

24) ¿Qué es un callback?

- [x] Una función pasada a otra función para que sea llamada en el momento apropiado.
- [ ] Un método privado que solo se ejecuta al compilar.
- [ ] Una variable que guarda exclusivamente números enteros.

---

### Eventos

25) ¿Qué permite hacer un evento?

- [ ] Reemplazar todas las funciones por lambdas.
- [x] Permitir que un objeto notifique a otros cuando algo ocurre.
- [ ] Invocar cualquier método privado desde fuera de la clase.

---

### Disparo de eventos

26) ¿Por qué se usa `Tick?.Invoke(this, segundos)` al disparar un evento?

- [ ] Para lanzar una excepción si no hay suscriptores.
- [ ] Para borrar todos los suscriptores antes de invocar.
- [x] Para invocar el evento solo si no es `null`.

---

### Evento vs delegado

27) ¿Cuál es una diferencia clave entre un evento y un delegado público?

- [x] Un evento solo puede ser invocado por la clase que lo declara.
- [ ] Un evento no permite `+=` ni `-=`.
- [ ] Un delegado nunca puede ser `null`.

