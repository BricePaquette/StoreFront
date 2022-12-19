using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StoreFront.DATA.EF.Models;
using StoreFront.UI.MVC.Models;
using Microsoft.AspNetCore.Identity;
namespace StoreFront.UI.MVC.Controllers
{
    public class ShoppingCartController : Controller
    {
        #region Steps to Impliment a session based shopping cart
        /*
         * 1) Register Session in program.cs (builder.Services.AddSession... && app.UseSession())
         * 2) Create the CartItemViewModel class in [ProjName].UI.MVC/Models folder
         * 3) Add the 'Add To Cart' button in the Index and/or Details view of your Products
         * 4) Create the ShoppingCartController (empty controller -> named ShoppingCartController)
         *      - add using statements
         *          - using GadgetStore.DATA.EF.Models;
         *          - using Microsoft.AspNetCore.Identity;
         *          - using GadgetStore.UI.MVC.Models;
         *          - using Newtonsoft.Json;
         *      - Add props for the GadgetStoreContext && UserManager
         *      - Create a constructor for the controller - assign values to context && usermanager
         *      - Code the AddToCart() action
         *      - Code the Index() action
         *      - Code the Index View
         *          - Start with the basic table structure
         *          - Show the items that are easily accessible (like the properties from the model)
         *          - Calculate/show the lineTotal
         *          - Add the RemoveFromCart <a>
         *      - Code the RemoveFromCart() action
         *          - verify the button for RemoveFromCart in the Index view is coded with the controller/action/id
         *      - Add UpdateCart <form> to the Index View
         *      - Code the UpdateCart() action
         *      - Add Submit Order button to Index View
         *      - Code SubmitOrder() action
         */
        #endregion
        private readonly StoreFrontContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ShoppingCartController(StoreFrontContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            //Retrieve the contents from the Session shopping cart (stored as JSON)
            //and convert them to c# via Newtonsoft.Json. After converting to C#, we
            //can pass the collection of cart contents back to the strongly-typed
            //view for display

            //Retrieve the cart contents
            var sessionCart = HttpContext.Session.GetString("cart");

            //Create a shell for the local (C# vesion) of the cart
            Dictionary<int, CartItemViewModel> shoppingCart = null;

            //Check to see if the sessionCart was null
            if (sessionCart == null || sessionCart.Count() == 0)
            {
                //User either hasn't put anything in the cart, or they have removed
                //all items from the cart. So, we'll set shoppingCart to an empty Dictionary'
                shoppingCart = new Dictionary<int, CartItemViewModel>();

                ViewBag.Message = "There are no items in your cart.";

            }
            else
            {
                ViewBag.Message = null;

                //Deserialize the cart contents from json to c#
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            }
            return View(shoppingCart);
        }
        public IActionResult AddToCart(int id)
        {
            //Empty shell for the local shopping cart variable
            //int (key) => ProductID
            //CartItemViewModel (value) => Product & Qty
            Dictionary<int, CartItemViewModel> shoppingCart = null; //Users shopping cart to be updated

            #region Session Notes
            /*
             * Session is a tool availiable on the server-side that can store information for a user
             * while theyre active on your site
             * 
             * Typically, the session lasts for 20 minutes (this can be adjusted in Program.cs)
             * Once the 20 minutes are up, the session variable is disposed
             * 
             * Values that we store in Session are limited to: string, int
             *   - Due to this restriction, we need to get creative when trying to store complex objects (CartItemViewModel)
             *   - To keep the information separated into properties, we will convert the c# object to a JSON string
             */
            #endregion

            var sessionCart = HttpContext.Session.GetString("cart");//Have they already added things to their cart

            //Check to see if the Session Object already exists
            //If not, instantiate the new local collection
            if (string.IsNullOrEmpty(sessionCart))
            {
                //If the session didn't exist yet, instantiate the collection as a new object
                shoppingCart = new Dictionary<int, CartItemViewModel>();
            }
            else
            {
                //Cart already exists - Transfer the existing cart items from the Session into our
                //local shoppingCart.
                //DeserializeObject() is a method that converts JSON to C# - we MUST tell this method which C# class to convert the JSON into
                //(here, that is our Dictionary<int, CartItemViewModel>
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            }

            //Add any newly selected product to the cart
            //Retrieve the product from the database
            Product product = _context.Products.Find(id);

            //Instantiate the CIVM so we can add to the cart
            CartItemViewModel civm = new CartItemViewModel(1, product);//Add one of the selected product to the cart

            //If the product was already in the cart, increase the quantity by 1.
            //Otherwise, add the new CIVM object to the cart
            if (shoppingCart.ContainsKey(product.ProductId))
            {
                shoppingCart[product.ProductId].Qty++;
            }
            else
            {
                shoppingCart.Add(product.ProductId, civm);
            }
            //Update the session version of the cart
            //Take the local copy and Serialize it as JSON
            //Then assign that value to our session
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);//Since we are going from C# to JSON we dont need to specify a type 

