using System.Collections;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Azure.Core;
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

        // POST: Default/Edit/5
        [HttpPost]
        //public ViewResult Edit(string lookupBtn, string submitBtn, IFormCollection fc, string customerId)
        public ViewResult Edit(string lookupBtn, string submitBtn, IFormCollection fc, CustomerViewModel viewModel)
        {
            ModelState.Clear(); // ModelState is prefered over ViewModel, so we must clear
                                // ModelState or fetched customer values won't show on the UI
                                
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

                    //List<Order> addedOrders = JsonSerializer.Deserialize<List<Order>>(addedRows);
                    //List<Order> updatedOrders = JsonSerializer.Deserialize<List<Order>>(updatedRows);
                    //List<Order> deletedOrders = JsonSerializer.Deserialize<List<Order>>(deletedRows);

                    List<Order> addedOrders = getOrders(addedRows);
                    List<Order> updatedOrders = getOrders(updatedRows);
                    List<Order> deletedOrders = getOrders(deletedRows);

                    // process addedRows
                    if (addedOrders != null)
                    {
                        addedOrders.ForEach(o =>
                        {
                            //if (!customer.Orders.ToList().Exists(c => c.OrderID == o.OrderID))
                            //{
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
                            //}
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
                                // must delete order details first so we don't get a constraint violation when deleting the order
                                //var orderDetailsToDelete = _database.Order_Details.Where(t => d.OrderID == d.OrderID);
                                //_database.Order_Details.RemoveRange(orderDetailsToDelete);

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

        //ActionResult Edit(string lookupBtn, string submitBtn, FormCollection fc, CustomerViewModel viewModel, [Bind(Prefix = "models")] IEnumerable<OrderViewModel> orders)

        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Return a list of Order objects from a string record
        /// </summary>
        /// <param name="record">input string with columns delimeted by comma. For example:
        /// "[{\"OrderID\":10254,\"CustomerID\":null,\"EmployeeID\":null,\"OrderDate\":\"1996-07-11T04:00:00.000Z\",\"RequiredDate\":\"1996-08-08T04:00:00.000Z\",\"ShippedDate\":\"1996-07-23T04:00:00.000Z\",\"ShipVia\":2,\"Freight\":45,\"ShipName\":\"Chop-suey Chinese\",\"ShipAddress\":\"Hauptstr. 31\",\"ShipCity\":\"Bern\",\"ShipRegion\":null,\"ShipPostalCode\":\"3012\",\"ShipCountry\":\"Switzerland\",\"Customer\":null,\"Order_Details\":[]}]"
        /// This parameter comes from the client side
        /// </param>
        /// <returns>List of Order objects</returns>
        private List<Order> getOrders(string record)
        {
            if (record == "[]") // no new rows added to grid
            {
                return null;
            }

            List<Order> rVal = new List<Order>();

            Dictionary<string, string> columns = new Dictionary<string, string>();

            string tempRecord = record;
            ArrayList recs = new ArrayList();
            var matcher = new Regex(@"{(.*?)}"); // match all records in between { and }
            var matches = matcher.Matches(record).Cast<Match>().Select(m => m.Value).Distinct(); // cast to IEnumerable so we can operate on
            foreach (string match in matches)
            {
                tempRecord = match.Replace("{", String.Empty); // remove beginning {
                tempRecord = tempRecord.Replace("}", String.Empty); // remove ending }
                tempRecord = tempRecord.Replace("\"", String.Empty); // remove all double quotes
                recs.Add(tempRecord);
            };

            // loop through all records (rows) and convert each one into an entity
            foreach (String rec in recs)
            {
                var fields = rec.Split(',');

                // loop through the individual record fields
                foreach (var field in fields)
                {
                    var fieldName = field.Split(':')[0];

                    // Deal with OrderDate and RequiredDate fields case. E.g., OrderDate:1996-07-11T04:00:00.000Z
                    string fieldValue = ((fieldName == "OrderDate" || fieldName == "RequiredDate" || fieldName == "ShippedDate")
                                        && field.Split(':')[1].Length != 0 && field.Split(':')[1] != "null"
                                        ? (field.Split(':')[1] + ":" + field.Split(':')[2] + ":" + field.Split(':')[3]) : field.Split(':')[1]);

                    columns.Add(fieldName, fieldValue);
                }

                Order newObj = new Order();
                newObj.OrderID = columns["OrderID"] != "null" ? Convert.ToInt16(columns["OrderID"]) : 0;
                newObj.CustomerID = columns["CustomerID"] != "null" ? columns["CustomerID"] : String.Empty;
                newObj.Freight = Convert.ToDecimal(columns["Freight"]);
                newObj.ShipCity = columns["ShipCity"];
                newObj.ShipCountry = columns["ShipCountry"];

                DateTime orderDate;
                newObj.OrderDate = DateTime.TryParseExact(columns["OrderDate"], "yyyy-MM-ddTHH:mm:ss.fffZ", null, System.Globalization.DateTimeStyles.None, out orderDate) ? orderDate : DateTime.MinValue;

                rVal.Add(newObj);

                columns.Clear();
            }

            return rVal;
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
