using MiniOrderApp.Shared;

namespace MiniOrderApp.Import.Services;

public interface IImportService
{
        Task<Result<int>> ImportProducts(Stream file);
        Task<Result<int>> ImportCustomers(Stream file);
}