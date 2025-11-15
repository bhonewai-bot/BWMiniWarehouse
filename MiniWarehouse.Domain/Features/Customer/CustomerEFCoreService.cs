using Microsoft.EntityFrameworkCore;
using MiniWarehouse.Database.AppDbContextModels;
using MiniWarehouse.Domain.Models;
using MiniWarehouse.Shared.Constants;

namespace MiniWarehouse.Domain.Features.Customer;

public class CustomerEFCoreService
{
    private readonly AppDbContext _db;

    public CustomerEFCoreService()
    {
        _db = new AppDbContext();
    }

    public List<CustomerModel> ViewCustomers()
    {
        return _db.TblCustomers
            .AsNoTracking()
            .Select(x => new CustomerModel()
            {
                CustomerId = x.CustomerId,
                CustomerName = x.CustomerName,
            })
            .ToList();
    }

    public void AddCustomer(string customerName)
    {
        var customer = new TblCustomer()
        {
            CustomerName = customerName
        };
        
        _db.TblCustomers.Add(customer);
        int result = _db.SaveChanges();

        string message = result > 0 ? Message.Customer.AddedSuccessfully : Message.Customer.AddFailed;
        Console.WriteLine(message);
    }
}