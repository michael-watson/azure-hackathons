using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Configuration;
using System;
using System.Data.SqlClient;
using Microsoft.Azure.Documents.Client;

namespace AbuseFunction
{
    public static class TakeItFunction
    {
        [FunctionName("TakeIt")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            //We will use the DocumentClient for Cosmos
            //var docClient = new DocumentClient(
            //    new Uri(ConfigurationManager.AppSettings["endpoint"]),
            //    ConfigurationManager.AppSettings["authKey"],
            //    new ConnectionPolicy { EnableEndpointDiscovery = false }
            //    );
            //var docLink = UriFactory.CreateDocumentUri("DatabaseId", "CollectionId", "IdOfDocument");
            //var result = await docClient.ReadDocumentAsync<UserProfile>(docLink);

            //if (result.StatusCode != HttpStatusCode.OK)
            //    return null;

            //return req.CreateResponse<UserProfile>(result);

            //We will use the ConnectionStrings for all Azure SQL and Azure PostgreSql
            //var str = ConfigurationManager.ConnectionStrings["sql_connection"].ConnectionString;
            var str="Server=tcp:databasehacksql.database.windows.net,1433;Initial Catalog=DatabaseHackSql;Persist Security Info=False;User ID=miwats;Password=Xamarin123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            using (SqlConnection centroConnection = new SqlConnection(str))
            {
                await centroConnection.OpenAsync();
                DbContext dbContext = new DbContext(centroConnection);

                try
                {
                    UserProfile profile = dbContext.UserProfiles.Where(p => p.Email == "michaelwatson93@gmail.com").FirstOrDefault();
                    return req.CreateResponse<UserProfile>(profile);
                }
                catch (Exception e)
                {

                }
            }

            return req.CreateResponse(HttpStatusCode.InternalServerError, "User not found and something happened");
        }
    }
}