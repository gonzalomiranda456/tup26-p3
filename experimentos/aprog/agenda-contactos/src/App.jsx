import { useMemo, useState } from 'react'
import './App.css'

const contactos = [
  {
    id: 1,
    nombre: 'Lucía',
    apellido: 'Fernández',
    telefono: '+54 381 421-7845',
    email: 'lucia.fernandez@example.com',
    empresa: 'Estudio Norte',
    cargo: 'Diseñadora UX',
    direccion: 'San Miguel de Tucumán, Tucumán',
    notas: 'Prefiere contacto por WhatsApp durante la mañana.',
  },
  {
    id: 2,
    nombre: 'Mateo',
    apellido: 'Giménez',
    telefono: '+54 381 556-2301',
    email: 'mateo.gimenez@example.com',
    empresa: 'Andes Software',
    cargo: 'Desarrollador Backend',
    direccion: 'Yerba Buena, Tucumán',
    notas: 'Disponible para reuniones técnicas los martes y jueves.',
  },
  {
    id: 3,
    nombre: 'Valentina',
    apellido: 'Rojas',
    telefono: '+54 381 612-9044',
    email: 'valentina.rojas@example.com',
    empresa: 'Clínica Central',
    cargo: 'Administración',
    direccion: 'Banda del Río Salí, Tucumán',
    notas: 'Enviar recordatorios por correo electrónico.',
  },
  {
    id: 4,
    nombre: 'Santiago',
    apellido: 'Díaz',
    telefono: '+54 381 488-1170',
    email: 'santiago.diaz@example.com',
    empresa: 'Logística NOA',
    cargo: 'Coordinador',
    direccion: 'Tafí Viejo, Tucumán',
    notas: 'Contacto principal para entregas de zona norte.',
  },
]

function nombreCompleto(contacto) {
  return `${contacto.nombre} ${contacto.apellido}`
}

function iniciales(contacto) {
  return `${contacto.nombre[0]}${contacto.apellido[0]}`
}

function App() {
  const [contactoSeleccionadoId, setContactoSeleccionadoId] = useState(contactos[0].id)

  const contactoSeleccionado = useMemo(
    () => contactos.find((contacto) => contacto.id === contactoSeleccionadoId) ?? contactos[0],
    [contactoSeleccionadoId],
  )

  return (
    <main className="app-shell">
      <section className="agenda" aria-label="Agenda de contactos">
        <aside className="master">
          <div className="section-heading">
            <p>Agenda</p>
            <h1>Contactos</h1>
          </div>

          <nav className="contact-list" aria-label="Lista de contactos">
            {contactos.map((contacto) => {
              const seleccionado = contacto.id === contactoSeleccionado.id

              return (
                <button
                  className={`contact-row ${seleccionado ? 'is-selected' : ''}`}
                  key={contacto.id}
                  onClick={() => setContactoSeleccionadoId(contacto.id)}
                  type="button"
                  aria-current={seleccionado ? 'true' : undefined}
                >
                  <span className="contact-avatar" aria-hidden="true">
                    {iniciales(contacto)}
                  </span>
                  <span className="contact-summary">
                    <strong>{nombreCompleto(contacto)}</strong>
                    <small>{contacto.empresa}</small>
                  </span>
                </button>
              )
            })}
          </nav>
        </aside>

        <section className="detail" aria-labelledby="contact-detail-title">
          <header className="detail-header">
            <div className="detail-avatar" aria-hidden="true">
              {iniciales(contactoSeleccionado)}
            </div>
            <div>
              <p>{contactoSeleccionado.cargo}</p>
              <h2 id="contact-detail-title">{nombreCompleto(contactoSeleccionado)}</h2>
              <span>{contactoSeleccionado.empresa}</span>
            </div>
          </header>

          <div className="detail-grid">
            <article className="detail-item">
              <span>Teléfono</span>
              <strong>{contactoSeleccionado.telefono}</strong>
            </article>
            <article className="detail-item">
              <span>Email</span>
              <strong>{contactoSeleccionado.email}</strong>
            </article>
            <article className="detail-item">
              <span>Dirección</span>
              <strong>{contactoSeleccionado.direccion}</strong>
            </article>
            <article className="detail-item">
              <span>Notas</span>
              <strong>{contactoSeleccionado.notas}</strong>
            </article>
          </div>
        </section>
      </section>
    </main>
  )
}

export default App
