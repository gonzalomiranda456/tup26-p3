from fastapi import FastAPI
import uvicorn

app = FastAPI()
contador = 0


@app.get("/contador")
def leer_contador():
    return {"contador": contador}


@app.put("/contador")
def incrementar_contador():
    global contador
    contador += 1
    return {"contador": contador}


@app.delete("/contador")
def borrar_contador():
    global contador
    contador = 0
    return {"contador": contador}


uvicorn.run(app, host="localhost", port=5001)
