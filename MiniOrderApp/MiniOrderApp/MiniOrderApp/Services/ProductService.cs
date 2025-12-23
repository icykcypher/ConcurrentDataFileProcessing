using MiniOrderApp.Models;
using MiniOrderApp.Shared;
using MiniOrderApp.Services.Interfaces;
using MiniOrderApp.Repositories.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MiniOrderApp.Services;

public class ProductService(IProductRepository repo) : IProductService
{
        public async Task<IEnumerable<Product>> GetAll() => await repo.GetAll();

        public async Task<Result<Product>> GetById(int id)
        {
                var product = await repo.GetById(id);
                if (product.Id == 0 && string.IsNullOrWhiteSpace(product.Name) && product.Price == 0 &&
                    product.Category is default(ProductCategory))
                        return Result<Product>.Failure($"Product with id {id} not found", ErrorStatus.NotFound);

                return Result<Product>.Success(product);
        }

        public async Task<Result<Product>> Add(Product product)
        {
                var errors = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(product, new ValidationContext(product), errors, true);

                if (!isValid)
                        return Result<Product>.ValidationFailure(errors);

                var exists = await repo.Exists(product);
                if (exists)
                        return Result<Product>.Failure("Product already exists", ErrorStatus.Conflict);

                var added = await repo.Add(product);
                return Result<Product>.Success(added);
        }

        public async Task<Result<Product>> Update(Product product)
        {
                var existing = await repo.GetById(product.Id);
                if (IsEmptyProduct(existing))
                        return Result<Product>.Failure($"Product with id {product.Id} not found", ErrorStatus.NotFound);

                var errors = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(product, new ValidationContext(product), errors, true);
                if (!isValid)
                        return Result<Product>.ValidationFailure(errors);

                var allProducts = await repo.GetAll();
                var duplicate = allProducts.Any(p =>
                        p.Id != product.Id &&
                        string.Equals(p.Name, product.Name, StringComparison.OrdinalIgnoreCase) &&
                        p.Price.CompareTo(product.Price) == 0 &&
                        p.Category == product.Category
                );

                if (duplicate)
                        return Result<Product>.Failure("Another product with the same data already exists",
                                ErrorStatus.Conflict);

                await repo.Update(product);
                return Result<Product>.Success(product);
        }

        public async Task<Result<bool>> Delete(int id)
        {
                var product = await repo.GetById(id);
                if (IsEmptyProduct(product))
                        return Result<bool>.Failure($"Product with id {id} not found", ErrorStatus.NotFound);

                await repo.Delete(id);
                return Result<bool>.Success(true);
        }

        private bool IsEmptyProduct(Product p) =>
                p.Id == 0 &&
                string.IsNullOrWhiteSpace(p.Name) &&
                p.Price == 0 &&
                p.Category == default(ProductCategory);
}