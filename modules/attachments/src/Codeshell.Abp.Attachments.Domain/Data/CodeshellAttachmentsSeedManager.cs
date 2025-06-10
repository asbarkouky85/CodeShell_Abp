using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Codeshell.Abp.Attachments.Data
{
    public class CodeshellAttachmentsSeedManager : DomainService, ICodeshellAttachmentsSeedManager
    {
        private readonly IRepository<AttachmentCategory, int> categories;
        Dictionary<int, object[]> _allowUploads = new Dictionary<int, object[]>();
        Dictionary<int, object[]> _preventDownload = new Dictionary<int, object[]>();
        private List<AttachmentCategory> _addList = new List<AttachmentCategory>();

        public CodeshellAttachmentsSeedManager(IAttachmentCategoryRepository categories)
        {
            this.categories = categories;
        }

        public AttachmentCategory AddCategoryByEnum<T>(
            T categoryId,
            string allowedExtensions,
            int maxSize,
            string folderPath = null,
            int? maxCount = null,
            string container = null,
            Dimension dimension = null,
            bool anonymousDownload = false) where T : struct
        {
            var name = Enum.GetName(typeof(T), categoryId);
            var id = (int)Enum.ToObject(typeof(T), categoryId);
            folderPath = Utils.CamelCaseToWords(folderPath ?? name, "_").ToLower();
            var cat = new AttachmentCategory(id, name, allowedExtensions, maxSize, maxCount, folderPath, dimension, container, anonymousDownload);
            _addList.Add(cat);
            return cat;
        }

        public void SetUploadPermissionByEnum<T>(T category, params object[] roles)
        {
            _allowUploads[(int)Enum.ToObject(typeof(T), category)] = roles;
        }

        public void PreventDownloadPermissionByEnum<T>(T category, params object[] roles) where T : struct
        {
            _preventDownload[(int)Enum.ToObject(typeof(T), category)] = roles;
        }

        public async Task SaveAsync()
        {
            var allCats = await categories.GetListAsync(true);
            var dbCats = new List<AttachmentCategory>();

            foreach (var cat in _addList)
            {
                var exs = allCats.FirstOrDefault(e => e.Id == cat.Id);
                if (exs == null)
                {
                    exs = await categories.InsertAsync(cat);
                }
                else
                {
                    exs.UpdateRestrictions(cat.ValidExtensions, cat.MaxSize, cat.MaxDimension, cat.MaxCount);
                    exs.SetDisplay(cat.NameAr, cat.NameEn);
                    exs.SetAnonymousDownload(cat.AnonymousDownload);

                }
                dbCats.Add(exs);
            }
            SetUpPermissions(dbCats);
        }

        private void SetUpPermissions(IEnumerable<AttachmentCategory> cats)
        {
            foreach (var p in cats)
            {

                if (_allowUploads.TryGetValue(p.Id, out object[] uploaders))
                {
                    p.AllowUpload(uploaders);
                }

                if (_preventDownload.TryGetValue(p.Id, out object[] downloaders))
                {
                    p.PreventDownload(downloaders);
                }
            }
        }
    }
}
