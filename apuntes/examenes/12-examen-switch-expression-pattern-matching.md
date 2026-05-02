# Examen de switch expression y pattern matching en C#

## Switch Expression y Pattern Matching en C#

---

### Switch expression

1) ¿Cuál es una diferencia central entre una instrucción `switch` clásica y un `switch expression`?

- [ ] El `switch expression` requiere `break` en cada brazo.
- [x] El `switch expression` produce un valor directamente.
- [ ] El `switch expression` usa `default` en lugar de `_`.

---

### Type pattern

2) ¿Qué permite hacer un type pattern como `string s` dentro de un `switch`?

- [ ] Convertir cualquier valor a `string` aunque no sea compatible.
- [ ] Comparar el valor únicamente contra un texto literal.
- [x] Verificar que el valor sea `string` y declararlo en la variable `s`.

---

### Orden de patrones

3) ¿Por qué los patrones más específicos deben escribirse antes que los más generales?

- [x] Porque un patrón general anterior puede capturar el caso y hacer inalcanzable al específico.
- [ ] Porque C# ejecuta siempre primero el último brazo del `switch`.
- [ ] Porque los patrones específicos solo funcionan después del wildcard `_`.

---

### Patrones lógicos y guardias

4) ¿Qué agrega una guardia `when` en un brazo de `switch expression`?

- [ ] Un valor por defecto que se ejecuta antes de todos los patrones.
- [x] Una condición booleana adicional que se evalúa solo si el patrón coincide.
- [ ] Una forma de evitar que el `switch expression` devuelva un valor.

---

### List pattern

5) En un list pattern, ¿qué representa `..`?

- [ ] Un elemento obligatorio que debe valer cero.
- [ ] Una comparación relacional entre dos elementos consecutivos.
- [x] Un segmento con cualquier cantidad de elementos.
