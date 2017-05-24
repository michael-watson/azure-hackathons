using System;
using System.Threading.Tasks;

using Microsoft.Azure.Documents.Client;

namespace CosmosHack
{
    public static class DocumentDbService
    {
        static readonly DocumentClient documentClient = new DocumentClient(new Uri("{COSMOS DB URL}"), "{COSMOS DB PRIMARY KEY}");
        static readonly string DatabaseId = "Xamarin";
        static readonly string CollectionId = "Dog";

        public static async Task<Dog> GetDogByIdAsync(string id)
        {
            var result = await documentClient.ReadDocumentAsync<Dog>(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            return result;
        }
    }
}