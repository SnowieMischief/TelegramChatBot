using Telegram.Bot.Types;
using System.Text;
namespace Library.BotHandlers;

/// <summary> Handler para manejar el ingreso de datos del <see cref="Usuario"/>. </summary>
public class RegistrarHandler : BaseHandler
{
    private RegistryHandler RegistryHandler { get { return RegistryHandler.GetInstance(); } }
    /// <summary> Enum para conocer el estado de <see cref="RegistrarHandler"/> </summary>
    public enum RegistrarState
    {
        Start,
        LecturaRol,
        LecturaNombre,
        LecturaApellido,
        LecturaNick,
        LecturaContraseña,
        LecturaFechaNacimiento,
        LecturaCedula,
        LecturaTelefono,
        LecturaCorreo,
        LecturaUbicacion,
        Confirmar,
        Fin,
    }
    
    /// <summary> Obtiene el <see cref="TipoDeUsuario"/> que accede al handler. </summary>
    public Dictionary<long,TipoDeUsuario> TempTipo= new ();

    
    /// <summary> Diccionario al que se añaden los datos del nuevo <see cref="Usuario"/> que se está creando </summary>
    public Dictionary<long, Dictionary<string, string>> GlobalTempInfo = new();
    

    /// <summary> Estado de <see cref="RegistrarHandler"/> </summary>
    public RegistrarState State { get; set; }
    
    /// <summary> Diccionario que guarda el estado en el <see cref="IHandler"/> según el ID de Telegram. </summary>
    /// <typeparam name="long"> ID de usuario de Telegram. </typeparam>
    /// <typeparam name="LoginState"> Estado del <see cref="IHandler"/>. </typeparam>
    private Dictionary<long, RegistrarState> Posiciones = new Dictionary<long, RegistrarState>();

    /// <summary> Inicializa una nueva instancia de la clase <see cref="RegistrarHandler"/>. </summary>
    /// <param name="next"> Próximo <see cref="IHandler"/> </param>
    public RegistrarHandler(BaseHandler? next) : base(next) {
        this.Keywords = new string[] {"registrar", "/registrar" };
        this.State = RegistrarState.Start;
        this._id = Handlers.RegistrarHandler;
    }

