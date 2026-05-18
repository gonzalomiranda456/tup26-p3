#:package Terminal.Gui@2.0.0-*
#:package Microsoft.Data.Sqlite@8.0.1
#:package Dapper@2.1.35
#:package Dapper.Contrib@2.0.78

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Terminal.Gui;
using Microsoft.Data.Sqlite;
using Dapper;
using Dapper.Contrib.Extensions;

string dbPath = "agenda.db";

if (args.Length > 0 && !args[0].StartsWith("-"))
{
    dbPath = args[0];
}

Console.WriteLine($"Base de datos configurada en: {dbPath}");
Console.WriteLine("El esqueleto compila correctamente.");

[Table("Contactos")]
public sealed class Contacto
{
    [Key] 
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Telefonos { get; set; } = "";
    public string Email { get; set; } = "";
    public string Notas { get; set; } = "";
    public bool Favorito { get; set; }

    public Contacto Clone()
    {
        return new Contacto
        {
            Id = this.Id,
            Nombre = this.Nombre,
            Telefonos = this.Telefonos,
            Email = this.Email,
            Notas = this.Notas,
            Favorito = this.Favorito
        };
    }
}