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

    // Esta clase es la que lee la formula y la separa en partes (tokens)
    public class Lexer
    {
        private string _texto;
        private int _posicion;

        public Lexer(string texto)
        {
            _texto = texto;
            _posicion = 0;
        }

        public List<Token> Escanear()
        {
            List<Token> tokens = new List<Token>();

            while (_posicion < _texto.Length)
            {
                char actual = _texto[_posicion];

                // Si es un espacio, lo saltamos y seguimos
                if (char.IsWhiteSpace(actual))
                {
                    _posicion++;
                    continue;
                }

                // Si es una x (o X), guardamos el token de variable
                if (actual == 'x' || actual == 'X')
                {
                    tokens.Add(new Token(TipoToken.VariableX, "x"));
                    _posicion++;
                    continue;
                }

                
                
                throw new Exception("No entiendo que es este simbolo: " + actual);
            }

            // Al final de todo avisamos que terminamos
            tokens.Add(new Token(TipoToken.FinDeLinea));
            return tokens;
        }
    }
}
