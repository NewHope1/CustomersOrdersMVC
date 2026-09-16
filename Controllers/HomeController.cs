using System.Collections;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using CustomersOrdersMVC.Data;
using CustomersOrdersMVC.Entities;
using CustomersOrdersMVC.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomersOrdersMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CustomersOrdersContext _database;

        public HomeController(ILogger<HomeController> logger, CustomersOrdersContext database)
        {
            _logger = logger;
            _database = database;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Customers Orders";
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Edit(string lookupBtn, string submitBtn, IFormCollection fc, CustomerViewModel viewModel)
        {
            ModelState.Clear(); // ModelState is prefered over ViewModel, so we must clear
                                // ModelState or customer values from database entity won't show on the UI
                                
            if (ModelState.IsValid)
            {
                var customer = _database.Customers.Include(t => t.Orders).Where(t => t.CustomerID == viewModel.CustomerID).FirstOrDefault(); // load customer including its orders

                if (!String.IsNullOrEmpty(lookupBtn)) // lookup button press, just return customer found
                {
                    if (customer != null)
                    {
                        var customerViewModel = new CustomerViewModel
                        {
                            CustomerID = customer.CustomerID,
                            ContactName = customer.ContactName,
                            ContactTitle = customer.ContactTitle,                          
                            Address = customer.Address
                        };

                        return View(customerViewModel);
                    }
                }
                else if (!String.IsNullOrEmpty(submitBtn)) // Page submit (save) press
                {
                    // Save all view fields

                    customer.ContactName = viewModel.ContactName;
                    customer.ContactTitle = viewModel.ContactTitle;
                    customer.Address = viewModel.Address;

                    // Now let's save customer orders...

                    var addedRows = fc["CreatedRowsData"];
                    var updatedRows = fc["UpdatedRowsData"];
                    var deletedRows = fc["DeletedRowsData"];

                    List<Order> addedOrders = JsonSerializer.Deserialize<List<Order>>(addedRows);
                    List<Order> updatedOrders = JsonSerializer.Deserialize<List<Order>>(updatedRows);
                    List<Order> deletedOrders = JsonSerializer.Deserialize<List<Order>>(deletedRows);

                    // process addedRows
                    if (addedOrders != null)
                    {
                        addedOrders.ForEach(o =>
                        {
                                var newOrder = new Order()
                                {
                                    Customer = customer,
                                    CustomerID = customer.CustomerID,
                                    OrderDate = o.OrderDate,
                                    Freight = o.Freight,
                                    ShipCity = o.ShipCity,
                                    ShipCountry = o.ShipCountry,
                                };

                                customer.Orders.Add(newOrder);
                        });
                    }

                    // process updatedRows
                    if (updatedOrders != null)
                    {
                        updatedOrders.ForEach(u =>
                        {
                            if (customer.Orders.ToList().Exists(c => c.OrderID == u.OrderID))
                            {
                                var existingOrder = customer.Orders.AsQueryable().FirstOrDefault(f => f.OrderID == u.OrderID);

                                existingOrder.OrderDate = u.OrderDate;
                                existingOrder.Freight = u.Freight;
                                existingOrder.ShipCity = u.ShipCity;
                                existingOrder.ShipCountry = u.ShipCountry;
                            }
                        });
                    }

                    // process deletedRows
                    if (deletedOrders != null)
                    {
                        deletedOrders.ForEach(d =>
                        {
                            if (customer.Orders.ToList().Exists(c => c.OrderID == d.OrderID))
                            {
                                var existingOrder = customer.Orders.AsQueryable().FirstOrDefault(f => f.OrderID == d.OrderID);
                                _database.Entry(existingOrder).State = EntityState.Deleted;
                            }
                        });
                    }

                    _database.SaveChanges();
                }
            }

            return View(viewModel);
        }

  
        public IActionResult Privacy()
        {
            return View();
        }

        // Read handlers. Fired when grid needs to fetch rows  
        //public ActionResult Read_Orders([DataSourceRequest] DataSourceRequest request, string customerId)
        public ActionResult Read_Orders([DataSourceRequest] DataSourceRequest request, CustomerViewModel customerViewModel)
        {
            if (!string.IsNullOrEmpty(customerViewModel.CustomerID))
            {
                var orders = _database.Orders.Where(o => o.CustomerID == customerViewModel.CustomerID).ToList();

                var ordersViewModel = new List<OrderViewModel>(
                  orders.Select(o => new OrderViewModel
                  {
                      CustomerID = o.CustomerID,
                      OrderID = o.OrderID,
                      OrderDate = o.OrderDate, 
                      Freight = o.Freight,
                      ShipCity = o.ShipCity,
                      ShipCountry = o.ShipCountry
                  }
                  )
              );

                return Json(ordersViewModel.ToDataSourceResult(request));

            }

            return Json(ModelState);
        }
    }
}
