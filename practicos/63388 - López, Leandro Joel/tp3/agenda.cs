#!/usr/bin/env dotnet
#:property PublishAot=false

#:package Terminal.Gui@2.0.1
#:package Microsoft.Data.Sqlite@*
#:package Dapper@*
#:package Dapper.Contrib@*

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Drivers;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Data.Common;
using Dapper.Contrib.Extensions;
using System.Security.Cryptography.X509Certificates;

/// ==== 
/// Estes es un archivo de referencia con el esqueleto del proyecto.
/// No es un código de ejemplo, sino el punto de partida para el desarrollo del trabajo práctico. 
/// ====

// Punto de entrada

try {
    string databasePath = args.Length switch {
        
        0=> "agenda.db",
        1=> args[0],
        _=> throw new ArgumentException("uso: agenda [archivo.db]")
    };

    using IApplication app = Application.Create().Init();
    var store = new SqliteAgendaStore(databasePath);
    var agenda = new AgendaWindow(store);
    app.Run(agenda);
    return 0;

}
 catch (Exception ex) {
    Console.WriteLine($"Error: {ex.Message}");
    return 1;
}

// Ventana principal
public sealed class AgendaWindow : Runnable {

    private readonly SqliteAgendaStore _store;
    private readonly List <Contacto> contactos = new();
    private readonly List<Contacto> contactosFiltrados = new();
    private readonly ObservableCollection<string> filas = new();
    private MenuItem soloFavoritosMenuItem = null!;
    private bool soloFavoritos;
    private string filtro = string.Empty;
    private ListView listView = null!;
    private TextField searchField = null!;
    private TextView detailsView = null!;
    private Label statusLabel = null!;
    private string ultimaOperacion = "Listo.";
    public AgendaWindow(SqliteAgendaStore store) : base(){

        this._store = store;
        Title  = "Agenda - AgendaT";
        Width  = Dim.Fill();
        Height = Dim.Fill();

        Menu.DefaultBorderStyle = LineStyle.Single;
        BuildLayout();
        CargarContactos();
    }

