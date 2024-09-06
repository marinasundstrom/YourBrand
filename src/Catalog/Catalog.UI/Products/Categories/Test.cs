namespace YourBrand.Catalog.Products;

public static class Test
{
    public static ProductCategoryTreeNode? FindNode(this IEnumerable<ProductCategoryTreeNode> nodes, int categoryId)
    {
        ProductCategoryTreeNode? productCategoryTreeNode = null;

        foreach (var childNode in nodes)
        {
            productCategoryTreeNode = childNode.FindNode(categoryId);

            if (productCategoryTreeNode is not null)
                break;
        }

        return productCategoryTreeNode;
    }

    public static ProductCategoryTreeNode? FindNode(this IEnumerable<ProductCategoryTreeNode> nodes, string path)
    {
        ProductCategoryTreeNode? productCategoryTreeNode = null;

        foreach (var childNode in nodes)
        {
            productCategoryTreeNode = childNode.FindNode(path);

            if (productCategoryTreeNode is not null)
                break;
        }

        return productCategoryTreeNode;
    }

    public static ProductCategoryTreeNode? FindNode(this ProductCategoryTreeNode productCategoryTreeNode, int categoryId)
    {
        if (productCategoryTreeNode.Id == categoryId)
        {
            return productCategoryTreeNode;
        }

        foreach (var childNode in productCategoryTreeNode.SubCategories)
        {
            var r = FindNode(childNode, categoryId);

            if (r is not null)
                return r;
        }

        return null;
    }

    public static ProductCategoryTreeNode? FindNode(this ProductCategoryTreeNode productCategoryTreeNode, string path)
    {
        if (productCategoryTreeNode.Path.StartsWith(path))
        {
            return productCategoryTreeNode;
        }

        foreach (var childNode in productCategoryTreeNode.SubCategories)
        {
            var r = FindNode(childNode, path);

            if (r is not null)
                return r;
        }

        return null;
    }
}