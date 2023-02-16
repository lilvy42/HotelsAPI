public class HotelDbContext : DbContext {
    public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) {}

    public DbSet<Hotel> Hotels => Set<Hotel>();
}