    private void BuildLayout() {

        soloFavoritosMenuItem = new MenuItem("_Solo favoritos", string.Empty, ToggleSoloFavoritos);
        
        
        var menu = new MenuBar(new MenuBarItem[]  {
            
                new ("_Archivo", new MenuItem [] {
                    new ("_Importar Json", "Ctrl+I", ImportarJson),
                    new ("_Exportar Json", "Ctrl+E", ExportarJson),                    
                    new MenuItem("_Salir", "Ctrl+Q", SolicitarSalir)
                }),

                new ("_Contacto", new MenuItem [] {
                    new ("_Nuevo contacto", "Ctrl+N", NuevoContacto),
                    new ("_Editar contacto", "Ctrl+E", EditarContacto),
                    new ("_Eliminar contacto", "Ctrl+D", EliminarContacto),
                    new ("_Alternar favorito", string.Empty, AlternarFavorito)
                }),

                new ("_Ver", new MenuItem [] {
                    soloFavoritosMenuItem,
                }),
                new ("_Ayuda", new MenuItem [] {
                    new ("_Acerca de", string.Empty, MostrarAcercaDe)
                })
        });

        var searchLabel = new Label() {
            Text = "Buscar:",
            X    = 1,
            Y    = 1
        };

        searchField = new TextField() {
            Text  = string.Empty,
            X     = Pos.Right(searchLabel) + 1,
            Y     = Pos.Top(searchLabel),
            Width = Dim.Fill(1)
        };

        searchField.TextChanged += (_, _) => {
            filtro = searchField.Text ?? string.Empty;
            ActualizarListaVisible();
        };

        listView = new ListView() {
            X      = 1,
            Y      = 3,
            Width  = Dim.Percent(55),
            Height = Dim.Fill(4)
        };

        listView.Source = new ListWrapper<string>(filas);
        listView.ValueChanged += (_, _) => MostrarDetalle();
        listView.Accepted += (_, _) => EditarSelecciona();

        detailsView = new TextView() {
            X      = Pos.Right(listView) + 2,
            Y      = Pos.Top(listView),
            Width  = Dim.Fill(1),
            Height = Dim.Fill(4)
            ReadOnly = true,
            WordWrap = true,
            Text = "Sin contactos."
        };

        statusLabel = new Label() {
            Text = ultimaOperacion,
            X    = 1,
            Y    = Pos.AnchorEnd(1),
            Width = Dim.Fill(1)
        };

        Add(menu, searchLabel, searchField, listView, detailsView, statusLabel);
    
    private void CargarContactos() {
        contactos.Clear();
        contactos.AddRange(store.Listartodos());
        ActualizarListaVisible();
    }

    private void ActualizarListaVisible() {

        contactosFiltrados.Clear();
        filas.Clear();

        string texto = filtro.Trim();
        foreach (Contacto contacto in contactos) {

            if (soloFavoritos && !contacto.Favorito) {

                continue;
            }

            if (!string.IsNullOrWhiteSpace(texto) &&
               !contacto.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase) &&
               !contacto.Telefonos.Contains(texto, StringComparison.OrdinalIgnoreCase) &&
               !contacto.Email.Contains(texto, StringComparison.OrdinalIgnoreCase) && {
                continue;
            }
        
            contactosFiltrados.Add(contacto);
            filas.Add($"{(contacto.Favorito ? "*" : " ")}{contacto.Nombre} - {contacto.Telefonos}");
    }
    listView.Source = new ListWrapper<string>(filas);
    MostrarDetalle();
    }

    private Contacto? Seleccionado() {

        int selected = listView.SelectedItem ?? -1;
        if (selected < 0 || selected >= contactosFiltrados.Count) {

            return null;
        }
        return contactosFiltrados[selected];
    }

    private void MostrarDetalle() {

        Contacto? contacto = Seleccionado();
    detailsView.Text = contacto is null ? " Sin contactos para mostrar." : $"Nombre: {contacto.Nombre}\nTelefonos: {contacto.Telefonos}\nEmail: {contacto.Email}\nFavorito: {(contacto.Favorito ? "Sí" : "No")}\n\nNotas:\n{contacto.Notas}";
    }

    private void NuevoContacto() {
        var dialog = new ContactoDialog("Nuevo contacto", new Contacto());
        App!.Run(dialog);
        if (dialog.Guardado) {
            
            store.Guardar(dialog.Contacto);
            CargarContactos();
            ActualizarEstado("Contacto creado.");
        }
    }

    private void EditarSelecciona() {

        Contacto? actual = Seleccionado();
        if (actual is null) {

            MessageBox.ErrorQuery(App!, "Agenda", "Selecciona un contacto para editar.", "OK");
            return;
        }

        var dialog = new ContactoDialog("Editar contacto", actual.Clonar());
        App!.Run(dialog);
        if (dialog.Guardado) {

            store.Guardar(dialog.Contacto);
            CargarContactos();
            ActualizarEstado($"Contacto '{dialog.Contacto.Nombre}' actualizado.");
        }
    }

    private void EliminarSeleccionado() {

        Contacto? actual = Seleccionado();
        if (actual is null) {

            MessageBox.ErrorQuery(App!, "Agenda", "Selecciona un contacto para eliminar.", "OK");
            return;
        }

        int response = MessageBox.Query(App!, "Eliminar", $"Eliminar a {actual.Nombre}?", "Sí", "No") ??-1;
        if (response == 1) {

            Store.Eliminar(actual.Id);
            CargarContactos();
            ActualizarEstado($"Contacto '{actual.Nombre}' eliminado.");
        }
    }

    private void AlternarFavorito() {

        Contacto? actual = Seleccionado();
        if (actual is null) {

            MessageBox.ErrorQuery(App!, "Agenda", "Selecciona un contacto para alternar favorito.", "OK");
            return;
        }

        actual.Favorito = !actual.Favorito;
        store.Guardar(actual);
        CargarContactos();
        ActualizarEstado(actual.Favorito ? "marcado como favorito" : "desmarcado como favorito");
    }

    private void ToggleSoloFavoritos() {

        soloFavoritos = !soloFavoritos;
        soloFavoritosMenuItem.Title = soloFavoritos ? "_Solo favoritos [x]" : "_Solo favoritos";
        ActualizarListaVisible();
        ActualizarEstado(soloFavoritos ? "Filtrando solo favoritos" : "Mostrando todos los contactos");
    }

    private void ImportarJson() {

        var dialog = new OpenDialog() {
            
            Title = "Importar contactos",

        };

        App!.Run(dialog);

        if (dialog.Canceled) {

            return;

        }

        var pathEntry = dialog.FilePaths.FirstOrDefault();
        if (pathEntry is null) {

            return;
        }

        string path = pathEntry.ToString();
        try {
            
            var importados = JsonAgendaIO.Leer(path);
            if (importados.count == 0) {

                MessageBox.Query(App!, "Importar", "No se encontraron contactos en el archivo.", "OK");
                return;
            }

            int response = MessageBox.Query(App!, "Importar", $"Se encontraron {importados.Count} contactos. ¿Deseas importarlos?", "Sí", "No") ?? -1;
            if (response != 1) {

                return;
            }

            foreach (var contacto in importados) {

                contacto.Id = 0;
                store.Guardar(contacto);
            }

            CargarContactos();
            ActualizarEstado($"Importados {importados.Count} contactos.");
        }

        catch (Exception ex) {

            MessageBox.ErrorQuery(App!, "Error", ex.Message, "OK");
        }
    }

    private void ExportarJson() {

        var dialog = new SaveDialog() {
            
            Title = "Exportar contactos",

        };

        App!.Run(dialog);

        if (dialog.Canceled || string.IsNullOrWhiteSpace(dialog.FileName?.ToString()) ) {

            return;

        }

        try {
            
            string path = dialog.FileName.ToString();

            JsonAgendaIO.Escribir(path, StoreLocation.ListrarTodos());
            MessageBox.Query(App!, "Exportar", $"Contactos exportados correctamente", "OK");
            ActualizarEstado($"Exportados {store.ListarTodos().Count} contactos a {Path.GetFileName(path)}.");
        }
        
        catch (Exception ex) {

            MessageBox.ErrorQuery(App!, "Error", ex.Message, "OK");
        }
    }

    private void MostrarAcercaDe() {

        MessageBox.Query(App!, "Acerca de", "AgendaT - Trabajo Practico 3\nTerminal GUI con Persistencia SQLite e import/export JSON.\n\nAtajos: F2, F3, Del, Ctrl+N, CtrlD, Ctrl+I, Ctrl+E, F4,Ctrl+Q", "OK");
    }

    private void ActualizarEstado(string mensaje) {

        ultimaOperacion = mensaje;
        statusLabel.Text = ultimaOperacion;
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


public class SqliteAgendaStore {}
public class JsonAgendaIO {}

[Table("Contactos")]
public class Contacto {
    [Key] public int    Id        { get; set; }
          public string Nombre    { get; set; } = "";
          public string Telefonos { get; set; } = "";
          public string Email     { get; set; } = "";
          public string Notas     { get; set; } = "";
          public bool   Favorito  { get; set; }
}