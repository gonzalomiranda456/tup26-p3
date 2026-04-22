class Compilador {
    public static Nodo Parse(string expresion) {
        throw new NotImplementedException("Implementar el parser para convertir la expresión en un AST.");
    }

    private string texto;
    private int posicion;
    private Token tokenActual;

    private Compilador(string texto) {
        this.texto = texto;
        this.posicion = 0;
        this.tokenActual = LeerToken();
    }
}


enum TokenTipo {
    numero,
    variable,
    suma,
    resta,
    multiplicacion,
    division,
    parentesisAbierto,
    parentesisCerrado,
    finExpresion
}

record Token(TipoToken Tipo, int Valor = 0);



