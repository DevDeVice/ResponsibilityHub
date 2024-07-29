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
    IAsyncEnumerable<T> GetEnumerable<T>() where T : class;
}
public class StorageRepository : IRepository
{
    private readonly string connString;
    private readonly string container;
    private readonly BlobServiceClient client;

    public StorageRepository() { }

    public StorageRepository(string connString, string container)
    {
        connString = connString;
        container = container;
        client = new BlobServiceClient(connString);
    }

    public async Task<T> Get<T>(Guid id) where T : class
    {
        var containerClient = client.GetBlobContainerClient(container);

        var blobClient = containerClient.GetBlobClient($"{id}.json");
        BlobDownloadResult result = await blobClient.DownloadContentAsync();
        var content = result.Content.ToString();

        return JsonSerializer.Deserialize<T>(content);
    }

    public IAsyncEnumerable<T> GetEnumerable<T>() where T : class
    {
        throw new NotImplementedException();
    }

    public async Task Save<T>(T input) where T : Person
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var containerClient = client.GetBlobContainerClient(container);
        var content = JsonSerializer.Serialize(input, options);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await containerClient.UploadBlobAsync($"{input.Id}.json", stream);

    }
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