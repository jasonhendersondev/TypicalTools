using TypicalTechTools.Models.Data;

namespace TypicalTechTools.Models.Repositories
{
    public class ProductRepository : IProductRepository
    {

        //Reference to our context class which will handle all our DB operations.
        private readonly TypistTechToolsDBContext _context;

        // Ctrl + . (full stop) to build context class automatically
        // Constructor which requests the product context form the dependency injection by specifying it
        //as an input parameter then passes it to our reference field.
        public ProductRepository(TypistTechToolsDBContext context)
        {
            _context = context;
        }

        public void CreateProduct(Product product)
        {

                //Pass the product to the context class to be added to the DBset
                _context.Products.Add(product);
                //Save all DBset changes to the database.
                _context.SaveChanges();
            
        }

        public void DeleteProduct(int id)
        {
            var product = _context.Products.Where(p => p.Id ==id).FirstOrDefault();
            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products.Where(p => p.Id == id).FirstOrDefault();
        }

        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }
    }
}
