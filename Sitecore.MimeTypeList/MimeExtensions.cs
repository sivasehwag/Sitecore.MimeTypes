// <copyright file="MimeExtensions.cs">
// Copyright (c) 2019
// </copyright>
// <author>sivakumar.r</author>

namespace Sitecore.MimeTypeList
{
    using Sitecore.Data;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="MimeExtensions" />
    /// </summary>
    public static class MimeExtensions
    {
        /// <summary>
        /// Defines the _defaultMimelist
        /// </summary>
        private static readonly string _defaultMimelist = "{A7A86B78-D703-4B0E-8357-E574A3CCE2DF}";

        /// <summary>
        /// Defines the _dictionaryitemlist
        /// </summary>
        private static readonly Lazy<IDictionary<string, string>> _dictionaryitemlist = new Lazy<IDictionary<string, string>>(CollectDictionary);

        /// <summary>
        /// Defines the _master
        /// </summary>
        private static Database _master = Database.GetDatabase("master");

        /// <summary>
        /// The ToDictionaryfromNV
        /// </summary>
        /// <param name="col">The col<see cref="NameValueCollection"/></param>
        /// <returns>The <see cref="Dictionary{string, string}"/></returns>
        public static Dictionary<string, string> ToDictionaryfromNV(this NameValueCollection collections)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var key in collections.AllKeys)
            {
                dict.Add("." + key.Trim().ToLower(), collections[key].Trim().ToLower());
            }
            return dict;
        }

        /// <summary>
        /// The CollectDictionary function
        /// </summary>
        /// <returns>The <see cref="IDictionary{string, string}"/></returns>
        private static IDictionary<string, string> CollectDictionary()
        {
            Dictionary<string, string> dictionaryitemlist = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { };
            var item = _master.GetItem(new ID(_defaultMimelist));
            if (item != null)
            {
                string keyValueRawValue = item["MimeTypeList"];
                NameValueCollection nameValueCollection = Web.WebUtil.ParseUrlParameters(keyValueRawValue);
                dictionaryitemlist = nameValueCollection.ToDictionaryfromNV();
            }
            var lst = dictionaryitemlist.ToList();
            foreach (var lstitem in lst)
            {
                if (!dictionaryitemlist.ContainsKey(lstitem.Value))
                {
                    dictionaryitemlist.Add(lstitem.Value, lstitem.Key);
                }
            }
            return dictionaryitemlist;
        }

        /// <summary>
        /// The GetMimeType
        /// Default mime type "application/octet-stream"
        /// </summary>
        /// <param name="extension">The extension<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetMimeType(string extension)
        {
            extension = extension.Trim().ToLower();
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            string mime;
            return _dictionaryitemlist.Value.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }

        /// <summary>
        /// The GetExtension
        /// Default extension is  "bin"
        /// </summary>
        /// <param name="mimeType">The mimeType<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetExtension(string mimeType)
        {
            mimeType = mimeType.Trim().ToLower();
            if (mimeType == null)
            {
                throw new ArgumentNullException("mimeType");
            }
            if (mimeType.StartsWith("."))
            {
                throw new ArgumentException("Requested mime type is not valid: " + mimeType);
            }
            string extension;
            return _dictionaryitemlist.Value.TryGetValue(mimeType, out extension) ? extension : "bin";
        }
    }
}
