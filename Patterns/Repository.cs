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
        BlobDownloadResult result = await blobClient.DownloadContentAsync();
        var content = result.Content.ToString();

        return JsonSerializer.Deserialize<T>(content);
    }
    public async Task<List<T>> GetAll<T>() where T : class
    {
        var containerClient = _client.GetBlobContainerClient(_container);
        var results = new List<T>();

        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            BlobDownloadResult result = await blobClient.DownloadContentAsync();
            var content = result.Content.ToString();

            var item = JsonSerializer.Deserialize<T>(content);
            if (item != null)
            {
                results.Add(item);
            }
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

    public async Task Save<T>(T input) where T : Person
    {
        //Version 3
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
                using var newStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                await blobClient.UploadAsync(newStream, blobHttpHeaders);
            }
            else
            {
                throw; // Rzuca wyjątek ponownie, jeśli nie jest to błąd związany z nieistniejącym kontenerem
            }
        }
        //Version 2
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

        //Version 1
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
    }
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
            await blobClient.DeleteIfExistsAsync();
        }
        catch (RequestFailedException ex)
        {
            throw new Exception($"Error deleting the blob: {ex.Message}");
        }
    }
    public async Task DeleteAll<T>() where T : class
    {
        var containerClient = _client.GetBlobContainerClient(_container);

        // Iterujemy po wszystkich obiektach w kontenerze i usuwamy je
        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            await blobClient.DeleteIfExistsAsync();
        }
    }
}