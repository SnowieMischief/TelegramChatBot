using Telegram.Bot;
using Telegram.Bot.Types;
namespace Library.BotHandlers;

/// <summary> Clase base para implementar el patrón Chain of Responsibility. En ese patrón se pasa un mensaje a través de una
/// cadena de "handlers" que pueden procesar o no el mensaje. Cada "handler" decide si procesa el mensaje, o si se lo
/// pasa al siguiente. Esta clase base implmementa la responsabilidad de recibir el mensaje y pasarlo al siguiente
/// "handler" en caso que el mensaje no sea procesado. La responsabilidad de decidir si el mensaje se procesa o no, y
/// de procesarlo, se delega a las clases sucesoras de esta clase base. </summary>
public abstract class BaseHandler : IHandler
{
    //TODO que es esto??????
    protected Handlers _id { get; set; }

    /// <summary> Método para obtener el id de un <see cref="IHandler"/>. </summary>
    /// <returns> Devuelve el id correspondiente. </returns>
    public Handlers GetId()
    {
        return this._id;
    }
    
    /// <summary> Obtiene el próximo "handler". </summary>
    /// <value> El "handler" que será invocado si este "handler" no procesa el mensaje. </value>
    public IHandler? Next { get; set; }

    /// <summary> Obtiene o asigna el conjunto de palabras clave que este "handler" puede procesar. </summary>
    /// <value> Un array de palabras clave. </value>
    public string[] Keywords { get; set; }

    /// <summary> Inicializa una nueva instancia de la clase <see cref="BaseHandler"/>. </summary>
    /// <param name="next"> El próximo "handler". </param>
    public BaseHandler(IHandler? next)
    {
        this.Next = next;
    }

    /// <summary> Inicializa una nueva instancia de la clase <see cref="BaseHandler"/> con una lista de comandos. </summary>
    /// <param name="keywords"> La lista de comandos. </param>
    /// <param name="next"> El próximo "handler". </param>
    public BaseHandler(string[] keywords, BaseHandler next)
    {
        this.Keywords = keywords;
        this.Next = next;
    }

    /// <summary> Se procesa el mensaje y asigna la respuesta al mensaje. </summary>
    /// <param name="message"> El mensaje a procesar. </param>
    /// <param name="response"> La respuesta al mensaje procesado. </param>
    protected abstract void InternalHandle(Message message, out string response);

    /// <summary> Permite volver al estado inicial de un <see cref="IHandler"/>. </summary>
    protected virtual void InternalCancel()
    {
        // Intencionalmente en blanco.
    }

    /// <summary> Determina si el mensaje puede ser procesado por el <see cref="IHandler"/>, 
    /// se busca el texto en el mensaje ignorando mayúsculas y miúsculas. </summary>
    /// <param name="message"> El mensaje a procesar. </param>
    /// <returns> true si el mensaje puede ser pocesado, false en caso contrario. </returns>
    protected virtual bool CanHandle(Message message)
    {
        // Cuando no hay palabras clave este método debe ser sobreescrito por las clases sucesoras y por lo tanto
        // este método no debería haberse invocado.
        if (this.Keywords == null || this.Keywords.Length == 0)
        {
            throw new InvalidOperationException("No hay palabras clave que puedan ser procesadas");
        }

        return this.Keywords.Any(s => message.Text.Equals(s, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary> Procesa el mensaje o lo pasa al siguiente "handler" si existe. </summary>
    /// <param name="message"> El mensaje a procesar. </param>
    /// <param name="response"> La respuesta al mensaje procesado. </param>
    /// <returns> El "handler" que procesó el mensaje si el mensaje fue procesado, null en caso contrario. </returns>
    public IHandler Handle(Message message, out string response)
    {
        if (this.CanHandle(message))
        {
            // TelegramBot.Posiciones[message.From.Id] = this._id;
            // Console.WriteLine($"El usuario {message.From.FirstName} está en {this._id}");
            this.InternalHandle(message, out response);
            HandlerHandler.ActiveHandler[message.From.Id] = _id;
            return this;
        }
        else if (this.Next != null)
        {
            return this.Next.Handle(message, out response);
        }
        else
        {
            response = String.Empty;
            return null;
        }
    }

    /// <summary> Retorna este "handler" al estado inicial. En los "handler" sin estado no hace nada. Los "handlers" que
    /// procesan varios mensajes cambiando de estado entre mensajes deben sobreescribir este método para volver al
    /// estado inicial. </summary>
    public virtual void Cancel()
    {
        this.InternalCancel();
        if (this.Next != null)
        {
            this.Next.Cancel();
        }
    }
}
