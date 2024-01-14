namespace Mango.Web.Models
{
    /*
     * Por que creamos de nuevo responseDto?
     * lo creamos para poder usarlo en el front de mango web
     * y por que no hacemos un proyecto de libreria y lo usamos en los dos proyectos?
     * por que tenemos los dos proyectos en la misma solucion podriamos hacerlo pero no siempre ambos proyectos (API y CONSUMER)
     * estan en la misma solucion
     */
    public class ResponseDto
    {
        public object? Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "";
    }
}
