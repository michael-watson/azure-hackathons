using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Data.SqlClient;
using System;

namespace AbuseFunction
{
    public static class NewProfileFunction
    {
        [FunctionName("NewProfile")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var str = "Server=tcp:databasehacksql.database.windows.net,1433;Initial Catalog=DatabaseHackSql;Persist Security Info=False;User ID=miwats;Password=Xamarin123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            using (SqlConnection connection = new SqlConnection(str))
            {
                await connection.OpenAsync();
                DbContext dbContext = new DbContext(connection);

                try
                {
                    var transation = connection.BeginTransaction();
                    dbContext.Transaction = transation;

                    UserProfile profile = new UserProfile
                    {
                        Id = Guid.NewGuid(),
                        Birthdate = DateTime.Now,
                        CompanyId = Guid.NewGuid(),
                        Email = "NewEmail@gmail.com",
                        EmailVerified = true,
                        FirstName = "Michael",
                        LastName = "Watson",
                        Gender = "male",
                        PhoneNumber = "650-892-7190",
                        PhoneType = "mobile",
                        PreferredName = "The Doctor",
                        TeacherId = Guid.NewGuid()
                    };

                    dbContext.UserProfiles.InsertOnSubmit(profile);
                    dbContext.SubmitChanges();
                    transation.Commit();

                    return req.CreateResponse<UserProfile>(profile);
                }
                catch (Exception e)
                {

                }
                finally
                {
                    connection.Close();
                }
            }

            return req.CreateResponse(HttpStatusCode.InternalServerError, "User not found and something happened");
        }
    }
}