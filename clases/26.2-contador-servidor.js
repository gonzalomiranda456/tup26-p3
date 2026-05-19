const express = require("express");

const app = express();
let contador = 0;

app.get("/contador", (req, res) => {
  res.json({ contador });
});

app.put("/contador", (req, res) => {
  contador++;
  res.json({ contador });
});

app.delete("/contador", (req, res) => {
  contador = 0;
  res.json({ contador });
});

app.listen(5001);
