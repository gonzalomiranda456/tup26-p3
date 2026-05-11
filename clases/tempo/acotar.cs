int max = args.Length > 0 ? int.Parse(args[0]) : 20;
var linea = "";
while ((linea = Console.ReadLine()) is not null) {
    while(linea.Length > max) {
        Console.WriteLine(linea[..max]);
        Console.Out.Flush();
        linea = linea[max..];
    }
    if(linea.Length > 0) {
        Console.WriteLine(linea);
        Console.Out.Flush();
    }
}