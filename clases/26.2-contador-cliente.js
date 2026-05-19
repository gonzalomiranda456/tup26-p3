const baseUrl = "http://localhost:5001/contador";

async function leerContador() {
  const response = await fetch(baseUrl);
  return await response.text();
}

async function enviarPut() {
  const response = await fetch(baseUrl, { method: "PUT" });
  return await response.text();
}

async function enviarDelete() {
  const response = await fetch(baseUrl, { method: "DELETE" });
  return await response.text();
}

async function main() {
  const inicial = await leerContador();
  console.log(`Estado inicial: ${inicial}`);

  console.log(`Incrementar 1: ${await enviarPut()}`);
  console.log(`Incrementar 2: ${await enviarPut()}`);

  const despuesDeIncrementar = await leerContador();
  console.log(`Después de incrementar 2 veces: ${despuesDeIncrementar}`);

  console.log(`Borrar contador: ${await enviarDelete()}`);

  const final = await leerContador();
  console.log(`Estado final: ${final}`);
}

main();
