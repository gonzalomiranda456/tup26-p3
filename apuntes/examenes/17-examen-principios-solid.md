# Examen de principios SOLID en C#

## Principios SOLID en C#

### Propósito de SOLID

1) ¿Cuál es el objetivo principal de los principios SOLID?

- [ ] Hacer que todo el código use herencia.
- [x] Guiar el diseño para que el código sea más mantenible, extensible, entendible y testeable.
- [ ] Evitar el uso de interfaces y abstracciones.

---

### Cambio de requisitos

2) ¿Por qué SOLID es útil en sistemas reales?

- [ ] Porque el código real nunca cambia.
- [ ] Porque elimina la necesidad de pruebas.
- [x] Porque ayuda a reducir el impacto de cambios, bugs y nuevos requisitos.

---

### Los cinco principios

3) ¿Qué contiene el acrónimo SOLID?

- [x] Cinco principios de diseño orientado a objetos.
- [ ] Cinco tipos primitivos de C#.
- [ ] Cinco patrones obligatorios de base de datos.

---

### SRP

4) ¿Qué afirma el Single Responsibility Principle?

- [ ] Una clase debe tener un solo método.
- [x] Una clase debe tener una sola razón para cambiar.
- [ ] Una clase debe implementar todas las interfaces posibles.

---

### Responsabilidad

5) En SRP, ¿qué es una responsabilidad?

- [ ] Cualquier línea individual de código.
- [ ] Un método privado sin parámetros.
- [x] Un conjunto coherente de funciones que cambian juntas.

---

### Señal de SRP

6) ¿Cuál es una señal de que una clase puede estar violando SRP?

- [x] Cambia por motivos muy distintos, como reglas de negocio, persistencia y notificación.
- [ ] Tiene un constructor.
- [ ] Expone una propiedad de solo lectura.

---

### Aplicación de SRP

7) ¿Qué busca una refactorización guiada por SRP?

- [ ] Dividir cada método en una clase distinta sin mirar el contexto.
- [x] Separar responsabilidades que cambian por razones diferentes.
- [ ] Reemplazar todas las clases por métodos estáticos.

---

### Exceso de separación

8) ¿Qué riesgo aparece si se aplica SRP de forma exagerada?

- [ ] El código se vuelve imposible de compilar.
- [ ] Todas las clases quedan automáticamente acopladas.
- [x] Se pueden crear demasiadas clases pequeñas y anémicas, difíciles de seguir.

---

### OCP

9) ¿Qué significa Open/Closed Principle?

- [x] El software debe estar abierto a extensión y cerrado a modificación.
- [ ] El software debe estar cerrado a extensión y abierto a modificación.
- [ ] Toda clase debe ser pública y mutable.

---

### Extensión

10) ¿Qué busca OCP al agregar una nueva variante de comportamiento?

- [ ] Modificar siempre el método central existente.
- [x] Agregar código nuevo sin tocar código ya probado y funcionando.
- [ ] Borrar las abstracciones para simplificar.

---

### Señal de OCP

11) ¿Cuál es una señal frecuente de violación de OCP?

- [ ] Usar polimorfismo.
- [ ] Crear una interfaz pequeña.
- [x] Tener que agregar un nuevo `if`, `else if` o `switch` cada vez que aparece una variante.

---

### Polimorfismo y OCP

12) ¿Cómo ayuda el polimorfismo a cumplir OCP?

- [x] Permite agregar implementaciones nuevas detrás de una abstracción común.
- [ ] Obliga a que todas las clases tengan los mismos campos privados.
- [ ] Impide que existan métodos abstractos.

---

### Strategy

13) ¿Qué aporta una estrategia intercambiable al diseño?

- [ ] Hace que una clase dependa de más detalles concretos.
- [x] Permite cambiar una regla de comportamiento mediante composición.
- [ ] Elimina la necesidad de validar datos.

---

### Herencia vs. composición

14) ¿Cuándo conviene preferir composición sobre herencia?

- [ ] Cuando solo se quiere reutilizar o variar comportamiento sin una relación "es un" estable.
- [ ] Cuando se necesita que una subclase reemplace correctamente a su base.
- [x] Cuando el comportamiento debe ser flexible y no depende de una jerarquía conceptual fuerte.

---

### LSP

15) ¿Qué exige Liskov Substitution Principle?

- [x] Que una subclase pueda usarse donde se espera la clase base sin romper el programa.
- [ ] Que toda clase tenga una única responsabilidad.
- [ ] Que cada interfaz tenga muchos métodos.

---

### Contrato de la clase base

16) ¿Qué debe respetar una subclase para cumplir LSP?

- [ ] Solo el nombre de la clase base.
- [x] Las expectativas, precondiciones, postcondiciones e invariantes del tipo base.
- [ ] La ubicación física del archivo.

