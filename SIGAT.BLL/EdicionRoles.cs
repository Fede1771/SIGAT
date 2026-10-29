using SIGAT.BE;

namespace SIGAT.BLL;

// Unidad de edición en memoria. El grafo se copia por ID para conservar las
// familias compartidas sin modificar los objetos de la sesión o del repositorio.
public sealed class EdicionRoles
{
    private EstadoRoles confirmado;
    private readonly List<string> cambios = new();
    public EstadoRoles Estado { get; private set; }
    public IReadOnlyList<string> Cambios => cambios.AsReadOnly();
    public bool TieneCambios => cambios.Count != 0;

    public EdicionRoles(EstadoRoles origen)
    {
        confirmado = Copiar(origen);
        Estado = Copiar(confirmado);
    }

    public static EstadoRoles Copiar(EstadoRoles origen)
    {
        var copia = new EstadoRoles { Version = origen.Version, IdAdministradorOriginal = origen.IdAdministradorOriginal };
        var permisos = origen.Permisos.ToDictionary(p => p.Id, p => p is PermisoCompuesto
            ? (Permiso)new PermisoCompuesto { Id = p.Id, Nombre = p.Nombre }
            : new PermisoSimple { Id = p.Id, Nombre = p.Nombre });
        var roles = origen.Roles.ToDictionary(r => r.Id, r => new Rol { Id = r.Id, Nombre = r.Nombre });
        copia.Permisos.AddRange(origen.Permisos.Select(p => permisos[p.Id]));
        copia.Roles.AddRange(origen.Roles.Select(r => roles[r.Id]));
        foreach (var p in origen.Permisos)
            foreach (var hijo in p.ObtenerHijos()) permisos[p.Id].Agregar(permisos[hijo.Id]);
        foreach (var r in origen.Roles)
        {
            foreach (var p in r.Permisos) roles[r.Id].AgregarPermiso(permisos[p.Id]);
            foreach (var hijo in r.Roles) roles[r.Id].AgregarRol(roles[hijo.Id]);
        }
        foreach (var u in origen.Usuarios)
        {
            var nuevo = new Usuario { IdUsuario = u.IdUsuario, NombreUsuario = u.NombreUsuario, Nombre = u.Nombre,
                Apellido = u.Apellido, Password = u.Password, Activo = u.Activo,
                DigitoVerificador = u.DigitoVerificador, EsAdministradorOriginal = u.EsAdministradorOriginal };
            foreach (var r in u.Roles) nuevo.AsignarRol(roles[r.Id]);
            copia.Usuarios.Add(nuevo);
        }
        return copia;
    }

    private void Aplicar(string descripcion, Action<EstadoRoles> accion)
    {
        var candidato = Copiar(Estado);
        accion(candidato);
        RolesBLL.Validar(candidato);
        if (Firma(candidato) == Firma(Estado)) return;
        Estado = candidato; // Publicar solo cuando toda la operación es válida.
        cambios.Clear();
        cambios.AddRange(Diferencias(confirmado, Estado));
        if (cambios.Count == 0 && Firma(Estado) != Firma(confirmado)) cambios.Add(descripcion);
    }

