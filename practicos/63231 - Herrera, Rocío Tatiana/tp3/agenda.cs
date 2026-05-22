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

string archivoDb = args.Length > 0 ? args[0] : ":memory:";
using SqliteAgendaStore store = new(archivoDb);
store.CrearTablas();
using IApplication app = Application.Create().Init();
app.Run(new AgendaWindow(store, archivoDb));

[Table("Contactos")]
public sealed class Contacto {
    [Key] public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Telefonos { get; set; } = "";
    public string Email { get; set; } = "";
    public string Notas { get; set; } = "";
    public bool Favorito { get; set; }
    public Contacto Clone() => new() { Id = Id, Nombre = Nombre, Telefonos = Telefonos, Email = Email, Notas = Notas, Favorito = Favorito };
}

public sealed class SqliteAgendaStore : IDisposable {
    readonly SqliteConnection cn;
    public SqliteAgendaStore(string archivo) { cn = new(new SqliteConnectionStringBuilder { DataSource = archivo }.ConnectionString); cn.Open(); }
    public void CrearTablas() => cn.Execute("""
        CREATE TABLE IF NOT EXISTS Contactos(
            Id INTEGER PRIMARY KEY AUTOINCREMENT, Nombre TEXT NOT NULL,
            Telefonos TEXT NOT NULL DEFAULT '', Email TEXT NOT NULL DEFAULT '',
            Notas TEXT NOT NULL DEFAULT '', Favorito INTEGER NOT NULL DEFAULT 0);
        """);
    public IEnumerable<Contacto> Listar() => cn.GetAll<Contacto>();
    public Contacto Agregar(Contacto c) { Validar(c); c.Id = 0; c.Id = Convert.ToInt32(cn.Insert(c)); return c; }
    public void Modificar(Contacto c) { Validar(c); cn.Update(c); }
    public void Eliminar(Contacto c) => cn.Delete(c);
    public void Dispose() => cn.Dispose();
    static void Validar(Contacto c) {
        if (string.IsNullOrWhiteSpace(c.Nombre)) throw new InvalidOperationException("El nombre no puede estar vacío.");
        if (!string.IsNullOrWhiteSpace(c.Email) && !c.Email.Contains('@')) throw new InvalidOperationException("El email debe contener @.");
    }
}
public sealed class AgendaWindow : Runnable {
    readonly SqliteAgendaStore store; readonly string db;
    readonly List<Contacto> contactos; readonly List<Contacto> visibles = [];
    readonly System.Collections.ObjectModel.ObservableCollection<string> filas = [];
    readonly TextField buscar; readonly ListView lista; readonly TextView detalle; readonly Label estado;
    bool soloFav;

    public AgendaWindow(SqliteAgendaStore store, string db) {
        this.store = store; this.db = db; contactos = store.Listar().ToList();
        Title = "Agenda - Terminal.Gui"; Width = Dim.Fill(); Height = Dim.Fill();
        Menu.DefaultBorderStyle = LineStyle.Single;

        Label lbl = new() { Text = "Buscar:", X = 1, Y = 1 };
        buscar = new() { X = Pos.Right(lbl) + 1, Y = 1, Width = Dim.Fill(1) };
        buscar.TextChanged += (_, _) => ActualizarVista();

        lista = new() { X = 0, Y = 3, Width = Dim.Percent(40), Height = Dim.Fill(1), Title = "Contactos", BorderStyle = LineStyle.Single };
        lista.SetSource(filas);
        lista.ValueChanged += (_, _) => MostrarDetalle();
        lista.Accepting += (_, e) => { Editar(); e.Handled = true; };

        detalle = new() { X = Pos.Right(lista) + 1, Y = 3, Width = Dim.Fill(), Height = Dim.Fill(1), Title = "Detalle", BorderStyle = LineStyle.Single, CanFocus = false };
        estado = new() { X = 1, Y = Pos.AnchorEnd(1), Width = Dim.Fill(), Text = "Listo." };
        Add(ArmarMenu(), lbl, buscar, lista, detalle, estado);
        ActualizarVista();
    }

    MenuBar ArmarMenu() => new() { Menus = [
        new MenuBarItem("_Archivo", [
            new MenuItem("_Importar JSON", "Ctrl+I", Importar),
            new MenuItem("_Exportar JSON", "Ctrl+E", Exportar),
            null!,
            new MenuItem("_Salir", "Ctrl+Q", () => App!.RequestStop())]),
        new MenuBarItem("_Contactos", [
            new MenuItem("_Nuevo", "F2 / Ctrl+N", Nuevo),
            new MenuItem("_Editar", "F3 / Enter", Editar),
            new MenuItem("_Eliminar", "Del / Ctrl+D", Eliminar)]),
        new MenuBarItem("_Ver", [
            new MenuItem("_Solo favoritos", null!, () => { soloFav = !soloFav; ActualizarVista(); Avisar(soloFav ? "Solo favoritos." : "Todos los contactos."); })]),
        new MenuBarItem("A_yuda", [
            new MenuItem("_Acerca de", null!, () => MessageBox.Query(App!, "Acerca de", $"AgendaT\nBase: {(db == ":memory:" ? "memoria" : db)}", "Aceptar"))])
    ] };

    protected override bool OnKeyDown(Key key) {
        if (key == Key.F2 || key == Key.N.WithCtrl) { Nuevo(); return true; }
        if (key == Key.F3 || key == Key.Enter) { Editar(); return true; }
        if (key == Key.Delete || key == Key.D.WithCtrl) { Eliminar(); return true; }
        if (key == Key.I.WithCtrl) { Importar(); return true; }
        if (key == Key.E.WithCtrl) { Exportar(); return true; }
        if (key == Key.F4) { buscar.SetFocus(); Avisar("Búsqueda activa."); return true; }
        if (key == Key.Q.WithCtrl) { App!.RequestStop(); return true; }
        return base.OnKeyDown(key);
    }
}