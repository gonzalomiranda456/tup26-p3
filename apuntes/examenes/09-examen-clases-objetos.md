# Examen de clases y objetos en C#

## Clases y objetos en C#

### Concepto de clase

1) ¿Qué describe una clase en C#?

- [ ] Solo una lista de valores numéricos consecutivos.
- [x] La estructura y el comportamiento de un objeto: qué datos guarda y qué operaciones realiza.
- [ ] Un archivo que no puede contener métodos.

---

### Orden de miembros

2) ¿Cuál es un orden convencional para organizar los bloques dentro de una clase?

- [ ] Métodos privados, propiedades, constructores, campos privados, métodos públicos.
- [ ] Propiedades, métodos públicos, campos privados, métodos privados, constructores.
- [x] Campos privados, constructores, propiedades, métodos públicos, métodos privados.

---

### Constructores

3) ¿Qué caracteriza a un constructor?

- [x] Se invoca al crear un objeto con `new`, tiene el mismo nombre que la clase y no tiene tipo de retorno.
- [ ] Se ejecuta cada vez que se lee una propiedad.
- [ ] Siempre debe devolver una instancia con `return`.

---

### Constructor por defecto

4) ¿Qué ocurre con el constructor por defecto cuando una clase declara al menos un constructor propio?

- [ ] El compilador genera dos constructores por defecto.
- [x] El constructor por defecto implícito desaparece y hay que declararlo a mano si se necesita.
- [ ] La clase deja de poder instanciarse con `new`.

---

### Constructor delegado

5) ¿Para qué se usa `: this(...)` en un constructor?

- [ ] Para llamar al constructor de la clase base.
- [ ] Para convertir una clase en estática.
- [x] Para llamar a otro constructor de la misma clase y reutilizar lógica de inicialización.

---

### Primary constructor

6) En una clase con primary constructor, ¿qué son sus parámetros?

```csharp
public class Punto(double x, double y) {
    public double X => x;
    public double Y => y;
}
```

- [x] Variables disponibles en todo el cuerpo de la clase.
- [ ] Propiedades públicas creadas automáticamente, igual que en un `record`.
- [ ] Campos estáticos compartidos por todas las instancias.

---

### Inicializador de objeto

7) ¿Qué permite hacer un inicializador de objeto?

- [ ] Crear una clase sin constructor.
- [x] Asignar propiedades después de construir el objeto, de forma concisa y legible.
- [ ] Ejecutar un método privado desde fuera de la clase.

---

### Propiedad completa

8) ¿Qué ventaja ofrece una propiedad con `get` y `set` explícitos?

- [ ] Hace que el campo sea público automáticamente.
- [ ] Impide cualquier validación al asignar valores.
- [x] Permite agregar lógica, como validar antes de guardar el valor.

---

### Auto-propiedad

9) ¿Qué significa esta propiedad?

```csharp
public int Stock { get; private set; }
```

- [x] Se puede leer desde fuera, pero solo se puede escribir desde dentro de la clase.
- [ ] Se puede escribir libremente desde cualquier parte.
- [ ] Solo se puede asignar en un inicializador `new { }`.

---

### `init`

10) ¿Qué permite una propiedad `{ get; init; }`?

- [ ] Modificar la propiedad en cualquier momento después de construido el objeto.
- [x] Asignar la propiedad durante la creación del objeto, pero no modificarla después.
- [ ] Convertir la propiedad en un método estático.

---

### `required`

11) ¿Qué efecto tiene `required` en una propiedad?

- [ ] Hace que la propiedad sea opcional si tiene valor `null`.
- [ ] Permite omitirla siempre que exista un constructor.
- [x] Obliga a asignarla en el inicializador y el compilador lo verifica.

---

### Indexadores

12) ¿Para qué sirve un indexador como `public string this[int index]`?

- [x] Para permitir acceso con sintaxis tipo `objeto[indice]`.
- [ ] Para hacer que una clase herede de `string`.
- [ ] Para ejecutar un constructor estático.

---

### Parámetros opcionales

13) ¿Qué valor usa `exp` en esta llamada?

```csharp
public double Potencia(double baseValor, double exp = 2) => Math.Pow(baseValor, exp);

calc.Potencia(2);
```

- [ ] `0`
- [x] `2`
- [ ] `null`

---

### Sobrecarga

14) ¿Qué es la sobrecarga de métodos?

- [ ] Reemplazar un método virtual en una clase derivada.
- [x] Definir varios métodos con el mismo nombre pero distintos parámetros.
- [ ] Ocultar un método base usando `new`.

---

### Miembros estáticos

15) ¿A qué pertenece un miembro `static`?