    // El resumen describe el resultado final, no los clics (agregar y luego
    // quitar una relación no debe aparecer como dos cambios pendientes).
    private static IEnumerable<string> Diferencias(EstadoRoles antes, EstadoRoles despues)
    {
        foreach (var p in despues.Permisos.Where(p => !antes.Permisos.Any(a => a.Id == p.Id)))
            yield return "Crear familia: " + p.Nombre;
        foreach (var p in antes.Permisos.Where(p => !despues.Permisos.Any(a => a.Id == p.Id)))
            yield return "Eliminar familia: " + p.Nombre;
        IEnumerable<string> Relaciones(IEnumerable<int> viejos, IEnumerable<int> nuevos, string destino, Func<int, string> nombre)
        {
            foreach (int id in nuevos.Except(viejos)) yield return "Agregar " + nombre(id) + " a " + destino;
            foreach (int id in viejos.Except(nuevos)) yield return "Quitar " + nombre(id) + " de " + destino;
        }
        string Permiso(int id) => (despues.Permisos.Concat(antes.Permisos).First(p => p.Id == id) is PermisoCompuesto ? "familia " : "permiso ") +
            despues.Permisos.Concat(antes.Permisos).First(p => p.Id == id).Nombre;
        string Rol(int id) => "rol " + despues.Roles.Concat(antes.Roles).First(r => r.Id == id).Nombre;
        foreach (var p in despues.Permisos)
        {
            var previo = antes.Permisos.Find(a => a.Id == p.Id);
            if (previo != null && previo.Nombre != p.Nombre) yield return "Renombrar familia " + previo.Nombre + " a " + p.Nombre;
            foreach (var cambio in Relaciones(previo?.ObtenerHijos().Select(h => h.Id) ?? Array.Empty<int>(), p.ObtenerHijos().Select(h => h.Id), "familia " + p.Nombre, Permiso)) yield return cambio;
        }
        foreach (var r in despues.Roles)
        {
            var previo = antes.Roles.Find(a => a.Id == r.Id);
            foreach (var cambio in Relaciones(previo?.Permisos.Select(p => p.Id) ?? Array.Empty<int>(), r.Permisos.Select(p => p.Id), "rol " + r.Nombre, Permiso)) yield return cambio;
            foreach (var cambio in Relaciones(previo?.Roles.Select(h => h.Id) ?? Array.Empty<int>(), r.Roles.Select(h => h.Id), "rol " + r.Nombre, Rol)) yield return cambio;
        }
        foreach (var u in despues.Usuarios)
            foreach (var cambio in Relaciones(antes.Usuarios.Find(a => a.IdUsuario == u.IdUsuario)?.Roles.Select(r => r.Id) ?? Array.Empty<int>(), u.Roles.Select(r => r.Id), "usuario " + u.NombreUsuario, Rol)) yield return cambio;
    }

    private static string Firma(EstadoRoles e) => System.Text.Json.JsonSerializer.Serialize(new
    {
        permisos = e.Permisos.OrderBy(p => p.Id).Select(p => new { p.Id, p.Nombre, hijos = p.ObtenerHijos().Select(h => h.Id).Order().ToArray() }),
        roles = e.Roles.OrderBy(r => r.Id).Select(r => new { r.Id, r.Nombre, permisos = r.Permisos.Select(p => p.Id).Order().ToArray(), roles = r.Roles.Select(h => h.Id).Order().ToArray() }),
        usuarios = e.Usuarios.OrderBy(u => u.IdUsuario).Select(u => new { u.IdUsuario, roles = u.Roles.Select(r => r.Id).Order().ToArray() })
    });

    public void Descartar() { Estado = Copiar(confirmado); cambios.Clear(); }

    public void Guardar(RolesBLL servicio)
    {
        if (!TieneCambios) return;
        servicio.Guardar(Estado); // Autorización, concurrencia, transacción y DV permanecen en BLL/DAL.
        confirmado = Copiar(Estado);
        cambios.Clear();
    }

    private static Rol RolEditable(EstadoRoles e, int id)
    {
        if (id == 1) throw new InvalidOperationException("El rol Administrador está protegido.");
        return e.Roles.Single(r => r.Id == id);
    }

    public void AsignarRolUsuario(int usuario, int rol, bool asignar)
    {
        var u = Estado.Usuarios.Single(u => u.IdUsuario == usuario);
        var r = Estado.Roles.Single(r => r.Id == rol);
        if (u.Roles.Any(x => x.Id == rol) == asignar) return;
        if (!asignar && rol == 1 && usuario == Estado.IdAdministradorOriginal)
            throw new InvalidOperationException("El administrador original debe conservar el rol Administrador.");
        Aplicar($"{(asignar ? "Asignar" : "Quitar")} {r.Nombre} a {u.NombreUsuario}", e =>
        {
            var cuenta = e.Usuarios.Single(x => x.IdUsuario == usuario);
            var perfil = e.Roles.Single(x => x.Id == rol);
            if (asignar) cuenta.AsignarRol(perfil); else cuenta.QuitarRol(perfil);
        });
    }

    public void AgregarAlRol(int destino, IEnumerable<int> permisos, IEnumerable<int> roles)
    {
        int[] ps = permisos.ToArray(), rs = roles.ToArray();
        if (ps.Length + rs.Length == 0) return;
        string nombre = Estado.Roles.Single(r => r.Id == destino).Nombre;
        string detalle = string.Join(", ", ps.Select(id => Estado.Permisos.Single(p => p.Id == id).Nombre)
            .Concat(rs.Select(id => "Rol: " + Estado.Roles.Single(r => r.Id == id).Nombre)));
        Aplicar($"Agregar a {nombre}: {detalle}", e =>
        {
            var r = RolEditable(e, destino);
            foreach (int id in ps) r.AgregarPermiso(e.Permisos.Single(p => p.Id == id));
            foreach (int id in rs) r.AgregarRol(e.Roles.Single(h => h.Id == id));
        });
    }

