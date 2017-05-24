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

namespace CosmosHackApi.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [SwaggerOperation("GetAll")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<Dog> Get(string id)
        {
            var documentClient = new DocumentClient(
                new Uri(ConfigurationManager.AppSettings["endpoint"]),
                ConfigurationManager.AppSettings["authKey"],
                new ConnectionPolicy { EnableEndpointDiscovery = false }
            );
            var docLink = UriFactory.CreateDocumentUri("Xamarin", "Dog", id);

            var result = await documentClient.ReadDocumentAsync<Dog>(docLink);

            if (result.StatusCode != HttpStatusCode.OK)
                return null;

            return result;
        }

        // POST api/values
        [SwaggerOperation("Create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [SwaggerOperation("Update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [SwaggerOperation("Delete")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Delete(int id)
        {
        }
    }
}
