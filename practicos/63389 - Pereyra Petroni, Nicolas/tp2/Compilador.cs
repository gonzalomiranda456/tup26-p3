class Compilador 
{
    public static Nodo Parse(string expresion) 
    {
        var parser = new Parser(expresion);
        return parser.Parsear();
    }
}

class Parser
{
    private string texto;
    private int pos;

    public Parser(string texto)
    {
        this.texto = texto.Replace(" ", "");
        this.pos = 0;
    }

    public Nodo Parsear()
    {
        return ParseExpresion();
    }


    private Nodo ParseExpresion()
    {
        Nodo nodo = ParseTermino();

        while (true)
        {
            if (Match('+'))
            {
                nodo = new SumaNodo(nodo, ParseTermino());
            }
            else if (Match('-'))
            {
                nodo = new RestaNodo(nodo, ParseTermino());
            }
            else
            {
                break;
            }
        }

        return nodo;
    }

    private Nodo ParseTermino()
    {
        Nodo nodo = ParseFactor();

        while (true)
        {
            if (Match('*'))
            {
                nodo = new MultiplicacionNodo(nodo, ParseFactor());
            }
            else if (Match('/'))
            {
                nodo = new DivisionNodo(nodo, ParseFactor());
            }
            else
            {
                break;
            }
        }

        return nodo;
    }

    private bool Match(char c)
    {
        if (pos < texto.Length && texto[pos] == c)
        {
            pos++;
            return true;
        }
        return false;
    }
}