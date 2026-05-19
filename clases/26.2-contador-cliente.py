import json
import urllib.request

base_url = "http://localhost:5001/contador"


def leer_contador():
    with urllib.request.urlopen(base_url) as response:
        return json.load(response)["contador"]


def enviar_put():
    request = urllib.request.Request(base_url, method="PUT")
    with urllib.request.urlopen(request):
        pass


def enviar_delete():
    request = urllib.request.Request(base_url, method="DELETE")
    with urllib.request.urlopen(request):
        pass


inicial = leer_contador()
print(f"Estado inicial: {inicial}")

enviar_put()
enviar_put()

despues_de_incrementar = leer_contador()
print(f"Después de incrementar 2 veces: {despues_de_incrementar}")

enviar_delete()

final = leer_contador()
print(f"Estado final: {final}")
