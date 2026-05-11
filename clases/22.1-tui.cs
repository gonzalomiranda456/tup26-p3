#:package Terminal.Gui@2.0.1
#:property PublishAot=false

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using Terminal.Gui.App;       // Application, IApplication, Runnable
using Terminal.Gui.Input;     // Key, Command
using Terminal.Gui.ViewBase;  // View, Pos, Dim
using Terminal.Gui.Views;     // Window, Label


using IApplication app = Application.Create().Init()   ;

using var raiz = new Runnable() {
    Width = Dim.Fill(),
    Height = Dim.Fill(),
};


Contacto? seleccionado = null;
// Hacer un menu Archivo>Leer, Guardar,-, Salir y Editar>Copiar, Pegar, Cortar
var menu = new MenuBar(new MenuBarItem[] {
    new("Archivo", new MenuItem[] {
        new("Leer", "", () => Console.WriteLine("Leer")),
        new("Guardar", "", () => Console.WriteLine("Guardar")),
        new("-", "", null),
        new("Salir", "", () => Environment.Exit(0)),
    }),
    new("Editar", new MenuItem[] {
        new("Copiar", "", () => Console.WriteLine("Copiar")),
        new("Pegar", "", () => Console.WriteLine("Pegar")),
        new("Cortar", "", () => Console.WriteLine("Cortar")),
    }),
});

raiz.Add(menu);

var ventanaAgenda = new Window() {
    Title = "Agenda",
    X = 0,
    Y = 1,
    Width = Dim.Fill(),
    Height = Dim.Fill(),
};

var agenda = new ObservableCollection<Contacto> {
    new("Adrián", "Andarade", "123456789"),
    new("Beatriz", "Benitez", "987654321"),
    new("Carlos", "Cataneo", "555555555"),
    new("Diana", "Díaz", "111111111"),
    new("Elena", "Escobar", "222222222"),
    new("Federico", "Fernández", "333333333"),
    new("Gabriela", "García", "444444444"),
};

var lista = new ListView<Contacto> {
    X = 0,
    Y = 0,
    Width = Dim.Fill(),
    Height = Dim.Fill(),
};

lista.SetSource(agenda);
lista.Index = 0;

lista.Activated += (_, _) => {
    seleccionado = lista.Value;
    app.RequestStop();
};

ventanaAgenda.Add(lista);
raiz.Add(ventanaAgenda);

lista.SetFocus();
app.Run(raiz);

Console.WriteLine(seleccionado);

record Contacto(string Nombre, string Apellido, string Telefono) {
    public override string ToString() => $"{$"{Nombre} {Apellido}".PadRight(40)} ({Telefono})";
};
