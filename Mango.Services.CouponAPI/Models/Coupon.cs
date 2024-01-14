namespace Mango.Services.CouponAPI.Models
{
    public class Coupon
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public double DiscountAmount { get; set; }
        public int MinAmount { get; set; }
        //podriamos tener mas campos que no necesriamente se comuniquen a traves de la API, solo pasamos los datos utiles en el DTO
    }
}
