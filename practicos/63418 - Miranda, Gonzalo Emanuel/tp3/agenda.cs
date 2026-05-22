#!/usr/bin/env dotnet
#:property PublishAot=false

#:package Terminal.Gui@2.0.1
#:package Microsoft.Data.Sqlite@*
#:package Dapper@*
#:package Dapper.Contrib@*


using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Data.Common;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.Json;
using System.IO;

/// ==== 
/// Estes es un archivo de referencia con el esqueleto del proyecto.
/// No es un código de ejemplo, sino el punto de partida para el desarrollo del trabajo práctico. 
/// ====

// Punto de entrada
string dbPath = args.Length > 0 ? args[0] : "agenda.db";
var store = new SqliteAgendaStore(dbPath);

using IApplication app = Application.Create().Init();
app.Run(new AgendaWindow(store));


// Ventana principal
public sealed class AgendaWindow : Runnable 
{
    private readonly SqliteAgendaStore _store;

    public AgendaWindow(SqliteAgendaStore store) 
    {
        _store = store;
        Title  = "Agenda - Terminal.Gui";
        Width  = Dim.Fill();
        Height = Dim.Fill();

        Menu.DefaultBorderStyle = LineStyle.Single;
        BuildLayout();
    }

    private void BuildLayout() {
        MenuBar menu = new() {
            Menus = [
                new MenuBarItem("_Archivo", [
                    new MenuItem("_Nuevo contacto", null!, AbrirDialogo),
                    null!, // Separador
                    new MenuItem("_Salir", "Ctrl+Q", SolicitarSalir)
                ])
            ]
        };

        Button openButton = new() {
            Text = "_Abrir diálogo",
            X    = Pos.Center(),
            Y    = Pos.Center()
        };

        openButton.Accepting += (_, e) => {
            AbrirDialogo();
            e.Handled = true;
        };

        Add(menu, openButton);
    }

    private void AbrirDialogo() {
        EjemploDialog dialog = new();
        App!.Run(dialog);
    }

    private void SolicitarSalir() {
        App!.RequestStop();
    }

    protected override bool OnKeyDown(Key key) {
        if (key == Key.Q.WithCtrl) {
            SolicitarSalir();
            return true;
        }

        return base.OnKeyDown(key);
    }
}

// Diálogo de ejemplo
public sealed class EjemploDialog : Dialog {
    public EjemploDialog() {
        Title  = "Diálogo de ejemplo";
        Width  = 50;
        Height = 8;

        Label message = new() {
            Text = "Este es un diálogo modal de ejemplo.",
            X    = Pos.Center(),
            Y    = 1
        };

        Button closeButton = new() {
            Text      = "_Cerrar",
            IsDefault = true
        };

        closeButton.Accepting += (_, e) => {
            App!.RequestStop();
            e.Handled = true;
        };

        Add(message);
        AddButton(closeButton);
    }
}


public class SqliteAgendaStore {
        private readonly string _connectionString;

        public SqliteAgendaStore(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }
    

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Contactos (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre TEXT NOT NULL,
                Telefonos TEXT,
                Email TEXT,
                Notas TEXT,
                Favorito INTEGER NOT NULL
            );";
        connection.Execute(createTableQuery);
        }

        public IEnumerable<Contacto> GetAll()
        {
            using var connection = new SqliteConnection(_connectionString);
            return connection.GetAll<Contacto>();
        }

        public void Insert(Contacto contacto)
        {
            using var connection = new SqliteConnection(_connectionString);
            contacto.Id = (int)connection.Insert(contacto);
        }

        public void Update(Contacto contacto)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Update(contacto);
        }

        public void Delete(Contacto contacto)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Delete(contacto);
        }
    }
public class JsonAgendaIO 
{
    public static void Exportar(IEnumerable<Contacto> contactos, string rutaArchivo)
    {
        // Configuramos el JSON para que sea legible (con sangrías) y maneje bien los caracteres como la ñ o tildes
        var opciones = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        
        string json = JsonSerializer.Serialize(contactos, opciones);
        File.WriteAllText(rutaArchivo, json);
    }

    public static List<Contacto> Importar(string rutaArchivo)
    {
        if (!File.Exists(rutaArchivo))
        {
            throw new FileNotFoundException("El archivo JSON no existe.");
        }

        string json = File.ReadAllText(rutaArchivo);
        var contactos = JsonSerializer.Deserialize<List<Contacto>>(json);
        
        return contactos ?? new List<Contacto>();
    }
}

[Table("Contactos")]
public sealed class Contacto {
    [Key] public int    Id        { get; set; }
          public string Nombre    { get; set; } = "";
          public string Telefonos { get; set; } = "";
          public string Email     { get; set; } = "";
          public string Notas     { get; set; } = "";
          public bool   Favorito  { get; set; }

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