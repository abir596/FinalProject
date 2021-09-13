using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Routing;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Extensions.Configuration;

namespace MessageSender.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly SocialMediaContext _context;
        
        public AccountsController(SocialMediaContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }
        
        // GET: Accounts
        [Authorize]
        public async Task<IActionResult> Home()
        {
            ViewBag.Posts = GetPostsHome();
            ViewBag.Images = GetImagesHome();
            ViewBag.Videos = GetVideosHome();
            return View();
        }
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 1048576000)]
        public async Task<IActionResult> UploadVideo(int id, [Bind("Id,Title,FileName,VideoFile,Date,AccountID")] VideoFiles videoFiles)
        {
            if (ModelState.IsValid)
            {
                id = _context.Account.Where(x => x.Email == User.FindFirstValue("Email")).FirstOrDefault().Id;
                videoFiles.Date = DateTime.Now;
                videoFiles.AccountID = id;
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(videoFiles.VideoFile.FileName);
                string extension = Path.GetExtension(videoFiles.VideoFile.FileName);
                videoFiles.FileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Video/", videoFiles.FileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await videoFiles.VideoFile.CopyToAsync(fileStream);
                }

                _context.Add(videoFiles);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(videoFiles);
        }
       
        [Authorize]
        public async Task<IActionResult> Index(string option, string search)
        {
            ViewBag.Videos = GetVideos();
            ViewBag.Posts = GetPosts();
            ViewBag.Images = GetImages();
            if (option == "FirstName")
            {
                return View(_context.Account.Where(x => x.FirstName == search || search == null).ToList());
            }
            else if (option == "LastName")
            {
                return View(_context.Account.Where(x => x.LastName == search || search == null).ToList());
            }
            else if (option == "Email")
            {
                return View(_context.Account.Where(x => x.Email == search || search == null).ToList());
            }
            else if (option == null && search != null)
            {
                return View(_context.Account.Where(x => x.FirstName.StartsWith(search)).ToList());
            }
            else
            {
                return View(_context.Account.Where(x => x.Email == User.FindFirstValue("Email")).ToList());
            }
        }

        public async Task<IActionResult> ImageDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imageModel = await _context.Images
                .FirstOrDefaultAsync(m => m.ImageId == id);
            if (imageModel == null)
            {
                return NotFound();
            }

            return View(imageModel);
        }
        public async Task<IActionResult> VideoDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videoFiles = await _context.VideoFiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (videoFiles == null)
            {
                return NotFound();
            }

            return View(videoFiles);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateImage(int id, [Bind("ImageId,Title,ImageName,ImageFile,Date,AccountID")] ImageModel imageModel)
        {
            if (ModelState.IsValid)
            {
                id = _context.Account.Where(x => x.Email == User.FindFirstValue("Email")).FirstOrDefault().Id;
                imageModel.Date = DateTime.Now;
                imageModel.AccountID = id;
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(imageModel.ImageFile.FileName);
                string extension = Path.GetExtension(imageModel.ImageFile.FileName);
                imageModel.ImageName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Image/", imageModel.ImageName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await imageModel.ImageFile.CopyToAsync(fileStream);
                }

                _context.Add(imageModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(imageModel);
        }
        public async Task<IActionResult> EditImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imageModel = await _context.Images.FindAsync(id);
            if (imageModel == null)
            {
                return NotFound();
            }
            return View(imageModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImage(int id, [Bind("ImageId,Title,ImageName,AccountID,ImageFile,Date")] ImageModel imageModel)
        {
            if (id != imageModel.ImageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(imageModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageModelExists(imageModel.ImageId))
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
            return View(imageModel);
        }
        public async Task<IActionResult> EditVideo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videoFiles = await _context.VideoFiles.FindAsync(id);
            if (videoFiles == null)
            {
                return NotFound();
            }
            return View(videoFiles);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVideo(int id, [Bind("Id,Title,FileName,AccountID,VideoFile,Date")] VideoFiles videoFiles)
        {
            if (id != videoFiles.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(videoFiles);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideoFileExists(videoFiles.Id))
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
            return View(videoFiles);
        }
        public List<ImageModel> GetImages()
        {
            var id = _context.Account.Where(x => x.Email == User.FindFirstValue("Email")).FirstOrDefault().Id;
            var images = _context.Images.Where(x => x.AccountID == id).ToList();
            return images;
        }
        public List<Post> GetPosts()
        {          
            var id = _context.Account.Where(x => x.Email == User.FindFirstValue("Email")).FirstOrDefault().Id;
            var posts = _context.Post.Where(x => x.AccountID == id).ToList();
            return posts;
        }
        public List<VideoFiles> GetVideos()
        {
            var id = _context.Account.Where(x => x.Email == User.FindFirstValue("Email")).FirstOrDefault().Id;
            var videos = _context.VideoFiles.Where(x => x.AccountID == id).ToList();
            return videos;
        }
        public List<ImageModel> GetImagesHome()
        {
            var images = _context.Images.Where(x => x.AccountID != null).ToList();
            return images;
        }
        public List<Post> GetPostsHome()
        {
            var posts = _context.Post.Where(x => x.AccountID != null).ToList();
            return posts;
        }
        public List<VideoFiles> GetVideosHome()
        {
            var videos = _context.VideoFiles.Where(x => x.AccountID != null).ToList();
            return videos;
        }
        // GET: Accounts/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.Posts = _context.Post.Where(x => x.AccountID == id).ToList();
            ViewBag.Images = _context.Images.Where(x => x.AccountID == id).ToList();
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }
        public async Task<IActionResult> PersonalInformation(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .FirstOrDefaultAsync(m => m.Id == id);
            if(account == null)
            {
                return NotFound();
            }
            return View(account);
        }
        [Authorize]
        public async Task<IActionResult> PostDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.post_id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }
        public IActionResult ChatApp(string option, string search, int id)
        {
            return View();
        }


        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,DOB,Gender,Email,ConfirmEmail,Country,CreatePassword,ConfirmPassword")] Account account)
        {
            if (ModelState.IsValid)
            {
                var isEmailAlreadyExists = _context.Account.Any(x => x.Email == account.Email);
                if (isEmailAlreadyExists)
                {
                    ModelState.AddModelError("Email", "User with this email already exists");
                    return View();
                }
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_context.Account.Any(x => x.Email == login.Email))
                {
                    if (_context.Account.Any(x => x.Email == login.Email && x.CreatePassword == login.Password))
                    {
                        var claims = new List<Claim>();
                        claims.Add(new Claim("Email", login.Email));
                        claims.Add(new Claim(ClaimTypes.Email, login.Email));
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            var ObjName = _context.Account.Where(x => x.Email == login.Email).FirstOrDefault();
                            var FirstName = ObjName.FirstName;
                            var LastName = ObjName.LastName;
                            TempData["FullName"] = "Welcome " + FirstName + " " + LastName + "!";
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Accounts");
                        }
                    }
                    ModelState.AddModelError("Password", "Your Password is incorrect");

                    return View(login);
                }
                ModelState.AddModelError("Email", "Email Address Not Found");

                return View(login);
            }
            return View(login);
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        // GET: Accounts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }
        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,DOB,Gender,Email,ConfirmEmail,Country,CreatePassword,ConfirmPassword")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
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
            return View(account);
        }
        [Authorize]
        [HttpPost("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WriteSomething(int id, [Bind("post_id", "post_txt", "post_date", "post_like", "AccountID")] Post post)
        {
            if (ModelState.IsValid)
            {
                id = _context.Account.Where(x => x.Email == User.FindFirstValue("Email")).FirstOrDefault().Id;
                post.AccountID = id;
                post.post_like = 0;
                post.post_date = DateTime.Now;               
                _context.Post.Add(post);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }
        public async Task<IActionResult> Like(int id)
        {
            Post update = await _context.Post.FindAsync(id);
            update.post_like += 1;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> LikeAccount(int id, int pid)
        {
            Post update = await _context.Post.FindAsync(pid);
            update.post_like += 1;
            await _context.SaveChangesAsync();            
            var routeValue = new RouteValueDictionary {{"id",id}};
            return RedirectToAction("Details", routeValue);
        }
        [Authorize]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, [Bind("post_id,post_txt,post_date,post_like,AccountID")] Post post)
        {                      
            if (id != post.post_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.post_id))
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
            return View(post);
        }

        // GET: Accounts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Account.FindAsync(id);         
            _context.Account.Remove(account);
     
            await _context.SaveChangesAsync();
            return RedirectToAction("Logout");
        }
        [Authorize]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.post_id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }
        [Authorize]
        [HttpPost, ActionName("DeletePost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePostConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool AccountExists(int id)
        {
            return _context.Account.Any(e => e.Id == id);
        }
        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.post_id == id);
        }
        private bool ImageModelExists(int id)
        {
            return _context.Images.Any(e => e.ImageId == id);
        }
        private bool VideoFileExists(int id)
        {
            return _context.VideoFiles.Any(e => e.Id == id);
        }
        public async Task<IActionResult> DeleteImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imageModel = await _context.Images
                .FirstOrDefaultAsync(m => m.ImageId == id);
            if (imageModel == null)
            {
                return NotFound();
            }

            return View(imageModel);
        }

        // POST: Image/Delete/5
        [HttpPost, ActionName("DeleteImage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImageConfirmed(int id)
        {
            var imageModel = await _context.Images.FindAsync(id);

            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Image", imageModel.ImageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
            _context.Images.Remove(imageModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeleteVideo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videoFiles = await _context.VideoFiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (videoFiles == null)
            {
                return NotFound();
            }

            return View(videoFiles);
        }

        // POST: Image/Delete/5
        [HttpPost, ActionName("DeleteVideo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVideoConfirmed(int id)
        {
            var videoFiles = await _context.VideoFiles.FindAsync(id);

            var videoPath = Path.Combine(_hostEnvironment.WebRootPath, "Video", videoFiles.FileName);
            if (System.IO.File.Exists(videoPath))
                System.IO.File.Delete(videoPath);
            _context.VideoFiles.Remove(videoFiles);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
