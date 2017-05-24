using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CosmosHackApi.Controllers
{
    public class BaseApiController<T> : ApiController
    {
        static readonly string collectionId = typeof(T).Name;

        public async Task<T> Get(string id)
        {
            try
            {
                var items = await DocumentDbRepository.GetItemAsync<T>(collectionId, id);

                return (T)(dynamic)items;
            }
            catch (DocumentClientException e)
            {
                Console.WriteLine("Unable to find item in DB");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown Error {e.Message}");
            }

            return default(T);
        }

        public async Task<Document> Post([FromBody]T item)
        {
            try
            {
                var create = await DocumentDbRepository.CreateItemAsync<T>(collectionId, item);

                return create;
            }
            catch (DocumentClientException e)
            {
                Console.WriteLine("Unable to create item, it already existed");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown Error {e.Message}");
            }

            return null;
        }

        public async Task<Document> Put([FromBody]T item)
        {
            try
            {
                var itemId = ((dynamic)item).Id.ToString();

                var update = await DocumentDbRepository.UpdateItemAsync<T>(collectionId, itemId, item);

                return update;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("Id property not found in JSON Payload");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown Error {e.Message}");
            }

            return null;
        }

        public async Task Delete(string id)
        {
            try
            {
                await DocumentDbRepository.DeleteItemAsync(collectionId, id);
            }
            catch (DocumentClientException e)
            {
                Console.WriteLine("Unable to delete file, it wasn't found in DB");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown Error {e.Message}");
            }
        }
    }
}