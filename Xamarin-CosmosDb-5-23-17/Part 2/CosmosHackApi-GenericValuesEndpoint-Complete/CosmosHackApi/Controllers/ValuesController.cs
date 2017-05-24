using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using System.Threading.Tasks;
using CosmosHackApi.Models;
using Microsoft.Azure.Documents.Client;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Documents;

namespace CosmosHackApi.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values?collectionId={ClassType}&id={DocumentID}
        public async Task<JObject> Get(string collectionId, string id)
        {
            try
            {
                var items = await DocumentDbRepository.GetItemAsync<JObject>(collectionId, id);

                return items;
            }
            catch (DocumentClientException e)
            {
                Console.WriteLine("Unable to find item in DB");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unknown Error {e.Message}");
            }

            return null;
        }

        // POST api/values?collectionId={ClassType}
        public async Task<Document> Post(string collectionId, [FromBody]JObject item)
        {
            try
            {
                var created = await DocumentDbRepository.CreateItemAsync<JObject>(collectionId, item);

                return created;
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

        // PUT api/values/5
        public async Task<Document> Put(string collectionId, [FromBody]JObject item)
        {
            try
            {
                var itemId = item["id"].ToString();

                var update = await DocumentDbRepository.UpdateItemAsync<JObject>(collectionId, itemId, item);

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

        // DELETE api/values/5
        public async Task Delete(string collectionId, string id)
        {
            try
            {
                await DocumentDbRepository.DeleteItemAsync(collectionId, id);
            }
            catch (DocumentClientException E)
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
