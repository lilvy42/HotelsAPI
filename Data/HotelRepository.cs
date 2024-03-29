public class HotelRepository : IHotelRepository
{
    private readonly HotelDbContext _context;

    public HotelRepository(HotelDbContext context) {
        _context = context;
    }

    public Task<List<Hotel>> GetHotelsAsync() => _context.Hotels.ToListAsync();

    public Task<List<Hotel>> GetHotelsAsync(string name) => 
        _context.Hotels.Where(h => h.Name.Contains(name)).ToListAsync();
    public async Task<Hotel> GetHotelAsync(int hotelId) => 
        (await _context.Hotels.FindAsync(new object[] {hotelId}) ?? new Hotel());
    
    public async Task InsertHotelAsync(Hotel hotel) => await _context.Hotels.AddAsync(hotel);

    public async Task UpdateHotelAsync(Hotel hotel)
    {
        var hotelFromDb = await _context.Hotels.FindAsync(new object[] {hotel.Id});
        if (hotelFromDb == null) return;
        hotelFromDb.Latitude = hotel.Latitude;
        hotelFromDb.Longitude = hotel.Longitude;
        hotelFromDb.Name = hotel.Name;
    }

    public async Task DeleteHotelAsync(int hotelId)
    {
        var hotelFromDb = await _context.Hotels.FindAsync(new object[] {hotelId});
        if (hotelFromDb == null) return;
        _context.Hotels.Remove(hotelFromDb);
    }

    public async Task SaveAsync() => await _context.SaveChangesAsync();

    private bool _disposed = false;
    protected virtual void Dispose(bool disposing) {
        if (!_disposed) {
            if (disposing) {
                _context.Dispose();
            }
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}