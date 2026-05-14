import { useMemo, useState } from 'react'
import './App.css'

const contactos = [
  {
    id: 1,
    nombre: 'Lucia',
    apellido: 'Fernandez',
    telefono: '+54 381 421-7845',
    email: 'lucia.fernandez@example.com',
    empresa: 'Estudio Norte',
    cargo: 'Disenadora UX',
    direccion: 'San Miguel de Tucuman, Tucuman',
    notas: 'Prefiere contacto por WhatsApp durante la manana.',
  },
  {
    id: 2,
    nombre: 'Mateo',
    apellido: 'Gimenez',
    telefono: '+54 381 556-2301',
    email: 'mateo.gimenez@example.com',
    empresa: 'Andes Software',
    cargo: 'Desarrollador Backend',
    direccion: 'Yerba Buena, Tucuman',
    notas: 'Disponible para reuniones tecnicas los martes y jueves.',
  },
  {
    id: 3,
    nombre: 'Valentina',
    apellido: 'Rojas',
    telefono: '+54 381 612-9044',
    email: 'valentina.rojas@example.com',
    empresa: 'Clinica Central',
    cargo: 'Administracion',
    direccion: 'Banda del Rio Sali, Tucuman',
    notas: 'Enviar recordatorios por correo electronico.',
  },
  {
    id: 4,
    nombre: 'Santiago',
    apellido: 'Diaz',
    telefono: '+54 381 488-1170',
    email: 'santiago.diaz@example.com',
    empresa: 'Logistica NOA',
    cargo: 'Coordinador',
    direccion: 'Tafi Viejo, Tucuman',
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
              <span>Telefono</span>
              <strong>{contactoSeleccionado.telefono}</strong>
            </article>
            <article className="detail-item">
              <span>Email</span>
              <strong>{contactoSeleccionado.email}</strong>
            </article>
            <article className="detail-item">
              <span>Direccion</span>
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
