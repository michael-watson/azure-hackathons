using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CosmosHackApi
{
    public static class DocumentDbRepository
    {
        private static readonly string DatabaseId = "Xamarin";
        private static DocumentClient client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]), ConfigurationManager.AppSettings["authKey"], new ConnectionPolicy { EnableEndpointDiscovery = false });

        public static async Task<T> GetItemAsync<T>(string collectionId, string id)
        {
            Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, collectionId, id));
            return (T)(dynamic)document;
        }

        public static async Task<Document> CreateItemAsync<T>(string collectionId, T item)
        {
            return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionId), item);
        }

        public static async Task<Document> UpdateItemAsync<T>(string collectionId, string id, T item)
        {
            return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, collectionId, id), item);
        }

        public static async Task DeleteItemAsync(string collectionId, string id)
        {
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, collectionId, id));
        }
    }
}