---

### Precondiciones

17) ¿Qué significa "no fortalecer precondiciones" en LSP?

- [ ] La subclase puede exigir condiciones más restrictivas que la base.
- [x] La subclase no debe exigir más de lo que exige la clase base para usar una operación.
- [ ] La subclase debe eliminar todas las validaciones.

---

### Postcondiciones

18) ¿Qué significa "no debilitar postcondiciones" en LSP?

- [ ] La subclase puede prometer menos que la base.
- [ ] La subclase debe devolver siempre `null`.
- [x] La subclase debe cumplir al menos las garantías que promete la clase base.

---

### Señal de LSP

19) ¿Cuál es una señal de posible violación de LSP?

- [x] Código que pregunta por subtipos concretos para evitar comportamientos inesperados.
- [ ] Código que usa una interfaz pequeña.
- [ ] Código que recibe una abstracción por constructor.

---

### ISP

20) ¿Qué afirma Interface Segregation Principle?

- [ ] Los clientes deben depender de una interfaz grande y completa.
- [x] Los clientes no deben depender de métodos que no usan.
- [ ] Toda interfaz debe tener estado interno.

---

### Interfaces pequeñas

21) ¿Qué diseño se alinea mejor con ISP?

- [ ] Una interfaz enorme con todas las operaciones posibles.
- [ ] Una clase concreta usada directamente por todos los clientes.
- [x] Varias interfaces pequeñas, específicas y cohesivas.

---

### Métodos no soportados

22) ¿Qué problema intenta evitar ISP?

- [x] Implementaciones obligadas a lanzar errores en métodos que no tienen sentido para ellas.
- [ ] El uso de métodos públicos.
- [ ] El uso de nombres descriptivos.

---

### Cliente y contrato

23) Según ISP, ¿de qué debería depender un cliente?

- [ ] De todos los métodos que existen en el sistema.
- [x] Solo del contrato que realmente necesita usar.
- [ ] De una clase concreta con todas las capacidades.

---

### Relación entre ISP y LSP

24) ¿Cómo puede ISP ayudar a LSP?

- [ ] Haciendo que todas las clases hereden de una sola base.
- [x] Evitando que los tipos prometan operaciones que luego no pueden cumplir correctamente.
- [ ] Eliminando la necesidad de polimorfismo.

---

### DIP

25) ¿Qué afirma Dependency Inversion Principle?

- [x] Los módulos de alto nivel y bajo nivel deben depender de abstracciones.
- [ ] Los módulos de alto nivel deben crear directamente todas sus dependencias concretas.
- [ ] Las abstracciones deben depender de detalles técnicos.

---

### Módulos de alto nivel

26) ¿Qué se considera un módulo de alto nivel en DIP?

- [ ] Un detalle técnico como escribir un archivo o enviar un correo.
- [x] Código que expresa reglas o lógica importante del negocio.
- [ ] Una clase que solo contiene constantes.

---

### Detalles técnicos

27) ¿Qué se considera un módulo de bajo nivel en DIP?

- [ ] La política central del dominio.
- [ ] Una regla de negocio abstracta.
- [x] Un detalle concreto como persistencia, envío de mensajes o acceso a un servicio externo.

---

### Inyección de dependencias

28) ¿Qué permite la inyección de dependencias?

- [x] Entregar implementaciones concretas desde afuera a código que depende de abstracciones.
- [ ] Hacer que una clase construya internamente todos sus detalles con `new`.
- [ ] Evitar que existan interfaces.

---

### Testeabilidad

29) ¿Por qué DIP mejora la testeabilidad?

- [ ] Porque elimina la necesidad de probar casos de error.
- [ ] Porque obliga a usar servicios reales en los tests.
- [x] Porque permite reemplazar detalles externos por dobles, mocks o implementaciones en memoria.

---

### Desacoplamiento

30) ¿Qué ventaja aporta depender de abstracciones?

- [x] Cambiar una implementación concreta afecta menos al código que la usa.
- [ ] El código queda más acoplado a una única tecnología.
- [ ] Las pruebas se vuelven imposibles.

---

### Señales de refactor

31) ¿Cuál de estas señales indica que el diseño puede necesitar refactorización?

- [ ] Clases con nombres claros y responsabilidades separadas.
- [x] Clases enormes, `if/else` sobre tipos o creación directa de infraestructura en lógica de negocio.
- [ ] Interfaces específicas para capacidades puntuales.

---

### Criterio práctico

32) ¿Cómo deben aplicarse los principios SOLID?

- [ ] Como dogmas obligatorios aunque el problema sea simple.
- [x] Como guías, con criterio, cuando la complejidad del diseño lo justifica.
- [ ] Solo después de eliminar todas las abstracciones.
