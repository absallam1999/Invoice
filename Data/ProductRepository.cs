using invoice.Controllers;
using invoice.Models;

namespace invoice.Data
{
    public class ProductRepository :Repository<Product>,IProductRepository
    {
        public ProductRepository(ApplicationDbContext _context) :base(_context)
        {
            
        }


    }
}