            HttpContext.Session.SetString("cart", jsonCart);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int id)
        {
            //Retrieve the cart from the session
            var sessionCart = HttpContext.Session.GetString("cart");

            //Convert from JSON to C#
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //Remove the Item
            shoppingCart.Remove(id);

            //Check to see if there are no remaining items in the cart, remove the cart from the session
            if (shoppingCart.Count() == 0)
            {
                HttpContext.Session.Remove("cart");
            }
            else
            {
                //Otherwise, update the session with the new cart contents
                string jsonCart = JsonConvert.SerializeObject(shoppingCart);
                HttpContext.Session.SetString("cart", jsonCart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult UpdateCart(int productId, int qty)
        {
            //Retrieve the cart
            var sessionCart = HttpContext.Session.GetString("cart");

            //Convert From JSON to C#
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //Update the qty for our specific cart item


            if (qty == 0)
            {
                shoppingCart.Remove(productId);
            }
            else
            {

                shoppingCart[productId].Qty = qty;
            }
            //Update the session 
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);
            HttpContext.Session.SetString("cart", jsonCart);

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> SubmitOrder()
        {
            #region Planning Out Order Submission
            /*
             * Big Picture Plan:
             * Create Order object -> Save to the DB
             * -OrderDate
             * -UserId
             * -ShipToName, ShipCity, ShipState, ShipZip -> This info needs to be pulled from the UserDetails Record
             * Add the record to the _context
             * Save DB Changes
             * 
             * Create OrderProducts object for each titem in the cart
             * -ProductId -> cart
             * -OrderId -> from Order object
             * -Qty -> Availiable from the Cart
             * -Price -> Cart
             * 
             * Add the record to the _context
             * Save the DB changes
             */
            #endregion

            //Retrieve the current user's ID
            //This is a mechanism provided by Identity to retrieve the UserID in the
            //current HttpContext. If you need to retrieve an ID in ANY controller you can use this

            string? userId = (await _userManager.GetUserAsync(HttpContext.User))?.Id;

            //Retrieve the userdetails record
            UserDetail ud = _context.UserDetails.Find(userId);

            //Create an Order object and assign the values
            Order o = new Order()
            {
                OrderDate = DateTime.Now,
                UserId = userId,
                ShipCity = ud.City,
                ShipToName = ud.UserFirstName + " " + ud.UserLastName,
                ShipPlanet = ud.PlanetName,
                ShipState = ud.State,
                ShipZip = ud.Zip
            };

            //Add the order object to the context
            _context.Orders.Add(o);

            //Retrieve the SessionCart
            var sessionCart = HttpContext.Session.GetString("cart");

            //Convert the cart to C#
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //Create an orderProduct object for each item in the cart
            foreach (var item in shoppingCart)
            {
                //Create and assign the orderProduct object
                OrderProduct op = new OrderProduct()
                {
                    OrderId = o.OrderId,
                    ProductId = item.Key,
                    
                    ProductPrice = item.Value.Product.ProductPrice,
                    Quantity = (short)item.Value.Qty
                };

                //Only need to add items to an existing entity if the items are a related record (like from a linking table)
                o.OrderProducts.Add(op);

            }
            //Commit the changes and sabe to the db
            _context.SaveChanges();

            return RedirectToAction("Index", "Orders");

        }
    }
}
