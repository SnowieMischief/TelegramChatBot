using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Nito.AsyncEx;

namespace Library;

/// <summary> Un "handler" del patrón Chain of Responsibility que implementa el comando "categorias". </summary>
public class CategoriasHandler : BaseHandler {
    private TelegramBotClient bot;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="HelloHandler"/>. Esta clase procesa el mensaje "Categorias".
    /// </summary>
    /// <param name="next">El próximo "handler".</param>
    public CategoriasHandler(TelegramBotClient bot, BaseHandler next) : base(next) {
        this.Keywords = new string[] {"categorias"};
    }

    /// <summary> Procesa el mensaje "Categorias" y retorna true; retorna false en caso contrario. </summary>
    /// <param name="message">El mensaje a procesar.</param>
    /// <param name="response">La respuesta al mensaje procesado.</param>
    /// <returns>true si el mensaje fue procesado; false en caso contrario.</returns>
    protected override void InternalHandle(Message message, out string response) {
        response = "Categorias: --a-a-a--a-a-a-a-";
    }
}
