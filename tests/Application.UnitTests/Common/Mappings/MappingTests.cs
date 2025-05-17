using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using AutoMapper;
using InvoiceManagementAPI.Application.Common.Mappings;
using InvoiceManagementAPI.Application.Customers.Queries.GetCustomersWithPagination;
using InvoiceManagementAPI.Application.Invoices.Queries.GetInvoiceDetail;
using InvoiceManagementAPI.Application.Invoices.Queries.GetInvoicesWithPagination;
using InvoiceManagementAPI.Application.Products.Queries.GetProductsWithPagination;
using InvoiceManagementAPI.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace InvoiceManagementAPI.Application.UnitTests.Common.Mappings;

public class MappingTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public MappingTests()
    {
        _configuration = new MapperConfiguration(config =>
            config.AddMaps(Assembly.GetAssembly(typeof(Application.Common.Mappings.MappingExtensions))));

        _mapper = _configuration.CreateMapper();
    }

    [Test]
    public void ShouldHaveValidConfiguration()
    {
        _configuration.AssertConfigurationIsValid();
    }

    [Test]
    [TestCase(typeof(Customer), typeof(CustomerDto))]
    [TestCase(typeof(Product), typeof(ProductDto))]
    [TestCase(typeof(Invoice), typeof(InvoiceDto))]
    [TestCase(typeof(Invoice), typeof(InvoiceDetailDto))]
    [TestCase(typeof(InvoiceItem), typeof(InvoiceItemDto))]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    {
        var instance = GetInstanceOf(source);

        _mapper.Map(instance, source, destination);
    }

    private object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type)!;

        // Type without parameterless constructor
        // Use RuntimeHelpers instead of obsolete FormatterServices
        return RuntimeHelpers.GetUninitializedObject(type);
    }
}
