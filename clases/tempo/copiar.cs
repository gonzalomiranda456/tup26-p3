
if(args.Length > 0) {
    Console.SetIn(new StreamReader(args[0]));
}
if(args.Length > 1) {
    Console.SetOut(new StreamWriter(args[1]) { AutoFlush = true });
}

var linea = "";
while( (linea = Console.ReadLine()) is not null) {
    if(linea.Equals("salir", StringComparison.OrdinalIgnoreCase)) {
        Environment.Exit(0);
        break;
    }
    Console.WriteLine(linea.ToUpper());
    Console.Out.Flush();
}
