# Part 2 - Create Azure REST API that connect with Cosmos DB
Now that we have connected to the database directly with a Xamarin.Forms application in Part 1, let's discuss creating a REST API layer that can handle more serious stuff. This is a very common architecture for mobile applications and using Azure with Cosmos is a real option for us.

In this exercise, we will be creating a new Azure REST API that will connect with our Cosmos DB we created previously. So we will continue to use the **DatabaseId** as *Xamarin* and the **CollectionId** as *Dog*. 

## Start

Open the start project or just create a new Project.

If creating the project from New, Select the **ASP.NET Web Application(.NET Framework)** template and select **Azure API App** in the next screen.

### Step 1
First we are going to create a very simple DocumentDb GET request for our API to interact with our Cosmos DB. We will use Postman to verify this is working. If you want to skip ahead to the **Advanced** section, we will create an abstraction that allows us to pass any object to the same API enpoint for basic CRUD operations. It's pretty fantastic.

#### Creating a simple GET endpoint
1a) The first thing we want to do is connect our Cosmos DB to our Azure API App. Normally we connect to SQL Databases through the connection string, but that's not how Cosmos works. We still use that URI and PRIMARY KEY, but now we can put them in our API so it is hidden. The `ConfigurationManager.AppSettings` is where we should be putting keys like this.

Open up the `Web.config` file and add the following keys to the `appSettings`:
```xml
<add key="endpoint" value="{URI (even the numers at the end)}" />
<add key="authKey" value="{PRIMARY KEY}" />
```

Now we can use these values later in our code.

1b) Let's add our `Dog` model to the project in the **Models** folder

```csharp
using Newtonsoft.Json;

public class Dog
{
    [JsonProperty("id")]
    public string Id {get;set;}
    [JsonProperty("name")]
    public string Name {get;set;}
    [JsonProperty("furColor")]
    public string FurColor {get;set;}
}
```

1c) Next we need to install the **Microsoft.Azure.DocumentDb** package.

1c) Now we can use the existing files in the new project, so let's open up the `ValuesController.cs` file located in the **Controllers** folder.

1d) Now modify the `Get` method to interact with our Cosmos DB just like we did in **Part 1**. Here is where we will also use those values we placed in our API Apps config file

```csharp   
    public static async Task<Dog> Get(string id)
    {
        var  documentClient = new DocumentClient(
            new Uri(ConfigurationManager.AppSettings["endpoint"]), 
            ConfigurationManager.AppSettings["authKey"], 
            new ConnectionPolicy { EnableEndpointDiscovery = false }
        );
        var docLink = UriFactory.CreateDocumentUri("Xamarin", "Dog", id);
        var result = await documentClient.ReadDocumentAsync<Dog>(docLink);

        if (result.StatusCode != System.Net.HttpStatusCode.Created)
            return null;

        return result;
    }
```

1d) Next we want to publish our service in the Azure portal. Make sure the project builds, then right click the project and select "Publish".

1e) Run through the prompt to create resources if needed. The resources needed and created through the prompt is an App Service and App Service Plan. You will be able to view them in the portal and they will be grouped if you select the existing Resource Group you created the Cosmos DB in Part 1 of this hack. 

1f) After your resource is published, it should pop up in a Edge or the browser you select. Copy the Url and open PostMan. If you don't have it, [Get it](https://www.getpostman.com/apps).

1g) We will do a simple GET request and paste in the URL into Postman. We will need to add on the api endpoint:

```
api/Values?id=1
```

Remember the object we had in our database from Part 1 was this json blob:
```json
{
    "id": "1",
    "name": "olive",
    "furColor": "brown/black"
}
```
We want to get this object from our database, but if you gave your object another ID. You can open up the **Data Explorer** under the Azure Portal Cosmos DB instance.

1h) After verifying this endpoint is working, you have successfully conntect your Cosmos DB to an Azure API! This is definitely something that is used in Mobile and really cool. We get all the benefits of Cosmos DB too! 

### Extra Observations From Exercise
1) Notice that we created a API app, but had a `Web.config` file in the project? That's becuase almost all Azure services build on top of each other. The templates just setup the structure of the application for what "Service" was chosen to give whatever benfits that tech provides.
2) Notice that we have the `UriFactory.CreateDocument()` in our `Get` method? We use the `CollectionId` directly to connect to the document. If we generalize the types in the `CollectionId` and what we return, we can have one set of API end points that work for any object existing in our database. We will create this in the next section that is a little more advanced.

## Step 2 - Create a layer that handles it all
First to create this layer, we need to create some static methods that perform our basic CRUD operations. If you have looked at the getting started sample from Microsoft, they create a static `DocumentDbRespository` class. We will be pulling from that code.

2a) We are going to have all of our CRUD operations be based off of the `Id` poperty for our objects and the `CollectionId` for what table to access. We will just want a `DatabaseId` to be **Xamarin**.

```csharp
public static class DocumentDbRepository
{
    private static readonly string DatabaseId = "Xamarin";
    private static DocumentClient client new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]), ConfigurationManager.AppSettings["authKey"], new ConnectionPolicy { EnableEndpointDiscovery = false });

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
```

If you don't understand everything going on in the code, let talk about it a bit more. If you do, go ahead and skip ahead.

#### Code Explanation

