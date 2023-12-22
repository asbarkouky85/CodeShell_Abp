using Codeshell.Abp.Attachments.Permissions;
using Codeshell.Abp.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Entities.Auditing;

#nullable disable

namespace Codeshell.Abp.Attachments
{
    public partial class AttachmentCategory : AuditedEntity<int>
    {
        public string NameEn { get; protected set; }
        public string NameAr { get; protected set; }
        public string ValidExtensions { get; protected set; }
        public int MaxSize { get; protected set; }
        public string FolderPath { get; protected set; }
        public Dimension MaxDimension { get; protected set; }
        public bool AnonymousDownload { get; protected set; }
        public int? MaxCount { get; protected set; }
        public string ContainerName { get; protected set; }

        public virtual ICollection<Attachment> Attachments { get; protected set; }
        public virtual ICollection<AttachmentCategoryPermission> AttachmentCategoryPermissions { get; protected set; }

        public AttachmentCategory()
        {
            Attachments = new HashSet<Attachment>();
        }

        public AttachmentCategory(int id, string name, string allowedExtensions, int maxSize, int? maxCount = null, string folderPath = null, Dimension dimension = null, string container = null, bool anonymousDownload = false)
        {
            Id = id;
            NameEn = name;
            ValidExtensions = allowedExtensions;
            MaxSize = maxSize;
            FolderPath = folderPath;
            MaxDimension = dimension;
            AttachmentCategoryPermissions = new HashSet<AttachmentCategoryPermission>();
            AnonymousDownload = anonymousDownload;
            MaxCount = maxCount;
            ContainerName = container;
        }

        public void SetAnonymousPermission(bool upload, bool download)
        {
            SetRolePermission("Anonymous", upload, download);
        }

        public void SetRolePermission<T>(T enumVal, bool upload, bool download)
        {
            SetRolePermission(enumVal.ToString(), upload, download);
        }

        public AttachmentCategory SetDisplay(string nameAr, string nameEn = null)
        {
            NameAr = nameAr;
            NameEn = nameEn ?? NameEn;
            return this;
        }

        public AttachmentCategory SetRolePermission(string role, bool upload, bool download)
        {
            var perm = AttachmentCategoryPermissions.FirstOrDefault(e => e.Role == role);
            var noPermission = !upload && !download;
            if (perm != null)
            {
                if (noPermission)
                    AttachmentCategoryPermissions.Remove(perm);
                else
                {
                    perm.SetPermission(upload, download);
                }
            }
            else if (!noPermission)
            {
                perm = new AttachmentCategoryPermission(role, upload, download);
                AttachmentCategoryPermissions.Add(perm);
            }
            return this;
        }

        public bool AllowAnonymousUpload()
        {
            return CanUpload("Anonymous");
        }

        public bool CanUpload(string role)
        {
            var perm = AttachmentCategoryPermissions.FirstOrDefault(e => e.Role == role);
            if (perm != null)
                return perm.Upload;
            return false;
        }

        public bool CanUpload(IEnumerable<string> roles)
        {
            foreach (var r in roles)
                if (CanUpload(r))
                    return true;
            return false;
        }

        public bool CanDownload(string role)
        {
            if (!AttachmentCategoryPermissions.Any(e => !e.Download))
                return true;

            var perm = AttachmentCategoryPermissions.FirstOrDefault(e => e.Role == role);
            if (perm != null)
                return perm.Download;
            return false;
        }



        public bool CanDownload(IEnumerable<string> roles)
        {
            foreach (var r in roles)
                if (CanDownload(r))
                    return true;
            return false;
        }

        public bool CanUploadEnum<T>(T role)
        {
            return CanUpload(role.ToString());
        }

        public bool CanDownloadEnum<T>(T role)
        {
            return CanDownload(role.ToString());
        }

        public void AllowUpload(params object[] roles)
        {
            foreach (var r in roles)
            {
                SetRoleUpload(r.ToString(), true);
            }
        }

        public void PreventDownload(params object[] roles)
        {
            foreach (var r in roles)
            {
                SetRoleDownload(r.ToString(), false);
            }
        }

        public void SetRoleUploadEnum<T>(T role, bool value)
        {
            SetRoleUpload(role.ToString(), value);
        }

        public void SetRoleDownloadEnum<T>(T role, bool value)
        {
            SetRoleUpload(role.ToString(), value);
        }

        public void SetRoleUpload(string role, bool value)
        {
            var perm = AttachmentCategoryPermissions.FirstOrDefault(e => e.Role == role);

            if (perm != null)
            {
                var noPermission = !value && !perm.Download;
                if (noPermission)
                    AttachmentCategoryPermissions.Remove(perm);
                else
                {
                    perm.SetPermission(value, null);
                }
            }
            else if (value)
            {
                perm = new AttachmentCategoryPermission(role, value, true);
                AttachmentCategoryPermissions.Add(perm);
            }
        }

        public void SetRoleDownload(string role, bool value)
        {
            var perm = AttachmentCategoryPermissions.FirstOrDefault(e => e.Role == role);

            if (perm != null)
            {
                var noPermission = !value && !perm.Upload;
                if (noPermission)
                    AttachmentCategoryPermissions.Remove(perm);
                else
                {
                    perm.SetPermission(null, value);
                }
            }
            else if (value)
            {
                perm = new AttachmentCategoryPermission(role, false, value);
                AttachmentCategoryPermissions.Add(perm);
            }
        }



        public bool ValidateFile(IFileInfo bytes, out ValidationResult message)
        {
            message = null;

            if (!ValidExtensions.ToLower().Contains(bytes.Extension.ToLower()))
            {
                message = new ValidationResult("MSG_invalid_file_type", new[] { bytes.Extension });
                return false;
            }

            if (MaxSize != 0 && bytes.Size > MaxSize)
            {
                message = new ValidationResult("MSG_file_exceeds_size", new[] { (MaxSize / 1024).ToString("D0") });
                return false;
            }

            if (MaxDimension != null && bytes.Dimesion != null && bytes.Dimesion.CompareTo(MaxDimension) < 0)
            {
                message = new ValidationResult("MSG_file_exceeds_dimensions", new[] { MaxDimension.Width.ToString(), MaxDimension.Height.ToString() });
                return false;
            }


            return true;
        }

        public void UpdateRestrictions(string validExtensions, int maxSize, Dimension maxDimension, int? maxCount = null)
        {
            ValidExtensions = validExtensions;
            MaxSize = maxSize;
            MaxDimension = maxDimension;
            MaxCount = maxCount;
        }

        public void SetAnonymousDownload(bool anonymousDownload)
        {
            AnonymousDownload = anonymousDownload;
        }
    }
}
