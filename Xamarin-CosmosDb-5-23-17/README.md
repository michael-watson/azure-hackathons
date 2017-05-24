# Cosmos DB Hack
This is a two part hack that expands on the knowledge set of CosmosDB. Azure Cosmos DB provides the best capabilities of relational and non-relational databases. Cosmos DB is always on, independently scales storage and has throughput with latency gurantees of <10ms for reads and <15ms for writes at [P99(scroll down to 'Latency SLA')](https://azure.microsoft.com/en-us/support/legal/sla/cosmos-db/v1_0/). Cosmos is pretty cool and you should [read more](https://docs.microsoft.com/en-us/azure/cosmos-db/introduction) about it.

## Pre-requisites
You need to have Visual Studio 2017 with Azure Development installed. If you don't have the template for a hack or anything, re-run your installer and make sure to install Azure Development.

Below is an overview of each part of the hack.

## Part 1
This hack will take you through connecting a Xamarin.Forms mobile application to a CosmoDB (DocumentDb) instance directly. You will setup your environment locally with the emulator for Cosmos, but we will deploy an instance of DocumentDB based Cosmos DB. 

This offers options for developers that don't necessarily need a REST API layer, but want to interact with the best databse in the business. Definitely a good database option for developers that is simple and easy to use. It is also built to handle this type of interaction.

## Part 2
This hack will take you through creating an Azure REST Api layer that interacts with the Cosmos DB. We will work towards creating templatized controllers that provide various options. This is a very fun and practical exercise that demonstrates not only how to use Cosmos in a cool way, but also build C# skills and an understanding of how Azure works for a lot of the services we touch.