Let's start with the `Initialize` method. Here we are initializing our `DocumentClient`. We are accessing the **Keys** we added back in step 1a to our `Web.config` file. This means we are connecting to our Cosmos DB thourgh a Url and a key that gives us access. Since it lives in our Azure API, we don't have to worry about a coder getting it from our Mobile App or really anywhere else. It's tucked where it should be. 

Comparing this API layer to connecting the Xamarin App directly to the Cosmos DB in Part one, we wouldn't be able to secure the Cosmos DB. So technically anyone could interact with the DB if they had three things:
1) Url to DB
2) Primary or Secondary Key from Azure Portal
3) Find the right `DatabaseId` and `CollectionId` combination

Azure REST API layer will allow us to use Active Directory for authentication. Then we can use other types of tools much easier.

Now let's turn our attention to the `GetItemAsync` method. Notice we utilize the `T` ("type of T") `type` generic keyword. This means that we will need to provide a `type` reference when we use the `GetItemAsync` method. for example if we called the following code:
```csharp
var item = await DocumentDbRepository.GetItemAsync<string>("1");
```
then `item` is going to be of `type` `string`. 

This allows us to provide the type to our method calls and expect the same return type. Notice the `dynamic` in the return object of the `GetItemAsync` method. This is a C# keyword that is part of the dynamic library and is commonly used in C# development. If you havn't seen it before or develop soley in Xamarin, dynamic C# can't be run on AOT compiled assemblies like iOS. 

But they can run in our Azure cloud stuff. There is a great Pluralsight course on Dynamic C# if you can figure out access. But the basic gist of it is the `dynamic` keyword will cause the compiler to ignore any compile errors from that object. You are telling the compiler *trust me computer, I know when everything runs it will work*. This means you won't see errors when you build or deploy your project, the errors will happen at runtime. 

**The completed solution to this point is located in the "Part 2/CosmosHackApi-SimpleGet-Complete"**

### Step 3 - JObject was made for Json
Remember how we setup our first `Collection` in our Cosmos DB? It was just a Json blob. `Json.Net` library his pretty much a standard for Json in C#/.Net projects. Inside the library there is a class called `JObject`. This object is basically a nice implementation that can take a string of Json and organize everything so you can easily access the json properties. But the best part is we can use this for our Cosmos DB in our Layer that makes things work!

3a) Let's open our `ValuesController` again and implement this crazy idea. We will user our `DocumentDbRepository` to implement the methods with the `JObject` type. 

```csharp
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
```
Notice we are using `JObject` for the parameters taken in from the `HTTP` request body. We also have included the minimum for error handling in your API. Remember that one unhandled exception will bring your API down, so going forward we will be adding error handling to our template.

3b) Now build and publish the application again.

3c) Let's jump over to our Azure Portal and create a new collection called **People** and document with the following json:
```json
{
    "id": "1",
    "name": "{Your Name}",
    "hairColor": "{Your Hair Color}"
}
```

If you don't remember how to do this, go back to Part 1 - step 2b.

3d) Next open up PostMan and let's test some things out live. change your url endpoint to be the following:
```
api/Values?collectionId=Dog&id=1
```
You should have got a response that looked the same as before where we see Olive.

3e) Now change the endpoint again, but let's change our collectionId
```
api/Values?collectionId=People&id=1
```
You should now get a response with your name and hair color. We implemented a solution with one set of API endpoints that will work for any data type. Test it out some more if you want. As long as the **CollectionId** exists in the Cosmos DB, you will get the correcct response. Super cool and powerful, but what about doing advanced queries? We can create one more abstraction that will make this easy to architect. If you continue to Step 4, we will create a `BaseApiController` that will let us implement this for a specific data type where we will get the basic CRUD operations easily and have the ability to do data sorting operations or whatever we want.

**The completed solution to this point is located in the "Part 2/CosmosHackApi-GenericValuesEndpoint-Complete"**

### Step 4 - Creating a BaseApiController to allow specific `type` implementations
This section will complete a basic architecture and framework that is a great solution for some of our customers. It also demonstrates some basics of Azure for them to get started with.

4a) Let's create a `BaseApiController` class in our **Controllers** folder. I like to start from the emply class, but if you right click the **Controller** folder and selected **Add->Controller** and wonder what template to use. Go ahead and use the Empyt API controller.

4b)  Inside of our controller, we want it to inherit from `ApiController`.

```csharp
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
```

If you skipped past the simple GET seciont, then add your `Dog` model class in our **DataObjects** folder.
```csharp
using Newtonsoft.Json;

public class Dog
{
    [JsonAttribute("id")]
    public string Id {get;set;}
    [JsonAttribute("name")]
    public string Name {get;set;}
    [JsonAttribute("furColor")]
    public string FurColor {get;set;}
}
```

4d) Next let's create a `XamarinDogsController` in our **Controllers** folder. Go ahead and add a blank class or blank controller. We will just have it subclass from our `BaseApiController` with a type of `Dog`.
```csharp
public class XamarinDogsController<Dog> : BaseApiController
{
}
```

Now this will automatically give us 4 of the following endpoints
```
GET api/XamarinDogs?id={ID to Get}
POST api/XamarinDogs Body with ID and object to create
PUT api/XamarinDogs Body with ID and parts of object to update
Delete api/XamarinDogs?id={ID to delete}
```

4e) Go ahead and publish the application one more time and test it out with Postman. You should be able to hit those endpoint and manipulate your data. Also notice how this architecture is stripping out the additional Cosmos DB properties and only returning the object we have defined in out **Models** folder. 