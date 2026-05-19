using HttpClient client = new();
// Crea un objeto HttpClient para poder enviar solicitudes HTTP desde este programa hacia una API web.

string respuesta = await client.GetStringAsync("http://localhost:5000/");
// Envía una petición GET a la URL indicada y espera la respuesta completa como texto.

Console.WriteLine(respuesta);   `
// Muestra en la consola el texto recibido desde el servidor.
