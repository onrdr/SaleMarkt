﻿using Business.Services.Abstract;
using Business.Services.Concrete;
using DataAccess;
using Models.Identity;
using DataAccess.Repositories.Abstract;
using DataAccess.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Identity; 

namespace WebUI.ExtensionMethods;

public static class ServiceCollectionExtensionMethods
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        services
            .AddApplicationServices()
            .ConfigureDatabase(configurationManager)
            .AddIdentity()
            .AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }

    static IServiceCollection ConfigureDatabase(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configurationManager.GetConnectionString("DefaultConnection"));
        });

        return services;
    }

    static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();

        services.AddScoped<IEmailService, EmailService>();
        services.AddTransient<IViewRenderService, ViewRenderService>();


        return services;
    }

    static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, AppRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcçdefgğhıijklmnoöpqrsştuüvwxyzABCÇDEFGĞHIİJKLMNOÖPQRSŞTUÜVWXYZ0123456789-._";

            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireDigit = true;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