- [x] A la clase, no a una instancia particular.
- [ ] A cada objeto creado con `new`.
- [ ] Solo a las clases abstractas.

---

### Clase estática

16) ¿Qué caracteriza a una clase marcada como `static`?

- [ ] Puede instanciarse con `new`, pero solo una vez.
- [ ] Puede contener campos de instancia y métodos estáticos.
- [x] No puede instanciarse y solo puede contener miembros estáticos.

---

### Constructor estático

17) ¿Cuándo se ejecuta un constructor estático?

- [ ] Cada vez que se crea una instancia.
- [x] Una sola vez, antes del primer uso de la clase.
- [ ] Cada vez que se llama a cualquier método de instancia.

---

### Operadores

18) Si una clase sobrecarga `==`, ¿qué más debería sobrecargar según el apunte?

- [x] `!=`, `Equals` y `GetHashCode`.
- [ ] Solo `ToString`.
- [ ] Todos los operadores aritméticos.

---

### Conversiones

19) ¿Cuándo conviene usar una conversión `explicit`?

- [ ] Cuando la conversión es siempre segura y natural.
- [ ] Cuando no se quiere escribir ningún cast.
- [x] Cuando la conversión puede perder información o fallar.

---

### Herencia

20) ¿Qué relación debería modelar la herencia entre una clase derivada y una clase base?

- [x] Una relación "es un".
- [ ] Una relación "tiene muchos".
- [ ] Una relación "usa temporalmente".

---

### `base(...)`

21) ¿Para qué se usa `base(...)` en el constructor de una clase derivada?

- [ ] Para llamar a otro constructor de la misma clase.
- [x] Para inicializar la clase base.
- [ ] Para impedir que la clase sea heredada.

---

### `protected`

22) ¿Qué permite el modificador `protected`?

- [ ] Acceso desde cualquier parte del programa.
- [ ] Acceso solo desde el mismo método.
- [x] Acceso dentro de la clase y desde sus clases derivadas.

---

### `sealed`

23) ¿Qué efecto tiene `sealed` aplicado a una clase?

- [x] Impide que otras clases hereden de ella.
- [ ] Obliga a que todos sus métodos sean abstractos.
- [ ] Permite herencia múltiple.

---

### Polimorfismo

24) En este código, ¿qué se imprime al llamar `obj.A()`?

```csharp
public class Base {
    public virtual void A() => Console.WriteLine("Base.A");
}

public class Derivada : Base {
    public override void A() => Console.WriteLine("Derivada.A");
}

Base obj = new Derivada();
obj.A();
```

- [ ] `Base.A`, porque importa el tipo declarado de la variable.
- [x] `Derivada.A`, porque `override` usa despacho dinámico según el tipo real.
- [ ] Da error, porque una variable `Base` no puede apuntar a `Derivada`.

---

### `new` en métodos

25) ¿Qué diferencia tiene `new` frente a `override` en un método derivado?

- [ ] `new` participa del despacho dinámico igual que `override`.
- [x] `new` oculta el miembro base y la versión ejecutada depende del tipo declarado de la variable.
- [ ] `new` solo puede usarse en constructores.

---

### Clase abstracta

26) ¿Qué caracteriza a una clase `abstract`?

- [x] No puede instanciarse directamente y puede obligar a subclases a implementar métodos abstractos.
- [ ] Debe ser siempre `static`.
- [ ] No puede tener métodos concretos.

---

### Abstracta vs. interfaz

27) Según la regla práctica del apunte, ¿cuándo conviene usar una clase abstracta?

- [ ] Cuando solo se comparte un contrato de comportamiento sin estado.
- [x] Cuando los tipos comparten estado y comportamiento base.
- [ ] Cuando se necesita herencia múltiple de clases.

---

### Inmutabilidad

28) ¿Qué hace un método mutador en una clase inmutable bien diseñada?

- [ ] Modifica directamente el estado interno del objeto actual.
- [ ] Convierte el objeto en `static`.
- [x] Retorna un nuevo objeto en lugar de modificar el actual.

---

### DTO

29) ¿Qué es un DTO?

- [x] Un objeto cuyo propósito es transportar datos entre capas, sin comportamiento de negocio.
- [ ] Una clase abstracta que define operadores aritméticos.
- [ ] Un método de extensión para `string`.

---

### Extension methods

30) ¿Cómo se declaran los métodos de extensión clásicos antes de C# 14?

- [ ] En cualquier clase de instancia, usando `base` en el primer parámetro.
- [x] En clases estáticas, usando `this` en el primer parámetro.
- [ ] Únicamente dentro de una clase abstracta.
