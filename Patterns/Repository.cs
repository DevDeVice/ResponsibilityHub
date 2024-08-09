using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ResponsibilityHub.Models;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ResponsibilityHub.Patterns;
public enum RepoType 
{ 
    Storage 
    //Cosmos 
}
public interface IRepositoryObject { }
public interface IRepository
{
    Task Save<T>(T o) where T : Person;
    Task<T> Get<T>(Guid id) where T : class;
    Task<List<T>> GetAll<T>() where T : class;
    IAsyncEnumerable<T> GetEnumerable<T>() where T : class;
    Task Delete<T>(Guid id) where T : class;
}
public class StorageRepository : IRepository
{
    private readonly string connString;
    private readonly string _container;
    private readonly BlobServiceClient _client;

    public StorageRepository() { }

    public StorageRepository(string connString, string container)
    {
        connString = connString;
        _container = container;
        _client = new BlobServiceClient(connString);
    }

    public async Task<T> Get<T>(Guid id) where T : class
    {
        var containerClient = _client.GetBlobContainerClient(_container);
        var blobClient = containerClient.GetBlobClient($"{id}.json");

        try
        {
            BlobDownloadResult result = await blobClient.DownloadContentAsync();
            var content = result.Content.ToString();

            return JsonSerializer.Deserialize<T>(content);
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            // Rzucanie specyficznego wyjątku jeśli blob nie istnieje
            throw new FileNotFoundException($"Blob with ID {id} was not found.", ex);
        }
    }
    public async Task<List<T>> GetAll<T>() where T : class
    {
        var results = new List<T>();

        try
        {
            await foreach (var item in GetEnumerable<T>())
            {
                results.Add(item);
            }
        }
        catch (Azure.RequestFailedException ex)
        {
            throw new Exception("An error occurred while fetching blobs.", ex);
        }
        catch (Exception ex)
        {
            // Obsługuje inne potencjalne błędy
            throw new Exception("An unexpected error occurred.", ex);
        }

        return results;
    }
    public async IAsyncEnumerable<T> GetEnumerable<T>() where T : class
    {
        var containerClient = _client.GetBlobContainerClient(_container);

        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            BlobDownloadResult result = await blobClient.DownloadContentAsync();
            var content = result.Content.ToString();

            var item = JsonSerializer.Deserialize<T>(content);
            if (item != null)
            {
                yield return item;
            }
        }
    }
    //Version 3 Save
    public async Task Save<T>(T input) where T : Person
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var content = JsonSerializer.Serialize(input, options);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var containerClient = _client.GetBlobContainerClient(_container);

        var blobName = $"{input.Id}.json";
        var blobClient = containerClient.GetBlobClient(blobName);
        var blobHttpHeaders = new BlobHttpHeaders
        {
            ContentType = "application/json"
        };

        try
        {
            await blobClient.UploadAsync(stream, blobHttpHeaders);
        }
        catch (RequestFailedException ex)
        {
            if (ex.ErrorCode == "ContainerNotFound")
            {
                await containerClient.CreateAsync();
                //albo stream.Position = 0;
                using var newStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                await blobClient.UploadAsync(newStream, blobHttpHeaders);
            }
            else
            {
                throw; // Rzuca wyjątek ponownie, jeśli nie jest to błąd związany z nieistniejącym kontenerem
            }
        }
    }
    //Version 2 Save
    /*var options = new JsonSerializerOptions
    {
        WriteIndented = true
    };
    var content = JsonSerializer.Serialize(input, options);
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
    var containerClient = _client.GetBlobContainerClient(_container);

    var blobName = $"{input.Id}.json";
    var blobClient = containerClient.GetBlobClient(blobName);

    try
    {
        await blobClient.UploadAsync(stream, new BlobHttpHeaders
        {
            ContentType = "application/json"
        });
    }
    catch (RequestFailedException ex)
    {
        if (ex.ErrorCode == "ContainerNotFound")
        {
            await containerClient.CreateAsync();
            using var newStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            await blobClient.UploadAsync(newStream, new BlobHttpHeaders
            {
                ContentType = "application/json"
            });
        }
    }*/

    //Version 1 Save
    /*var options = new JsonSerializerOptions
    {
        WriteIndented = true
    };
    var content = JsonSerializer.Serialize(input, options);
    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
    var containerClient = _client.GetBlobContainerClient(_container);

    try
    {
        await containerClient.UploadBlobAsync($"{input.Id}.json", stream);
    }
    catch (RequestFailedException ex)
    {
        if(ex.ErrorCode == "ContainerNotFound")
        {
            await containerClient.CreateAsync();
            using var newStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            await containerClient.UploadBlobAsync($"{input.Id}.json", newStream);
        }
    }*/
    /*public class CosmosRepository : IRepository
{
    public Task<T> Get<T>(Guid id) where T : class
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<T> GetEnumerable<T>() where T : class
    {
        throw new NotImplementedException();
    }

    public Task Save<T>(T o) where T : Person
    {
        throw new NotImplementedException();
    }
}*/


    public async Task Delete<T>(Guid id) where T : class
    {
        var containerClient = _client.GetBlobContainerClient(_container);
        var blobClient = containerClient.GetBlobClient($"{id}.json");

        try
        {
            await blobClient.DeleteAsync();
            /*var response = await blobClient.DeleteIfExistsAsync();
            if (!response)
            {
                throw new FileNotFoundException($"Blob with ID {id} not found.");
            }*/
        }
        catch (RequestFailedException ex)
        {
            throw new Exception($"Error deleting the blob: {ex.Message}", ex);
        }
    }
    //Version 2 DeleteAll
    public async Task DeleteAll<T>() where T : class
    {
        var containerClient = _client.GetBlobContainerClient(_container);

        try
        {
            var blobs = containerClient.GetBlobsAsync();

            // Usunięcie blobów równolegle, aby zmniejszyć czas przetwarzania
            await Parallel.ForEachAsync(blobs, async (blobItem, token) =>
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                try
                {
                    //await blobClient.DeleteIfExistsAsync(cancellationToken: token);
                    await blobClient.DeleteAsync(cancellationToken: token);
                }
                catch (RequestFailedException ex) when (ex.ErrorCode != "BlobNotFound")
                {
                    // Ignoruj wyjątek, jeśli blob nie został znaleziony, rzucaj inne wyjątki
                    throw new Exception($"Failed to delete blob: {blobItem.Name}", ex);
                }
            });
        }
        catch (RequestFailedException ex)
        {
            throw new Exception("An error occurred while deleting blobs.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred during deletion.", ex);
        }
    }
    //version 1 DeleteAll
    /*public async Task DeleteAll<T>() where T : class
    {
        var containerClient = _client.GetBlobContainerClient(_container);

        try
        {
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                await blobClient.DeleteIfExistsAsync();
            }
        }
        catch (RequestFailedException ex)
        {
            throw new Exception("An error occurred while deleting blobs.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred during deletion.", ex);
        }
    }*/
}