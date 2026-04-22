using System;
using System.Collections.Generic;

namespace Calculadora
{
    // Aca pongo los tipos de cosas que podemos encontrar en la cuenta
    public enum TipoToken
    {
        Numero,
        Suma,
        Resta,
        Multiplicacion,
        Division,
        ParentesisIzquierdo,
        ParentesisDerecho,
        VariableX,
        FinDeLinea // Para saber cuando se termina la formula
    }

    // Esta clase guarda que tipo es y si tiene algun valor (como los numeros)
    public class Token
    {
        public TipoToken Tipo;
        public string Valor;

        public Token(TipoToken tipo, string valor = "")
        {
            Tipo = tipo;
            Valor = valor;
        }
    }
}
