using AzureApp31;
using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using System.Collections.Generic;

public static class ElasticsearchExtensions
{    public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        var url = configuration["ElasticConfiguration:Uri"];
        var defaultIndex = configuration["ElasticConfiguration:IndexUser"];

        var settings = new ConnectionSettings(new Uri(url))
            .DefaultIndex(defaultIndex);

        AddDefaultMappings(settings);

        var client = new ElasticClient(settings);

        services.AddSingleton(client);

        CreateIndex(client, defaultIndex);
    }

    private static void AddDefaultMappings(ConnectionSettings settings)
    {
        settings.
            DefaultMappingFor<ApplicationUser>(m => m
                .Ignore(p => p.Phone)
            );
    }

    private static void CreateIndex(IElasticClient client, string indexName)
    {
        var createIndexResponse = client.Indices.Create(indexName,
            index => index.Map<ApplicationUser>(x => x.AutoMap())
        );
    }
}

public interface IElasticsearchHelper
{
    void SaveSingle(ApplicationUser user);
    void SaveMany(List<ApplicationUser> users);
    void Find(string query, int page = 1, int pageSize = 5);
}
public class ElasticsearchHelper : IElasticsearchHelper
{
    private readonly IElasticClient _elasticClient;

    public ElasticsearchHelper(
        IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public void SaveSingle(ApplicationUser user)
    {
        //_elasticClient.Update<ApplicationUser>(user, u => u.Doc(user));
        
        _elasticClient.IndexDocumentAsync(user);

        //ConnectionSettings.DefaultMappingFor<TDocument>() or set a default index using ConnectionSettings.DefaultIndex().'        
    }

    public void SaveMany(List<ApplicationUser> users)
    {
        var result = _elasticClient.IndexMany(users);

        if (result.Errors)
        {
            foreach (var itemWithError in result.ItemsWithErrors)
            {
                //Log
            }
        }
    }

    public void Find(string query, int page = 1, int pageSize = 5)
    {
        var response = _elasticClient.Search<ApplicationUser>
            (
                s => s.Index("users").Query(q => q.QueryString(d => d.Query(query)))
                    .From((page - 1) * pageSize)
                    .Size(pageSize)
            );
            

        if (!response.IsValid)
        {
            //Log
        }

        if (page > 1)
        {
            GetSearchUrl(query, page - 1, pageSize);
        }

        if (response.IsValid && response.Total > page * pageSize)
        {
            GetSearchUrl(query, page + 1, pageSize);
        }

        var docs = response.Documents;
    }

    private static string GetSearchUrl(string query, int page, int pageSize)
    {
        return $"/search?query={Uri.EscapeDataString(query ?? "")}&page={page}&pagesize={pageSize}/";
    }

    

    //public async Task SaveBulkAsync(Product[] products)
    //{
    //    _cache.AddRange(products);
    //    var result = await _elasticClient.BulkAsync(b => b.Index("products").IndexMany(products));
    //    if (result.Errors)
    //    {
    //        // the response can be inspected for errors
    //        foreach (var itemWithError in result.ItemsWithErrors)
    //        {
    //            _logger.LogError("Failed to index document {0}: {1}",
    //                itemWithError.Id, itemWithError.Error);
    //        }
    //    }
    //}
}