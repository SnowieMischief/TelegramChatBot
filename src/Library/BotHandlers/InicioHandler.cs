using Telegram.Bot.Types;
namespace Library.BotHandlers;

/// <summary> Dependiendo del <see cref="TipoDeUsuario"/> del <see cref="Usuario"/> muestra distintas opciones.
/// Para un <see cref="Trabajador"/> muestra <see cref="OfertarServicioHandler"/>, <see cref="VerOfertasHandler"/>,
/// <see cref="VerInfoHandler"/> y <see cref="VerSolicitudesHandler"/>.
/// Para un <see cref="Empleador"/> muestra <see cref="VerContratosHandler"/>, <see cref="VerInfoHandler"/> y
/// y <see cref="BuscarHandler"/> </summary>
public class InicioHandler : BaseHandler {
    /// <summary> Enum para indicar el estado del <see cref="InicioHandler"/>. </summary>
    public enum InicioState {
        Start,
        Empleador,
        Trabajador,
        Identificarse
    }

    /// <summary> Estado de <see cref="InicioHandler"/> </summary>
    public InicioState State { get; set; }

    /// <summary> Inicializa una nueva instancia de la clase <see cref="InicioHandler"/>. Esta clase procesa el mensaje "Inicio". </summary>
    /// <param name="next">El próximo "handler".</param>
    public InicioHandler(BaseHandler next) : base(next) {
        this.Keywords = new string[] {"/inicio", "inicio"};
        this.State = InicioState.Start;
        this._id = Handlers.InicioHandler;
    }

    /// <summary> Diccionario que guarda el estado en el <see cref="IHandler"/> según el ID de Telegram. </summary>
    /// <typeparam name="long"> ID de usuario de Telegram. </typeparam>
    /// <typeparam name="LoginState"> Estado del <see cref="IHandler"/>. </typeparam>
    private Dictionary<long, InicioState> Posiciones = new Dictionary<long, InicioState>();

    /// <summary> Verifica que se pueda procesar el mensaje </summary>
    /// <param name="message"> Mensaje a procesar </param>
    /// <returns> true si puede procesar el mensaje, false en caso contrario </returns>
    protected override bool CanHandle(Message message) {
        if (!this.Posiciones.ContainsKey(message.From.Id)) {
            this.Posiciones[message.From.Id] = InicioState.Start;
        }
        switch (this.Posiciones[message.From.Id]) {
            case InicioState.Start:
                return base.CanHandle(message);
            default:
                return true;
        }
    }

    /// <summary> Procesamiento de los mensajes. </summary>
    /// <param name="message"> Mensaje a procesar </param>
    /// <param name="response"> Respuesta al mensaje procesado </param>
    protected override void InternalHandle(Message message, out string response) {
        if (message == null || message.From == null) {
            throw new Exception("No se recibió un mensaje");
        }
        
        response = "Error desconocido";
        
        this.State = this.Posiciones[message.From.Id];

        switch(State) {
            case InicioState.Start:
                this.Posiciones[message.From.Id] = InicioState.Identificarse;
                response = "Identificarse como:\n1) Trabajador\n2) Empleador";
                break;

            case InicioState.Identificarse:
                switch(message.Text) {
                    case "1":
                        this.Posiciones[message.From.Id] = InicioState.Trabajador;
                        response = "¿Que operación desea realizar?\n1) Ofertar un servicio\n2) Ver mis ofertas\n3) Ver mis datos personales\n4) Ver mis solicitudes pendientes\n5) Volver al inicio";
                        break;
                    case "2":
                        this.Posiciones[message.From.Id] = InicioState.Empleador;
                        response = "¿Que operación desea realizar?\n1) Ver mis contratos\n2) Ver mis datos personales\n3) Buscar ofertas\n4) Volver al inicio";
                        break;
                    default:
                        this.Posiciones[message.From.Id] = InicioState.Identificarse;
                        response = "Verifique que la identificación sea correcta, ingrese solo el número\n\nIdentificarse como:\n1) Empleador\n2) Trabajador";
                        break;
                }
                break;

            case InicioState.Trabajador:
                switch (message.Text) {
                    case "1":
                        response = "Elija una categoría de las siguientes para ofertar\n (Lista de las ofertas) ";
                        switch (message.Text) {
                            case "":
                                break;
                            default:
                                response = "Asegurese de que la categoría seleccionada exista, ingrese solo el número\nEliga una categoría de las siguientes para ofertar\n (Lista de las ofertas) ";
                                break;
                        }
                        break;

                    case "2":
                        response = "";
                        break;

                    case "3":
                        response = "(Información acá)\n\n¿Desea realizar alguna operación más?\n1) Darse de baja\n2) No";
                        switch (message.Text) {
                            case "1":
                                response = "Ingrese a continuación su contraseña\n» ";
                                // TODO Verificación de contraseña
                                break;
                            case "2":
                                break;
                        }
                        break;

                    case "4":
                        response = "(Solicitudes pendientes)";
                        break;
                    
                    case "5":
                        this.Posiciones[message.From.Id] = InicioState.Start; 
                        response = "Identificarse como:\n1) Empleador\n2) Trabajador";
                        break;

                    default:
                        response = "Asegurese de que la opción elegida sea valida, ingrese solo el número";
                        this.Posiciones[message.From.Id] = InicioState.Trabajador;
                        break;
                }
                break;
            
            case InicioState.Empleador:
                switch (message.Text) {
                    case "1":
                        response = "(Lista de contratos)";  //TODO Mostrar lista de contratos
                        break;

                    case "2":
                        response = "(Datos personales)"; //TODO Mostrar datos
                        break;

                    case "3":
                        response = "(Buscador ofertas)";
                        break;
                    
                    case "4":
                        this.Posiciones[message.From.Id] = InicioState.Start;
                        response = "Volviendo al inicio";
                        break;

                    default:
                        response = "Asegurese de que la opción elegida sea valida, ingrese solo el número";
                        this.Posiciones[message.From.Id] = InicioState.Empleador;
                        break;
                    }
                    break;
            }
        Console.WriteLine(response);
    }
}
