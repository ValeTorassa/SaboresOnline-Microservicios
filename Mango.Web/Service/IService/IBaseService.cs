using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IBaseService
    {
        Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true);
    }

    /*
      El Task representa una operación asincrónica, y el signo de pregunta (?) indica que el resultado puede ser nulo.
      se utiliza Task para encapsular la lógica que se ejecutará de forma asíncrona.
     */
}
