using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lab7.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			if (!Request.IsAuthenticated)
			{
				ViewBag.message = "Please register to get image operations access";
			}

			return View();
		}

		public ActionResult MyImages()
		{
			var myimageList = new List<string>();
			using (var db = new Models.ApplicationDbContext())
			{
				var user = db.Users.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
				myimageList = GetUserImages(user.GroupName);
				ViewBag.groupname = user.GroupName;
			}
			return View(myimageList);
		}

		public ActionResult UploadImage()
		{
			ViewBag.error = null;
			return View();
		}

		[HttpPost]
		public ActionResult UploadImage(string imagename, HttpPostedFileBase picture)
		{
			string error = null;
			if (string.IsNullOrEmpty(imagename))
			{
				error = "Image name is required";
				return View("UploadImage");
			}

			using (var db = new Models.ApplicationDbContext())
			{
				var user = db.Users.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
				var UserFolderName = string.Format("~/ImageFolders/{0}/{1}", user.GroupName, User.Identity.Name);
				if (picture != null && picture.ContentLength > 0)
				{

					var filename = Path.GetFileName(picture.FileName);
					string newfilename = imagename + ".png";
					var filePath1 = UserFolderName + "/" + newfilename;
					var s = Server.MapPath(filePath1);
					picture.SaveAs(s);
				}
				else
				{
					error = "Please choose a valid image";
					return View("UploadImage");
				}
			}

			return RedirectToAction("MyImages");
		}

		public List<string> GetUserImages(string groupname)
		{

			var myimageList = new List<string>();

			var UserFolderName = string.Format("~/ImageFolders/{0}/{1}", groupname, User.Identity.Name);
			if (!Directory.Exists(UserFolderName))
			{
				Directory.CreateDirectory(UserFolderName);
			}

			var userdirectory = new DirectoryInfo(Server.MapPath(UserFolderName));

			foreach (var file in userdirectory.GetFiles())
			{
				if (!myimageList.Contains(file.Name))
				{
					myimageList.Add(file.Name);
				}
			}
			return myimageList;
		}


		public ActionResult GetAllGropUserImages()
		{
			List<string> imageList = new List<string>();
			using (var db = new Models.ApplicationDbContext())
			{
				var user = db.Users.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
				var groupusernames = db.Users.Where(r => r.GroupName == user.GroupName).ToList();
				var GroupFolderName = string.Format("~/ImageFolders/{0}", user.GroupName);

				foreach (var groupUSer in groupusernames)
				{
					var userdirectory = new DirectoryInfo(Server.MapPath(GroupFolderName + "/" + groupUSer.Email));
					var w = userdirectory.GetFiles();
					var userDirectory = GroupFolderName + "/" + groupUSer.Email;
					foreach (var file in userdirectory.GetFiles())
					{
						if (!imageList.Contains(file.Name))
						{
							imageList.Add(userDirectory.Split('~').LastOrDefault() + "/" + file.Name);
						}
					}

				}
			}

			return View(imageList);

		}

		public ActionResult ViewUsers()
		{
			var db = new Models.ApplicationDbContext();
			var user = db.Users.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
			var groupusers = db.Users.Where(r => r.GroupName == user.GroupName).ToList();
			return View(groupusers);
		}

		public ActionResult DeleteUser(string email)
		{
			using (var db = new Models.ApplicationDbContext())
			{ 
			var user = db.Users.Where(r => r.Email == email).FirstOrDefault();
				db.Users.Remove(user);
				db.SaveChanges();
			}
			return RedirectToAction("ViewUsers");
		}

		[HttpPost]
		public ActionResult deletepicture(string picname) 
		{
			var picturelink = Server.MapPath(picname);
			var fileinfo = new FileInfo(picturelink);
			fileinfo.Delete();
			return RedirectToAction("MyImages");
		}
	}
}