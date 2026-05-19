#!/bin/sh

base_url="http://localhost:5001/contador"

printf "Estado inicial: "
curl -s "$base_url"
echo

printf "Incrementar 1: "
curl -s -X PUT "$base_url"
echo

printf "Incrementar 2: "
curl -s -X PUT "$base_url"
echo

printf "Despues de incrementar 2 veces: "
curl -s "$base_url"
echo

printf "Borrar contador: "
curl -s -X DELETE "$base_url"
echo

printf "Estado final: "
curl -s "$base_url"
echo
