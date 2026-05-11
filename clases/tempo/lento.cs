var linea = "";
while( (linea = Console.ReadLine()) is not null) {
    Console.WriteLine(linea);
    Console.Out.Flush();
    Thread.Sleep(3000);
}