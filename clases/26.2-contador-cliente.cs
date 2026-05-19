using HttpClient client = new() {
    BaseAddress = new Uri("http://localhost:5001")
};

Console.WriteLine($"Estado inicial: {await LeerContador()}");

Console.WriteLine($"Incrementar 1: {await IncrementarContador()}");
Console.WriteLine($"Incrementar 2: {await IncrementarContador()}");

Console.WriteLine($"Después de incrementar 2 veces: {await LeerContador()}");

Console.WriteLine($"Borrar contador: {await BorrarContador()}");

Console.WriteLine($"Estado final: {await LeerContador()}");

async Task<string> LeerContador() =>
    await client.GetStringAsync("/contador");

async Task<string> IncrementarContador() {
    HttpResponseMessage respuesta = await client.PutAsync("/contador", null);
    return await respuesta.Content.ReadAsStringAsync();
}

async Task<string> BorrarContador() {
    HttpResponseMessage respuesta = await client.DeleteAsync("/contador");
    return await respuesta.Content.ReadAsStringAsync();
}
