namespace YourBrand.Catalog.Features.ProductManagement;

using System;


public class VariantAlreadyExistsException(string message) : Exception(message)
{
}