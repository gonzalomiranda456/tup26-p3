using System.Diagnostics;

var cmd = new ProcessStartInfo("ls") {
	RedirectStandardOutput = true,
};

using var proceso = Process.Start(cmd);
var salida = proceso.StandardOutput.ReadToEnd();

foreach (var linea in salida.Split("\n")) {
    if(linea.StartsWith("15"))
        Console.WriteLine($"- {linea}");
}
