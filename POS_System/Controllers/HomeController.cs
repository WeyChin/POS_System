using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POS_System.DA.ProductDA;
using POS_System.DA.UserDA;
using POS_System.Models;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace POS_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserDAL _context;
        private readonly ProductDAL _Productcontext;
        private readonly StripeModels _stripeModels;

        public HomeController(ILogger<HomeController> logger, UserDAL context, ProductDAL ProductContext, IOptions<StripeModels> stripeModels)
        {
            _context = context;
            _Productcontext = ProductContext;
            _logger = logger;
            _stripeModels = stripeModels.Value;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserType") != null)
            {
                return View("HomePage");
            }
            else
            {
                return View();
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult ValidateUserLogin(string userName, string pass)
        {
            try
            {
                string userType;
                var user = _context.GetUserByUsernameAndPassword(userName, pass);
                // Do something with the user, e.g., redirect to another page
                if (user != null)
                {
                    HttpContext.Session.SetInt32("UserID", user.UserID);

                    if (user.UserGroupID == 3)
                    {
                        userType = "SuperAdmin";
                        HttpContext.Session.SetString("UserType", userType);
                    }
                    else if (user.UserGroupID == 4)
                    {
                        userType = "Admin";
                        HttpContext.Session.SetString("UserType", userType);
                    }
                    else
                    {
                        throw new Exception("Not Admin Now Allow To Access.");
                    }

                    ViewData["UserType"] = userType;
                    return View("~/Views/Home/ScanProductPage.cshtml");
                }
                else
                {
                    throw new Exception("UserName Or Password Is Incorrect.");
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message; // Set error message if needed
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProductDetails(string productSKU)
        {
            try
            {
                var product = _Productcontext.GetProductDetailsByProductSKU(productSKU);

                if (product == null)
                {
                    return NotFound(); // Return 404 if user is not found
                }

                return Json(new
                {
                    Name = product.Name,
                    SKU = product.SKU,
                    PricePerUnit = product.PricePerUnit
                });

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("~/Views/Home/ScanProductPage.cshtml");
            }
        }

        [HttpPost]
        public IActionResult CreatePaymentIntent([FromBody] PaymentIntentRequest request)
        {
            try
            {
                StripeConfiguration.ApiKey = _stripeModels.SecretKey;
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                    {
                        "card"
                    },

                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = request.Currency,
                                UnitAmount = Convert.ToInt32(request.Amount) * 100,
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name= "test",
                                    Description = "testing 123"
                                }
                            },
                            Quantity = 1
                        }
                    },
                    Mode = "payment",
                    SuccessUrl = "https://www.google.com",
                    CancelUrl = "https://hotayi.atlassian.net/issues/?filter=10050"

                };

                var service = new SessionService();
                var session = service.Create(options);


                var updateRequest = new ProductQuantityUpdateRequest
                {
                    Items = new List<ProductQuantityUpdateItem>
                    {
                        new ProductQuantityUpdateItem
                        {
                            SKU = "LAYSBBQ-0001",
                            Quantity = 1
                        },
                        new ProductQuantityUpdateItem
                        {
                            SKU = "DUCK-0001",
                            Quantity = 1
                        },


                    }
                };


                UpdateProductQuantities(updateRequest);

                return Redirect(session.Url);
                //return Json(new { clientSecret = paymentIntent.ClientSecret });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class PaymentIntentRequest
        {
            public decimal Amount { get; set; }
            public string Currency { get; set; }
        }

        [HttpPost]
        public IActionResult UpdateProductQuantities([FromBody] ProductQuantityUpdateRequest request)
        {
            using (var transaction = _Productcontext.Database.BeginTransaction())
            {
                try
                {
                    int userID = (int)HttpContext.Session.GetInt32("UserID");
                    if (userID == 0)
                    {
                        throw new Exception("Unable To Get UserID");
                    }

                    // Your code to update product quantities based on the received data
                    foreach (var item in request.Items)
                    {
                        var sku = item.SKU;
                        var quantity = item.Quantity;

                        _Productcontext.UpdateStockQtyByProductSKU(sku, quantity, userID);
                    }
                    transaction.Commit();

                    // Return success response if needed
                    return Ok(new { message = "Product quantities updated successfully." });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(500, new { error = ex.Message });
                }
            }
        }
    }
}
