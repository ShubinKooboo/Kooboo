//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Api;
using Kooboo.Lib.Utilities;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class MediaApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "media";
            }
        }

        public bool RequireSite { get { return true; } }

        public bool RequireUser { get { return true; } }

        [Kooboo.Attributes.RequireParameters("path", "name")]
        public ImageFolderViewModel CreateFolder(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            string name = call.GetValue("name");
            string path = call.GetValue("path");
            if (!path.EndsWith("/"))
            {
                path = path + "/";
            }

            string fullpath = Lib.Helper.UrlHelper.Combine(path, name);
            if (!fullpath.StartsWith("/"))
            {
                fullpath = "/" + fullpath;
            }
            if (fullpath.EndsWith("/"))
            {
                fullpath = fullpath.TrimEnd('/');
            }

            Folder folder = new Folder(ConstObjectType.Image) {FullPath = fullpath};

            sitedb.Folders.AddOrUpdate(folder);

            return new ImageFolderViewModel
            {
                Id = folder.FullPath,
                Name = folder.Segment,
                FullPath = folder.FullPath,
                LastModified = PathService.GetLastModified(sitedb, folder.FullPath, ConstObjectType.Image),
                Count = sitedb.Folders.GetFolderObjects<Image>(folder.FullPath, true, false).Count +
                sitedb.Folders.GetSubFolders(folder.FullPath, ConstObjectType.Image).Count
            };
        }

        public MediaLibraryViewModel List(ApiCall call)
        {
            string path = call.GetValue("path", "fullpath");
            if (string.IsNullOrEmpty(path) || path == "\\")
            {
                path = "/";
            }

            MediaLibraryViewModel model = new MediaLibraryViewModel
            {
                Folders = GetFolders(call.WebSite.SiteDb(), path),
                Files = GetFiles(call.WebSite.SiteDb(), path),
                CrumbPath = PathService.GetCrumbPath(path)
            };




            return model;
        }

        public MediaLibraryViewModel ListBy(ApiCall call)
        {
            string by = call.GetValue("by");
            if (string.IsNullOrEmpty(by)) { return null; }

            var lower = by.ToLower();
            if (lower == "page" || lower == "view" || lower == "layout" || lower == "textcontent" || lower == "style" || lower == "htmlblock")
            {
                MediaLibraryViewModel model = new MediaLibraryViewModel
                {
                    Files = GetFilesBy(call.WebSite.SiteDb(), @by), CrumbPath = PathService.GetCrumbPath("/")
                };



                return model;
            }
            else
            {
                return null;
            }
        }

        public MediaPagedViewModel PagedListBy(ApiCall call)
        {
            string by = call.GetValue("by");
            if (string.IsNullOrEmpty(by)) { return null; }

            var lower = by.ToLower();
            if (lower == "page" || lower == "view" || lower == "layout" || lower == "textcontent" || lower == "style" || lower == "htmlblock")
            {
                int pagesize = ApiHelper.GetPageSize(call, 50);
                int pagenr = ApiHelper.GetPageNr(call);

                MediaPagedViewModel model = new MediaPagedViewModel
                {
                    Files = GetPagedFilesBy(call.WebSite.SiteDb(), @by, pagesize, pagenr),
                    CrumbPath = PathService.GetCrumbPath("/")
                };



                return model;
            }
            else
            {
                return null;
            }
        }

        public MediaPagedViewModel PagedList(ApiCall call)
        {
            string path = call.GetValue("path", "fullpath");
            if (string.IsNullOrEmpty(path) || path == "\\")
            {
                path = "/";
            }

            int pagesize = ApiHelper.GetPageSize(call, 50);
            int pagenr = ApiHelper.GetPageNr(call);

            MediaPagedViewModel model = new MediaPagedViewModel
            {
                Folders = GetFolders(call.WebSite.SiteDb(), path),
                Files = GetPagedFiles(call.WebSite.SiteDb(), path, pagesize, pagenr),
                CrumbPath = PathService.GetCrumbPath(path)
            };




            return model;
        }

        private List<ImageFolderViewModel> GetFolders(SiteDb siteDb, string path)
        {
            var subFolders = siteDb.Folders.GetSubFolders(path, ConstObjectType.Image);

            List<ImageFolderViewModel> result = new List<ImageFolderViewModel>();

            foreach (var item in subFolders)
            {
                ImageFolderViewModel model = new ImageFolderViewModel
                {
                    Name = item.Segment,
                    FullPath = item.FullPath,
                    Count = siteDb.Folders.GetFolderObjects<Image>(item.FullPath, true, false).Count +
                            siteDb.Folders.GetSubFolders(item.FullPath, ConstObjectType.Image).Count,
                    LastModified = PathService.GetLastModified(siteDb, item.FullPath, ConstObjectType.Image)
                };
                // model.Id = path;

                result.Add(model);
            }

            return result;
        }

        private List<MediaFileViewModel> GetFiles(SiteDb siteDb, string path)
        {
            string baseurl = siteDb.WebSite.BaseUrl();

            List<Image> images = siteDb.Folders.GetFolderObjects<Image>(path, true);

            List<MediaFileViewModel> result = new List<MediaFileViewModel>();

            foreach (var item in images)
            {
                MediaFileViewModel model = new MediaFileViewModel
                {
                    Id = item.Id,
                    Height = item.Height,
                    Width = item.Width,
                    Size = item.Size,
                    Name = item.Name,
                    LastModified = item.LastModified,
                    Thumbnail = ThumbnailService.GenerateThumbnailUrl(item.Id, 90, 0, siteDb.Id),
                    Url = ObjectService.GetObjectRelativeUrl(siteDb, item),
                    IsImage = true
                };
                model.PreviewUrl = Lib.Helper.UrlHelper.Combine(baseurl, model.Url);

                var usedby = siteDb.Images.GetUsedBy(item.Id);

                if (usedby != null)
                {
                    model.References = usedby.GroupBy(it => it.ConstType).ToDictionary(
                                      key =>
                                      {
                                          return ConstTypeService.GetModelType(key.Key).Name;
                                      }, value => value.Count());
                }

                result.Add(model);
            }
            return result;
        }

        // public PagedListViewModel<MediaFileViewModel> Files { get; set; }

        private PagedListViewModel<MediaFileViewModel> GetPagedFiles(SiteDb siteDb, string path, int pageSize, int pageNumber)
        {
            string baseurl = siteDb.WebSite.BaseUrl();

            List<Image> images = siteDb.Folders.GetFolderObjects<Image>(path, true);

            int totalskip = 0;
            if (pageNumber > 1)
            {
                totalskip = (pageNumber - 1) * pageSize;
            }

            PagedListViewModel<MediaFileViewModel> result = new PagedListViewModel<MediaFileViewModel>();

            result.TotalCount = images.Count();
            result.TotalPages = ApiHelper.GetPageCount(result.TotalCount, pageSize);
            result.PageSize = pageSize;
            result.PageNr = pageNumber;

            List<MediaFileViewModel> pagedlist = new List<MediaFileViewModel>();

            foreach (var item in images.Skip(totalskip).Take(pageSize))
            {
                MediaFileViewModel model = new MediaFileViewModel
                {
                    Id = item.Id,
                    Height = item.Height,
                    Width = item.Width,
                    Size = item.Size,
                    Name = item.Name,
                    LastModified = item.LastModified,
                    Thumbnail = ThumbnailService.GenerateThumbnailUrl(item.Id, 90, 0, siteDb.Id),
                    Url = ObjectService.GetObjectRelativeUrl(siteDb, item),
                    IsImage = true
                };
                model.PreviewUrl = Lib.Helper.UrlHelper.Combine(baseurl, model.Url);

                var usedby = siteDb.Images.GetUsedBy(item.Id);

                if (usedby != null)
                {
                    model.References = usedby.GroupBy(it => it.ConstType).ToDictionary(
                                      key =>
                                      {
                                          return ConstTypeService.GetModelType(key.Key).Name;
                                      }, value => value.Count());
                }

                pagedlist.Add(model);
            }
            result.List = pagedlist;
            return result;
        }

        private List<MediaFileViewModel> GetFilesBy(SiteDb siteDb, string by)
        {
            string baseurl = siteDb.WebSite.BaseUrl();
            // by = View, Page, Layout, TextContent, Style.
            byte consttype = ConstTypeContainer.GetConstType(by);

            var images = siteDb.Images.ListUsedBy(consttype);

            List<MediaFileViewModel> result = new List<MediaFileViewModel>();

            foreach (var item in images)
            {
                MediaFileViewModel model = new MediaFileViewModel
                {
                    Id = item.Id,
                    Height = item.Height,
                    Width = item.Width,
                    Size = item.Size,
                    Name = item.Name,
                    LastModified = item.LastModified,
                    Thumbnail = ThumbnailService.GenerateThumbnailUrl(item.Id, 90, 0, siteDb.Id),
                    Url = ObjectService.GetObjectRelativeUrl(siteDb, item),
                    IsImage = true
                };
                model.PreviewUrl = Lib.Helper.UrlHelper.Combine(baseurl, model.Url);

                var usedby = siteDb.Images.GetUsedBy(item.Id);

                if (usedby != null)
                {
                    model.References = usedby.GroupBy(it => it.ConstType).ToDictionary(
                                      key =>
                                      {
                                          return ConstTypeService.GetModelType(key.Key).Name;
                                      }, value => value.Count());
                }

                result.Add(model);
            }
            return result;
        }

        private PagedListViewModel<MediaFileViewModel> GetPagedFilesBy(SiteDb siteDb, string by, int PageSize, int PageNumber)
        {
            string baseurl = siteDb.WebSite.BaseUrl();
            // by = View, Page, Layout, TextContent, Style.
            byte consttype = ConstTypeContainer.GetConstType(by);

            var images = siteDb.Images.ListUsedBy(consttype);

            int totalskip = 0;
            if (PageNumber > 1)
            {
                totalskip = (PageNumber - 1) * PageSize;
            }

            PagedListViewModel<MediaFileViewModel> result = new PagedListViewModel<MediaFileViewModel>();

            result.TotalCount = images.Count();
            result.TotalPages = ApiHelper.GetPageCount(result.TotalCount, PageSize);
            result.PageSize = PageSize;
            result.PageNr = PageNumber;

            List<MediaFileViewModel> list = new List<MediaFileViewModel>();

            foreach (var item in images.Skip(totalskip).Take(PageSize))
            {
                MediaFileViewModel model = new MediaFileViewModel
                {
                    Id = item.Id,
                    Height = item.Height,
                    Width = item.Width,
                    Size = item.Size,
                    Name = item.Name,
                    LastModified = item.LastModified,
                    Thumbnail = ThumbnailService.GenerateThumbnailUrl(item.Id, 90, 0, siteDb.Id),
                    Url = ObjectService.GetObjectRelativeUrl(siteDb, item),
                    IsImage = true
                };
                model.PreviewUrl = Lib.Helper.UrlHelper.Combine(baseurl, model.Url);

                var usedby = siteDb.Images.GetUsedBy(item.Id);

                if (usedby != null)
                {
                    model.References = usedby.GroupBy(it => it.ConstType).ToDictionary(
                                      key =>
                                      {
                                          return ConstTypeService.GetModelType(key.Key).Name;
                                      }, value => value.Count());
                }

                list.Add(model);
            }

            result.List = list;
            return result;
        }

        public void DeleteFolders(ApiCall call)
        {
            string jsonvalue = call.Context.Request.Body;

            List<string> folderFullPaths = Lib.Helper.JsonHelper.Deserialize<List<string>>(jsonvalue);

            foreach (var item in folderFullPaths)
            {
                call.WebSite.SiteDb().Folders.Delete(item, ConstObjectType.Image, call.Context.User.Id);
            }
        }

        public void DeleteImages(ApiCall call)
        {
            string jsonvalue = call.Context.Request.Body;

            List<Guid> imageIds = Lib.Helper.JsonHelper.Deserialize<List<Guid>>(jsonvalue);

            foreach (var item in imageIds)
            {
                call.WebSite.SiteDb().Images.Delete(item, call.Context.User.Id);
            }
        }

        [Kooboo.Attributes.RequireParameters("id")]
        public ImageViewModel Get(ApiCall call)
        {
            var image = call.WebSite.SiteDb().Images.Get(call.ObjectId);

            if (image != null)
            {
                ImageViewModel model = new ImageViewModel
                {
                    Name = image.Name,
                    Url = Kooboo.Sites.Service.ObjectService.GetObjectRelativeUrl(call.WebSite.SiteDb(), image)
                };

                Dictionary<string, string> query = new Dictionary<string, string>
                {
                    {"SiteId", call.WebSite.Id.ToString()}
                };
                model.SiteUrl = Kooboo.Lib.Helper.UrlHelper.AppendQueryString(model.Url, query);
                model.Id = image.Id;
                model.Alt = image.Alt;
                model.FullUrl = Kooboo.Lib.Helper.UrlHelper.Combine(call.WebSite.BaseUrl(), model.Url);
                return model;
            }
            return null;
        }

        [Kooboo.Attributes.RequireParameters("id")]
        public void ImageUpdate(ApiCall call)
        {
            var image = call.WebSite.SiteDb().Images.Get(call.ObjectId);

            if (image != null)
            {
                string alt = call.GetValue("alt");
                string base64 = call.GetValue("base64");
                image.Alt = alt;

                if (!string.IsNullOrEmpty(base64))
                {
                    if (DataUriService.isDataUri(base64))
                    {
                        var datauri = DataUriService.PraseDataUri(base64);

                        if (datauri.isBase64)
                        {
                            image.ContentBytes = Convert.FromBase64String(datauri.DataString);
                            image.ResetSize();
                        }
                    }
                    else
                    {
                        image.ContentBytes = Convert.FromBase64String(base64);
                        image.ResetSize();
                    }
                }
                call.WebSite.SiteDb().Images.AddOrUpdate(image, call.Context.User.Id);
            }
        }

        public string ContentImage(ApiCall call)
        {
            var files = Kooboo.Lib.NETMultiplePart.FormReader.ReadFile(call.Context.Request.PostData);

            if (files == null || files.Count() == 0)
            {
                return null;
            }

            foreach (var f in files)
            {
                string filename = f.FileName;

                string url = GetUploadUrl(call, filename);

                var siteobject = call.WebSite.SiteDb().Images.UploadImage(f.Bytes, url, call.Context.User.Id);
                return url;
            }

            return null;
        }

        private string GetUploadUrl(ApiCall call, string filename)
        {
            var sitedb = call.WebSite.SiteDb();
            if (string.IsNullOrEmpty(filename))
            {
                return null;
            }
            string fullname = filename.Replace("\\", "/");
            if (fullname.StartsWith("/"))
            {
                fullname = fullname.Substring(1);
            }

            string checkurl = "/" + fullname;

            var image = sitedb.Images.GetByUrl(checkurl);
            if (image == null)
            {
                return checkurl;
            }

            for (int i = 0; i < 999; i++)
            {
                checkurl = "/" + i.ToString() + filename;
                image = sitedb.Images.GetByUrl(checkurl);
                if (image == null)
                {
                    return checkurl;
                }
            }

            return null;
        }
    }
}