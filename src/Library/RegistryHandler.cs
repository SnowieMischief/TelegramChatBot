namespace Library;
using System;

/// <summary> Clase para manejar el registro </summary>
public class RegistryHandler
{
    private UsuariosCatalog usuarios;
    
    private static RegistryHandler? _instance;

    private static RegistryHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RegistryHandler();
            }

            return _instance;
        }
    }
    
    /// <summary> Método para borrar los datos de la clase </summary>
    /// <param name="user"> Tipo de usuario que llama al método </param>
    public static void Wipe(Usuario user)
    {
        if (user.GetTipo().Equals(TipoDeUsuario.Administrador))
        {
            RegistryHandler._instance = null;
        }
    }
    
    /// <summary> Constructor de tipo Singleton de la clase</summary>
    private RegistryHandler()
    {
        this.usuarios = UsuariosCatalog.GetInstance();
    }

    /// <summary> Método para obtener la instancia </summary>
    /// <returns> Devuelve la instancia </returns>
    public static RegistryHandler GetInstance()
    {
        return RegistryHandler.Instance;
    }

    // TODO documentar que el "fechaNacimiento" se tiene que introducir como "año,mes,dia". Idem para la cedula, correo, etc
    /// <summary> Método para registrar un trabajador </summary>
    /// <param name="nombre"> Nombre del usuario </param> 
    /// <param name="apellido"> Apellido del usuario </param> 
    /// <param name="contraseña"> Contraseña del usuario </param> 
    /// <param name="fechaNacimiento"> Fecha de nacimiento del usuario </param> 
    /// <param name="cedula"> Cédula del usuario </param> 
    /// <param name="telefono"> Teléfono del usuario </param> 
    /// <param name="correo"> Correo electrónico del usuario </param> 
    /// <param name="ubicacion"> Ubicación </param>
    /// <returns> Devuelve la instancia de <see cref="Trabajador"/> creada </returns>
    public Trabajador RegistrarTrabajador(string nombre, string apellido, string nick, string contraseña, string fechaNacimiento, 
                                          string cedula, string telefono, string correo, Tuple<double,double> ubicacion)
    {
        DateTime nacimiento = DateTime.Parse(fechaNacimiento);
        if (VerificarCorreo(correo) && VerificarCedula(cedula) && VerificarNick(nick))
        {
            Trabajador nuevoTrabajador = (Trabajador)usuarios.AddUsuario( TipoDeUsuario.Trabajador, nombre,  apellido,  nick,  contraseña,  nacimiento, 
                 cedula,  telefono,  correo,  ubicacion);
            return nuevoTrabajador;
        }
        throw (new ArgumentException("Alguno de los valores introducidos no fue válido"));
    }

    /// <summary> Método para registrar un empleador </summary>
    /// <param name="nombre"> Nombre del empleador </param> 
    /// <param name="apellido"> Apellido del empleador </param> 
    /// <param name="contraseña"> Contraseña del empleador </param> 
    /// <param name="fechaNacimiento"> Fecha de nacimiento del empleador </param> 
    /// <param name="cedula"> Cédula del empleador </param> 
    /// <param name="telefono"> Teléfono del empleador </param> 
    /// <param name="correo"> Correo electrónico del empleador </param> 
    /// <param name="ubicacion"> Ubicación del empleador </param>
    /// <returns> Devuelve la instancia de <see cref="Empleador"/> creada </returns>
    public Empleador RegistrarEmpleador(string nombre, string apellido, string nick, string contraseña, string fechaNacimiento, 
                                        string cedula, string telefono, string correo, Tuple<double,double> ubicacion)
    {
        DateTime nacimiento = DateTime.Parse(fechaNacimiento);
        if (VerificarCorreo(correo) && VerificarCedula(cedula) && VerificarNick(nick))
        {
            Empleador nuevoEmpleador = (Empleador)usuarios.AddUsuario( TipoDeUsuario.Empleador, nombre,  apellido,  nick,  contraseña,  nacimiento, 
                cedula,  telefono,  correo,  ubicacion);
            return nuevoEmpleador;
        }

        throw (new ArgumentException("Alguno de los valores introducidos no fue válido"));

    }

    /// <summary> Método para registrar un administrador </summary>
    /// <param name="nick">nick del administrador</param>
    /// <param name="contraseña">contraseña del administrador</param>
    /// <param name="telefono">teléfono del administrador</param>
    /// <param name="correo">correo del administrador</param>
    /// <returns> Devuelve la instancia de <see cref="Trabajador"/> creada </returns>
    public Administrador RegistrarAdministrador(string nick, string contraseña, string telefono, string correo)
    {
        if (VerificarCorreo(correo) && VerificarNick(nick))
        {
            Administrador nuevoAdministrador = (Administrador)usuarios.AddAdminstrador(nick, contraseña, telefono, correo);
            return nuevoAdministrador;
        }
        throw (new ArgumentException("Alguno de los valores introducidos no fue válido"));
    }

    /// <summary> Método para verificar el nickname de un <see cref="Usuario"/> </summary>
    /// <param name="nick"> Nickname del <see cref="Usuario"/> </param>
    /// <returns> Devuelve true si no existe otro <see cref="Usuario"/> con ese nick, de lo contrario devuelve false </returns>
    public bool VerificarNick(string nick)
    {
        foreach (Usuario usuario in usuarios.GetUsuarios())
        {
            if (usuario.Nick.Equals(nick)) return false;
        }
        return true;
    }
    
    /// <summary> Método para verificar el correo de un <see cref="Usuario"/> </summary>
    /// <param name="correo"> Correo del <see cref="Usuario"/> </param>
    /// <returns> Devuelve true si el formato del correo es válido, de lo contrario devuelve false </returns>
    public bool VerificarCorreo(string correo)
    {
        bool arroba = false;
        bool punto = false;
        foreach (char caracter in correo)
        {
            if (caracter.Equals('@'))
            {
                if (arroba) return false;
                arroba = true;
            }
            if (caracter.Equals('.'))
            {
                if (arroba) punto = true;
            }
        }
        if (punto) return true;
        return false;
    }

    /// <summary> Método para verificar la cédula de un <see cref="Usuario"/> </summary>
    /// <param name="cedula"> Cédula del <see cref="Usuario"/> </param>
    /// <returns> Devuelve true si el formato es válido, de lo contrario devuelve false </returns>
    public bool VerificarCedula(string cedula)
    {
        cedula = cedula.Replace(".", string.Empty);
        cedula = cedula.Replace("-", string.Empty);
        string validos = "0123456789";
        foreach (char caracteres in cedula)
        {
            if (validos.Contains(caracteres))
            {
                if (cedula.Length > 6 && cedula.Length <= 8)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    /// <summary> Método para eliminar un <see cref="Usuario"/> </summary>
    /// <param name="usuario"> <see cref="Usuario"/> que se desea eliminar </param>
    public void RemoveUsuario(Usuario admin, Usuario usuarioEliminar)
    {
        if (!admin.GetTipo().Equals(TipoDeUsuario.Administrador))
            throw new("Solo un administrador puede eliminar usuarios");
        if (!usuarios.GetUsuarios().Contains(usuarioEliminar)) {
            throw new ArgumentNullException("El usuario ingresado no existe");
        }
        else {
            usuarios.RemoveUsuario(admin,usuarioEliminar);
        }
    }

    /// <summary> Método para obtener los datos de un <see cref="Usuario"/> </summary>
    /// <param name="nombre"> Nombre del usuario </param>
    /// <param name="apellido"> Apellido del usuario </param>
    /// <param name="contraseña"> Contraseña del usuario </param>
    /// <returns> Devuelve el <see cref="Usuario"/> que coincida con los parámetros dados </returns>
    public Usuario GetUsuario(string nickname, string contraseña)
    {
        foreach (Usuario user in usuarios.GetUsuarios())
        {
            if (user.Nick.Equals(nickname) && user.VerifyContraseña(contraseña))
            {
                return user;
             }
        }
        throw (new ArgumentException("Los datos introducidos no coinciden con ningun usuario"));
    }

    /// <summary> Método para obtener reputación de un trabajador o empleador </summary>
    /// <param name="nickname"> Nickname del usuario </param>
    /// <returns> Devuelve la reputación </returns>
    public Calificacion GetReputacion(string nickname)
    {
        foreach (Usuario user in usuarios.GetUsuarios())
        {
            if(user.Nick.Equals(nickname))
            {
                if (user is Trabajador) return ((Trabajador)user).GetReputacion();
                if (user is Empleador) return ((Empleador)user).GetReputacion();
            }
        }
        throw (new("Usuario no encontrado"));
    }
    
    /// <summary> Método para obtener la información pública de un usuario </summary>
    /// <param name="nickname"> Nickname del usuario </param>
    /// <returns> Devuelve la información del usuario </returns>

    public Dictionary<string, string> GetUserInfo(string nickname)
    {
        Usuario user = GetUser(nickname);
        return user.GetPublicInfo();
    }
    /// <summary> Método para obtener el contacto de un usuario </summary>
    /// <param name="nickname"> Nickname del usuario </param>
    /// <returns> Devuelve el contacto del usuario </returns>

    public Dictionary<string, string> GetUserContact(string nickname)
    {
        Usuario user = GetUser(nickname);
        return user.GetContacto();
    }

    /// <summary> Método para obtener una instancia de un usuario </summary>
    /// <param name="nickname"> Nickname del usuario </param>
    /// <returns> Devuelve la instancia del usuario en caso de que exista </returns>
    private Usuario GetUser(string nickname)
    {
        Usuario? user = null;
        try {
            foreach (Usuario usuario in usuarios.GetUsuarios()) {
                if (usuario.Nick.Equals(nickname)) {
                    user = usuario;
                }
            }
        }
        catch(NullReferenceException) {
            throw (new Exception("No se encontró el usuario"));
        }
        return user;
    }

    /// <summary> Método para obtener la lista de trabajadores </summary>
    /// <returns> Devuelve lista de trabajadores </returns>
    public List<string> GetTrabajadores()
    {
        List<string> trabajadores = new();
        foreach (Usuario usuario in usuarios.GetUsuarios())
        {
            if (usuario.GetTipo().Equals(TipoDeUsuario.Trabajador))
            {
                trabajadores.Add(usuario.Nick);
            }
        }

        return trabajadores;
    }
    
    /// <summary> Método para obtener la lista de empleadores </summary>
    /// <returns> Devuelve lista de empleadores </returns>
    public List<string> GetEmpleadores()
    {
        List<string> empleadores = new();
        foreach (Usuario usuario in usuarios.GetUsuarios())
        {
            if (usuario.GetTipo().Equals(TipoDeUsuario.Trabajador))
            {
                empleadores.Add(usuario.Nick);
            }
        }

        return empleadores;
    }

    /// <summary> Método para obtener lista de usuarios calificables </summary>
    /// <returns> Devuelve lista </returns>
    private List<ICalificable> NonAdmins()
    {
        List<ICalificable> nonAdmins = new();
        foreach (Usuario user in usuarios.GetUsuarios())
        {
            if (user is ICalificable)
            {
                nonAdmins.Add((ICalificable)user);
            }
        }
        return NonAdmins();
    }
}