    public void QuitarDelRol(int destino, int componente, bool esRol)
    {
        string nombre = esRol ? Estado.Roles.Single(r => r.Id == componente).Nombre : Estado.Permisos.Single(p => p.Id == componente).Nombre;
        Aplicar($"Quitar de {Estado.Roles.Single(r => r.Id == destino).Nombre}: {nombre}", e =>
        {
            var r = RolEditable(e, destino);
            if (esRol)
            {
                var hijo = r.Roles.SingleOrDefault(x => x.Id == componente) ?? throw new InvalidOperationException("El rol no está asignado directamente.");
                r.QuitarRol(hijo);
            }
            else
            {
                var p = r.Permisos.SingleOrDefault(x => x.Id == componente) ?? throw new InvalidOperationException("El permiso es heredado o no pertenece al rol.");
                r.QuitarPermiso(p);
            }
        });
    }

    public static bool Contiene(Permiso origen, int id) => origen.Id == id || origen.ObtenerHijos().Any(h => Contiene(h, id));

    public IEnumerable<string> Impacto(int familia)
    {
        foreach (var r in Estado.Roles)
            if (Usa(r, familia)) yield return "Rol: " + r.Nombre;
        foreach (var p in Estado.Permisos.Where(p => p.Id != familia && Contiene(p, familia)))
            yield return "Familia: " + p.Nombre;
        foreach (var u in Estado.Usuarios.Where(u => u.Roles.Any(r => Usa(r, familia))))
            yield return "Usuario: " + u.NombreUsuario;
    }

    private static bool Usa(Rol r, int familia) => r.Permisos.Any(p => Contiene(p, familia)) || r.Roles.Any(h => Usa(h, familia));

    public IEnumerable<string> ImpactoRol(int id)
    {
        var rol = Estado.Roles.Find(r => r.Id == id);
        if (rol == null) yield break;
        foreach (var r in Estado.Roles.Where(r => r.Id != id && r.ContieneRol(rol))) yield return "Rol: " + r.Nombre;
        foreach (var u in Estado.Usuarios.Where(u => u.Roles.Any(r => r.ContieneRol(rol)))) yield return "Usuario: " + u.NombreUsuario;
    }

    public int CrearFamilia(string nombre, int? padre = null)
    {
        nombre = nombre.Trim();
        int id = Estado.Permisos.Select(p => p.Id).DefaultIfEmpty(7).Max() + 1;
        Aplicar("Crear familia " + nombre + (padre.HasValue ? " dentro de " + Estado.Permisos.Single(p => p.Id == padre.Value).Nombre : ""), e =>
        {
            var nueva = new PermisoCompuesto { Id = id, Nombre = nombre };
            e.Permisos.Add(nueva);
            if (padre.HasValue) e.Permisos.Single(p => p.Id == padre).Agregar(nueva);
        });
        return id;
    }

    public void AgregarAFamilia(int familia, IEnumerable<int> componentes)
    {
        int[] ids = componentes.ToArray();
        if (ids.Length == 0) return;
        string detalle = string.Join(", ", ids.Select(id => Estado.Permisos.Single(p => p.Id == id).Nombre));
        Aplicar("Agregar a " + Estado.Permisos.Single(p => p.Id == familia).Nombre + ": " + detalle, e =>
        {
            var f = e.Permisos.Single(p => p.Id == familia);
            foreach (int id in ids) f.Agregar(e.Permisos.Single(p => p.Id == id));
        });
    }

    public void QuitarDeFamilia(int familia, int componente)
    {
        Aplicar("Quitar de " + Estado.Permisos.Single(p => p.Id == familia).Nombre + ": " + Estado.Permisos.Single(p => p.Id == componente).Nombre, e =>
        {
            var f = e.Permisos.Single(p => p.Id == familia);
            var hijo = f.ObtenerHijos().SingleOrDefault(p => p.Id == componente)
                ?? throw new InvalidOperationException("Solo se pueden quitar componentes directos de esta familia.");
            f.Quitar(hijo);
        });
    }

    public void EliminarFamilia(int id)
    {
        if (id <= 7) throw new InvalidOperationException("Los permisos base no se pueden eliminar.");
        if (Impacto(id).Any()) throw new InvalidOperationException("La familia está en uso. Quite primero sus asignaciones a roles y familias.");
        Aplicar("Eliminar familia " + Estado.Permisos.Single(p => p.Id == id).Nombre,
            e => e.Permisos.Remove(e.Permisos.Single(p => p.Id == id)));
    }
}
