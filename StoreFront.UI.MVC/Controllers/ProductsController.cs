using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreFront.DATA.EF.Models;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using StoreFront.UI.MVC.Utilities;
using X.PagedList;

namespace StoreFront.UI.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly StoreFrontContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductsController(StoreFrontContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        // GET: Products
        public async Task<IActionResult> Index(string searchTerm, int categoryId = 0, int page = 1)
        {

            int pageSize = 10;//10 Items per page

            //var storeFrontContext = _context.Products.Include(p => p.Category);
            var products = _context.Products.Where(p => !p.IsDiscontinued).Include(p => p.Category).ToList();

            #region Optional Category Filter

            //Create a ViewData Object to send a list of categories to the view
            //(This is similar to what you see in the Products/Create or Products/Edit actions)
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewBag.Category = 0;//Adding this variable to persist (save) the selected category

            if (categoryId != 0)
            {
                products = products.Where(p => p.CategoryId == categoryId).ToList();

                //Repopulate the dropdown with the current category selected
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryId);
                ViewBag.Category = categoryId;
            }

            #endregion


            #region Search Filter



            if (!string.IsNullOrEmpty(searchTerm))
            {

                products = products.Where(p => p.ProductDescription != null).ToList();

                products = products.Where(p =>
                p.ProductName.ToLower().Contains(searchTerm.ToLower()) ||
                p.Category.CategoryName.ToLower().Contains(searchTerm.ToLower()) ||
                p.ProductDescription.ToLower().Contains(searchTerm.ToLower())).ToList();

                ViewBag.NbrResults = products.Count;
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                ViewBag.NbrResults = null;
                ViewBag.SearchTerm = null;

            }


            #endregion


            //return View(await storeFrontContext.ToListAsync());
            return View(products.ToPagedList(page, pageSize));
        }
        public async Task<IActionResult> ListProducts()
        {

            var products = _context.Products.Where(p => !p.IsDiscontinued).Include(p => p.Category);//filter for products that are NOT discontinued
            return View(await products.ToListAsync());
        }
        [AllowAnonymous]
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.CurrId = id;
            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductDescription,IsDiscontinued,ProductCount,ProductPrice,CategoryId,ProductImage,UnitsOnOrder,Image")] Product product)
        {
            if (ModelState.IsValid)
            {

                #region FILE UPLOAD - CREATE

                //Check if a file was uploaded
                if (product.Image != null)
                {
                    //Check the file type
                    //- Store the extention of the img in a string
                    string ext = Path.GetExtension(product.Image.FileName);

                    //Create a list of valid extentions that will be used to check the ext against
                    string[] validExts = { ".jpeg", ".jpg", ".gif", ".png" };

                    //Verify the uploaded file has an extention matching one of the extensions in the list above
                    //We will also want to verify the file size will work with our .NET app
                    if (validExts.Contains(ext.ToLower()) && product.Image.Length < 4_194_303)//_ wont change the numeric value they just make big number ez to read
                    {
                        //Generate a unique file name
                        product.ProductImage = Guid.NewGuid() + ext;

                        //Save to the web server (here, save to wwwroot/images)
                        //To access the wwwroot, we added the _webHostEnvironment field to the controller (See the top)
                        //Retireve the path tp the wwwroot
                        string webRootPath = _webHostEnvironment.WebRootPath;


                        //Variable for the full image path (this is where we will save the image)
                        string fullImagePath = _webHostEnvironment.WebRootPath + "/img/";

                        //Create a MemoryStream to read the image into the server memory
                        using (var memoryStream = new MemoryStream())
                        {
                            //Transfer the file from the request to the server memory
                            await product.Image.CopyToAsync(memoryStream);

                            //Genberate the image using the info captured from the memorystream
                            //Add a using statement for System.Drawing to get access to the Image class
                            using (var img = Image.FromStream(memoryStream))
                            {
                                //Send the image to the ImageUtility for resizing and thumbnail creation
                                //Items need fir the ImageUtility.ResizeImage()

                                //- A string for the path where the file should be saved
                                //- A string for the name of the file
                                //- The Image itself
                                //- An int for the images maximum size (in pixels)
                                //- An int for the maximum thumbnail size
                                int maxImageSize = 500; //in pixels
                                int maxThumbSize = 100; //in pixels
                                ImageUtility.ResizeImage(fullImagePath, product.ProductImage, img, maxImageSize, maxThumbSize);

                                //myFile.Save("path/to/folder", "filename"); => how to save something that is not an image
                            }
                        }
                    }
                }
                else
                {
                    //No image was uploaded so we will assign a default file name
                    //And include a default image (With that FileName) in the images folder
                    product.ProductImage = "noimage.png";
                }
                #endregion


                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductDescription,IsDiscontinued,ProductCount,ProductPrice,CategoryId,ProductImage,UnitsOnOrder,Image")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                #region FILE UPLOAD - EDIT

                //Retain the old image file name so we can delete it if a new file was uploaded
                string oldImageName = product.ProductImage;
                if (product.Image != null)
                {
                    //Get the file's extention
                    string ext = Path.GetExtension(product.Image.FileName);

                    //Create a list of valid extentions
                    string[] validExts = { ".jpeg", ".jpg", ".gif", ".png", ".webp" };

                    //Check the files extention and the files size
                    if (validExts.Contains(ext.ToLower()) && product.Image.Length < 4_194_303)
                    {
                        //Generate a unique file name\
                        product.ProductImage = Guid.NewGuid() + ext;

                        //Build the file path to save the image
                        string webRootPath = _webHostEnvironment.WebRootPath;
                        string fullImagePath = webRootPath + "/img/";

                        //Delete the old image
                        if (oldImageName != "noimage.png")
                        {
                            ImageUtility.Delete(fullImagePath, oldImageName);
                        }

                        //Save the new image to the wwwroot
                        using (var memoryStream = new MemoryStream())
                        {
                            //Get the image from the MemoryStream
                            await product.Image.CopyToAsync(memoryStream);

                            //Copy the image from the stream Into an Image on the server
                            using (var img = Image.FromStream(memoryStream))
                            {
                                //Set up the max image and max thumbnail sizes in pixels
                                int maxImageSize = 500;
                                int maxThumbSize = 100;
                                //Resize and save the image
                                ImageUtility.ResizeImage(fullImagePath, product.ProductImage, img, maxImageSize, maxThumbSize);
                            }
                        }
                    }
                }

                #endregion

                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'StoreFrontContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
