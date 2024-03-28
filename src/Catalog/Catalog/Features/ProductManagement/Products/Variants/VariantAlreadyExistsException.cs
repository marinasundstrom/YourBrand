namespace YourBrand.Catalog.Features.ProductManagement;

using System;


public class VariantAlreadyExistsException : Exception
{
    public VariantAlreadyExistsException(string message) : base(message) { }
}