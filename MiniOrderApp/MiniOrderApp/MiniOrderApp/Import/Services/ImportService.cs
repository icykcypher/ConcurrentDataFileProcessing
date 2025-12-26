using MiniOrderApp.Import.Dtos;
using MiniOrderApp.Import.Parsers;
using MiniOrderApp.Models;
using MiniOrderApp.Repositories.Interfaces;
using MiniOrderApp.Shared;

namespace MiniOrderApp.Import.Services;

public class ImportService(
        IProductRepository productRepo,
        ICustomerRepository customerRepo)
        : IImportService
{
        public async Task<Result<int>> ImportProducts(Stream file)
        {
                var parser = new CsvParser<ProductImportDto>(p => new ProductImportDto
                {
                        Name = p[0],
                        Price = float.Parse(p[1]),
                        Category = p[2]
                });

                int count = 0;

                foreach (var dto in parser.Parse(file))
                {
                        var product = new Product
                        {
                                Name = dto.Name,
                                Price = dto.Price,
                                Category = Enum.Parse<ProductCategory>(dto.Category, true)
                        };

                        if (!await productRepo.Exists(product))
                        {
                                await productRepo.Add(product);
                                count++;
                        }
                }

                return Result<int>.Success(count);
        }

        public async Task<Result<int>> ImportCustomers(Stream file)
        {
                var parser = new CsvParser<CustomerImportDto>(p => new CustomerImportDto
                {
                        Name = p[0],
                        Email = p[1]
                });

                int count = 0;

                foreach (var dto in parser.Parse(file))
                {
                        var customer = new Customer
                        {
                                Name = dto.Name,
                                Email = dto.Email
                        };

                        if (!await customerRepo.Exists(customer))
                        {
                                await customerRepo.Add(customer);
                                count++;
                        }
                }

                return Result<int>.Success(count);
        }
}