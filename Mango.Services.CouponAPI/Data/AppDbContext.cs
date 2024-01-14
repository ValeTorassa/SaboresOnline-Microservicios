using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Models.Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.Coupon>().HasData(new Models.Coupon
            {
                CouponId = 1,
                CouponCode = "10PERCENTMANGO",
                DiscountAmount = 10,
                MinAmount = 100
            });


            modelBuilder.Entity<Models.Coupon>().HasData(new Models.Coupon
            {
                CouponId = 2,
                CouponCode = "20PERCENTMANGO",
                DiscountAmount = 20,
                MinAmount = 200
            });
        }
    }
}