    /// <summary> Verifica que se pueda procesar el mensaje </summary>
    /// <param name="message"> Mensaje a procesar </param>
    /// <returns> true si puede procesar el mensaje, false en caso contrario </returns>
    protected override bool CanHandle(Message message)
    {
        if (!this.Posiciones.ContainsKey(message.From.Id))
        {
            this.Posiciones[message.From.Id] = RegistrarState.Start;
        }

        switch (this.Posiciones[message.From.Id])
        {
            case RegistrarState.Start:
                return base.CanHandle(message);
            default:
                return true;
        }
    }
    
    
    /// <summary> Procesamiento de los mensajes. </summary>
    /// <param name="message"> Mensaje a procesar </param>
    /// <param name="response"> Respuesta al mensaje procesado </param>
    protected override void InternalHandle(Message message, out string response)
    {
        if (!GlobalTempInfo.ContainsKey(message.From.Id))
            GlobalTempInfo[message.From.Id] = new Dictionary<string, string>();
        
        if (message == null || message.From == null || message.Text == null)
        {
            throw new Exception("No se recibió un mensaje");
        }
        
        message.Text = message.Text.ToLower().Trim();

        response = "Error desconocido vuelva a intentarlo";
        
        switch (this.Posiciones[message.From.Id])
        {
            case RegistrarState.Start:
                response = "Selecciona tu rol:\n1) Trabajador \n2) Empleador\n3) Regresar al inicio";
                this.Posiciones[message.From.Id] = RegistrarState.LecturaRol;
                break;
            
            case RegistrarState.LecturaRol:
                response = "Ingresa tu(s) nombre(s), escribe cancelar para volver al inicio";
                this.Posiciones[message.From.Id] = RegistrarState.LecturaNombre;
                switch (message.Text)
                {
                    case "1":
                        this.TempTipo[message.From.Id] = TipoDeUsuario.Trabajador;
                        break;
                    case "2":
                        this.TempTipo[message.From.Id] = TipoDeUsuario.Empleador;
                        break;
                    case "3":
                        response = "Regresando al menu";
                        this.Posiciones[message.From.Id] = RegistrarState.Start;
                        break;
                    default:
                        response = "Error, intentalo de nuevo";
                        this.Posiciones[message.From.Id] = RegistrarState.LecturaRol;
                        break;
                }

                break;
            
            case RegistrarState.LecturaNombre:
                this.GlobalTempInfo[message.From.Id]["nombre"] = message.Text;
                response = "Ingresa tu(s) apellido(s), escribe \"cancelar\" parar volver al inicio";
                this.Posiciones[message.From.Id] = RegistrarState.LecturaApellido;
                if(message.Text.Equals("cancelar"))
                {
                        this.Posiciones[message.From.Id] = RegistrarState.Start;
                        response = "Volviendo al inicio"; 
                        //this.TempInfo = new Dictionary<string, string>();
                        //this.TempTipo = null;
                }

                break;
            
            case RegistrarState.LecturaApellido:
                this.GlobalTempInfo[message.From.Id]["apellido"] = message.Text;
                response = "Ingresa un nombre de usuario, escribe \"cancelar\" parar volver al inicio";
                this.Posiciones[message.From.Id] = RegistrarState.LecturaNick;
                if(message.Text.Equals("cancelar"))
                {
                    this.Posiciones[message.From.Id] = RegistrarState.Start;
                    response = "Volviendo al inicio"; 
                    //this.TempInfo = new Dictionary<string, string>();
                    //this.TempTipo = null;
                }

                break;
            
            case RegistrarState.LecturaNick:
                if (!RegistryHandler.VerificarNick(message.Text))
                {
                    response = "Ese nick no es válido o ya fue tomado por otro usuario, ingresa otro o escribe" + 
                               " \"cancelar\" parar volver al inicio";
                    break;
                }
                this.GlobalTempInfo[message.From.Id]["nick"] = message.Text;
                response = "Ingresa una contraseña, escribe \"cancelar\" parar volver al inicio";
                this.Posiciones[message.From.Id] = RegistrarState.LecturaContraseña;
                if(message.Text.Equals("cancelar"))
                {
                    this.Posiciones[message.From.Id] = RegistrarState.Start;
                    response = "Volviendo al inicio"; 
                    //this.TempInfo = new Dictionary<string, string>();
                    //this.TempTipo = null;
                }

                break;
            
            case RegistrarState.LecturaContraseña:
                if (message.Text.Equals("volver"))
                {
                    response =
                        "La palabra \"volver\" es una palabra reservada del programa y no se puede utilizar, por favor ingresa otra contraseña";
                    break;
                }
                this.GlobalTempInfo[message.From.Id]["contraseña"] = message.Text;
                response = "Ingresa tu fecha de nacimiento (formato \"día/mes/año\"), escribe \"cancelar\" parar volver al inicio";
                this.Posiciones[message.From.Id] = RegistrarState.LecturaFechaNacimiento;
                if(message.Text.Equals("cancelar"))
                {
                    this.Posiciones[message.From.Id] = RegistrarState.Start;
                    response = "Volviendo al inicio"; 
                    //this.TempInfo = new Dictionary<string, string>();
                    //this.TempTipo = null;
                }

                break;
            
            case RegistrarState.LecturaFechaNacimiento:
                try
                {
                    DateTime.ParseExact(message.Text, "dd/MM/yyyy", null);
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e);
                    response =
                        "Formato no aceptado, vuelve a intentarlo usando el formado \"día/mes/año\" sin comillas";
                    break;
                }
                this.GlobalTempInfo[message.From.Id]["fechaNacimiento"] = message.Text;
                response = "Ingresa tu cedula (con puntos y guion), escribe \"cancelar\" parar volver al inicio";
                this.Posiciones[message.From.Id] = RegistrarState.LecturaCedula;
                if(message.Text.Equals("cancelar"))
                {
                    this.Posiciones[message.From.Id] = RegistrarState.Start;
                    response = "Volviendo al inicio"; 
                    //this.TempInfo = new Dictionary<string, string>();
                    //this.TempTipo = null;
                }

                break;
            
            case RegistrarState.LecturaCedula:
                string parsed = RegistryHandler.ParseCedula(message.Text);
                if (!RegistryHandler.VerificarCedula(message.Text))
                {
                    response = $"Cédula no válida (leída como {parsed}), inténtalo de nuevo o escribe " + 
                               "\"cancelar\" parar volver al inicio";
                    break;
                }
                this.GlobalTempInfo[message.From.Id]["cedula"] = parsed;
                
                response = "Ingresa un teléfono para que puedan contactarte (será privado hasta que aceptes hablar con" +
                           " otro usuario), escribe \"cancelar\" parar volver al inicio";
                this.Posiciones[message.From.Id] = RegistrarState.LecturaTelefono;
                if(message.Text.Equals("cancelar"))
                {
                    this.Posiciones[message.From.Id] = RegistrarState.Start;
                    response = "Volviendo al inicio"; 
                }

                break;
            
            case RegistrarState.LecturaTelefono:
                this.GlobalTempInfo[message.From.Id]["telefono"] = message.Text;
                response = "Ingresa un correo para que puedan contactarte (será privado hasta que aceptes hablar con" +
                           "otro usuario), escribe \"cancelar\" parar volver al inicio";
                this.Posiciones[message.From.Id] = RegistrarState.LecturaCorreo;
                if(message.Text.ToLower().Equals("cancelar"))
                {
                    this.Posiciones[message.From.Id] = RegistrarState.Start;
                    response = "Volviendo al inicio";
                }

                break;
            
            case RegistrarState.LecturaCorreo:
                if (!RegistryHandler.VerificarCorreo(message.Text))
                {
                    response = $"Correo no válido o formato no aceptado, inténtalo de nuevo o escribe " + 
                               "\"cancelar\" parar volver al inicio";
                    break;
                }
                this.GlobalTempInfo[message.From.Id]["correo"] = message.Text;
                response = "Ingresa tu dirección (calle y numero), escribe \"cancelar\" parar volver al inicio";
                this.Posiciones[message.From.Id] = RegistrarState.Confirmar;
                if(message.Text.Equals("cancelar"))
                {
                    this.Posiciones[message.From.Id] = RegistrarState.Start;
                    response = "Volviendo al inicio";
                }

                break;
            
            case RegistrarState.Confirmar:
                bool confidence;
                try{
                    confidence = AddressValidation.Process(message.Text);
                    if (!confidence) {
                        response = "Ubicación inválida, escríbala nuevamente.";
                        return;
                    }
                } catch (Exception e)
                {
                    Console.WriteLine("Error con la request");
                }
                var latLong= Geocode.Process(message.Text);

                string lng = latLong.Item1.ToString();
                string lat = latLong.Item2.ToString();
                
                this.GlobalTempInfo[message.From.Id]["ubicacionLng"] = lng;
                this.GlobalTempInfo[message.From.Id]["ubicacionLat"] = lat;

                StringBuilder respuesta = new StringBuilder();
                respuesta.Append("Ingresaste la siguiente información:\n");
                foreach (KeyValuePair<string,string> keyValuePair in GlobalTempInfo[message.From.Id])
                {
                    respuesta.Append($"{keyValuePair.Key}: {keyValuePair.Value} \n");
                }
                respuesta.Append("¿Guardar tu usuario? (Si/No)");
                response = respuesta.ToString();
                this.Posiciones[message.From.Id] = RegistrarState.Fin;
                if(message.Text.Equals("cancelar"))
                {
                    this.Posiciones[message.From.Id] = RegistrarState.Start;
                    response = "Volviendo al inicio"; 
                }

                break;
            case RegistrarState.Fin:
                bool cancelado = false;
                bool aceptado = false;
                switch (message.Text)
                {
                    case "no":
                        response = "Cancelado, volviendo a inicio";
                        this.Posiciones[message.From.Id] = RegistrarState.Start;
                        cancelado = true;
                        aceptado = true;
                        break;
                    case "si":
                        aceptado = true;
                        break;
                    case "default":
                        response = "Intentalo de nuevo";
                        break;
                }
                
                if(cancelado == true || aceptado == false) break;
                
                string nombre = this.GlobalTempInfo[message.From.Id]["nombre"];
                string apellido = this.GlobalTempInfo[message.From.Id]["apellido"];
                string nick = this.GlobalTempInfo[message.From.Id]["nick"];
                string contraseña = this.GlobalTempInfo[message.From.Id]["contraseña"];
                string fechaNacimiento = this.GlobalTempInfo[message.From.Id]["fechaNacimiento"];
                string cedula = this.GlobalTempInfo[message.From.Id]["cedula"];
                string telefono = this.GlobalTempInfo[message.From.Id]["telefono"];
                string correo = this.GlobalTempInfo[message.From.Id]["correo"];
                double doubleLat = double.Parse(GlobalTempInfo[message.From.Id]["ubicacionLat"]);
                double doubleLng = double.Parse(GlobalTempInfo[message.From.Id]["ubicacionLng"]);
                Tuple<double, double> ubicacion = new Tuple<double, double>(doubleLat, doubleLng);
                // TODO usar un método que cree una tupla de coordenadas a partir del string de dirección del usuario
                switch (TempTipo[message.From.Id])
                {
                    case TipoDeUsuario.Empleador:
                        try
                        {
                            RegistryHandler.RegistrarEmpleador(nombre, apellido, nick, contraseña, fechaNacimiento, cedula,
                                telefono, correo, ubicacion);
                        }
                        catch (Exception e)
                        {
                            response = "Error desconocido, volviendo a inicio";
                            this.Posiciones[message.From.Id] = RegistrarState.Start;
                            break;
                        }
                        response = "Registrado con éxito, volviendo a inicio para que puedas iniciar sesión";
                        break;
                    case TipoDeUsuario.Trabajador:
                        try
                        {
                            RegistryHandler.RegistrarTrabajador(nombre, apellido, nick, contraseña, fechaNacimiento, cedula,
                                telefono, correo, ubicacion);
                        }
                        catch (Exception e)
                        {
                            response = "Error desconocido, volviendo a inicio";
                            this.Posiciones[message.From.Id] = RegistrarState.Start;
                            break;
                        }
                        response = "Registrado con éxito, volviendo a inicio para que puedas iniciar sesión";
                        break;
                    default:
                        response = "Error desconocido, volviendo a inicio";
                        this.Posiciones[message.From.Id] = RegistrarState.Start;
                        break;
                }
                this.Posiciones[message.From.Id] = RegistrarState.Start;
                break;
            default:
                break;
        }
    }
}
