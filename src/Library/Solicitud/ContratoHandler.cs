using System.Security.Authentication;
using Library.Excepciones;

namespace Library;
using System;

/// <summary> Clase para manejar el catálogo. </summary>
/// /// <!-- Utilizamos patrón singleton ya que solo necesitamos una misma instancia de esta clase, si hubieran más
/// se mezclarían los elementos de la misma y no sabríamos a cual instancia acceder para interactuar con las solicitudes .-->
public class ContratoHandler
{

    public SolicitudCatalog Catalogo { get { return SolicitudCatalog.GetInstance(); } }
    
    private static ContratoHandler? _instance;

    private static ContratoHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ContratoHandler();
            }

            return _instance;
        }
    }
    
    /// <summary> Constructor tipo Singleton de la clase. </summary>
    private ContratoHandler()
    {
    }

    /// <summary> Método para obtener la instancia de la clase. </summary>
    /// <returns> Devuelve la instancia. </returns>
    public static ContratoHandler GetInstance()
    {
        return ContratoHandler.Instance;
    }

    /// <summary> Método para borrar los datos de la clase. </summary>
    /// <param name="user"> Tipo de usuario que llama al método. </param>

    public static void Wipe(Usuario user)
    {
        if (user.GetTipo().Equals(TipoDeUsuario.Administrador))
        {
            ContratoHandler._instance = null;
        }
        else
        {
            throw (new ElevacionException("Solo un administrador puede utilizar el método Wipe() de ContratoHandler"));
        }
    }

    /// <summary> Método que crea una solicitud de trabajo. </summary>
    /// <param name="oferta"> Oferta en cuestión. </param>
    /// <param name="emp"> Empleador que va a realizar la solicitud. </param>
    public Solicitud SolicitarTrabajador(OfertaDeServicio oferta, Empleador emp){
        Solicitud solicitud = Catalogo.AddSolicitud(oferta, emp);
        return solicitud;
    }

    /// <summary> Método para aceptar una solicitud. </summary>
    /// <param name="solicitud"> Variable de tipo <see cref="Solicitud"> para aceptar. </param>
    public void AceptarSolicitud(Usuario user, Solicitud solicitud){
        if(user.Nick.Equals(solicitud.Oferta.GetOfertante()))
        {
            solicitud.RecibirRespuesta(Aceptacion.Aceptada);
        }
        else
        {
            throw (new UsuarioIncorrectoException("El trabajador no coincide con el de la oferta"));
        }
    }

    /// <summary> Método para rechazar una solicitud. </summary>
    /// <param name="solicitud"> Variable de tipo <see cref="Solicitud"> para rechazar. </param>
    public void RechazarSolicitud(Usuario user, Solicitud solicitud){
        if(user.Nick.Equals(solicitud.Oferta.GetOfertante()))
        {
            solicitud.RecibirRespuesta(Aceptacion.Rechazada);
        }
        else
        {
            throw (new UsuarioIncorrectoException("El trabajador no coincide con el de la oferta"));
        }
    }


    /// <summary> Método para obtener una solicitud por id </summary>
    /// <param name="id"> Valor de id para filtrar <see cref="Solicitud"/> </param>
    /// <returns> Devuelve la <see cref="Solicitud"/> por valor de id </returns>
    public Solicitud GetSolicitud(int id)
    {
        return this.Catalogo.GetSolicitud(id);
    }
    
    /// <summary> Método para mostrar solicitudes. </summary>
    /// <param name="user"> Variable de tipo <see cref="Usuario"/>. </param>
    /// <returns> Devuelve las solicitudes según que tipo de <see cref="Usuario"/> se. </returns>
    public List<Solicitud> GetSolicitudes(Usuario user)
    {
        List<Solicitud> solicitudesDelUsuario = new();
        if (user.GetTipo().Equals(TipoDeUsuario.Administrador))
        {
            solicitudesDelUsuario = this.Catalogo.Solicitudes;
        }

        else if (user.GetTipo().Equals(TipoDeUsuario.Trabajador))
        {
            foreach (Solicitud solicitud in Catalogo.Solicitudes)
            {
                if(solicitud.Trab.Equals(user.Nick)) solicitudesDelUsuario.Add(solicitud);
            }
        }
        else if (user.GetTipo().Equals(TipoDeUsuario.Empleador))
        {
            foreach (Solicitud solicitud in Catalogo.Solicitudes)
            {
                if(solicitud.GetEmpleador().Equals(user.Nick)) solicitudesDelUsuario.Add(solicitud);
            }
        }
        else
        {
            throw (new("Error: tipo de usuario no definido"));
        }
        return solicitudesDelUsuario;
    }
}
