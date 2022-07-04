namespace YourBrand.Orders.Hypermedia;

public static partial class Mapper
{
    public static string CreateEmbedQuery(string[] embed)
    {
        if (embed.Length == 0)
            return string.Empty;

        return $"&embed={string.Join("&embed=", embed)}";
    }

    public static TModel Append<TModel>(string urlBase, TModel model, int skip, int limit, string[] embed, bool hasMore)
        where TModel : Resource
    {
        model.Links.Add("self", new Link
        {
            Href = $"{urlBase}?skip={skip}&limit={limit}{CreateEmbedQuery(embed)}"
        });

        /*

        model.Links.Add("find", new Link
        {
            Href = "{urlBase}?page={?page}",
        });

        */

        if (skip != 0 && skip >= limit)
        {
            model.Links.Add("previous", new Link
            {
                Href = $"{urlBase}?skip={skip - limit}&limit={limit}{CreateEmbedQuery(embed)}"
            });
        }

        if (hasMore)
        {
            model.Links.Add("next", new Link
            {
                Href = $"{urlBase}?skip={skip + limit}&limit={limit}{CreateEmbedQuery(embed)}"
            });
        }

        return model;
    }
}