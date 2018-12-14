﻿// This file is part of Mystery Dungeon eXtended.

// Mystery Dungeon eXtended is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Mystery Dungeon eXtended is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with Mystery Dungeon eXtended.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Specialized;

namespace PMDCP.Net
{
    /// <summary>
    /// Submits post data to a url.
    /// </summary>
    public class PostSubmitter
    {
        /// <summary>
        /// determines what type of post to perform.
        /// </summary>
        public enum PostTypeEnum
        {
            /// <summary>
            /// Does a get against the source.
            /// </summary>
            Get,
            /// <summary>
            /// Does a post against the source.
            /// </summary>
            Post
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PostSubmitter() {
        }

        /// <summary>
        /// Constructor that accepts a url as a parameter
        /// </summary>
        /// <param name="url">The url where the post will be submitted to.</param>
        public PostSubmitter(string url)
            : this() {
            Url = url;
        }

        /// <summary>
        /// Constructor allowing the setting of the url and items to post.
        /// </summary>
        /// <param name="url">the url for the post.</param>
        /// <param name="values">The values for the post.</param>
        public PostSubmitter(string url, NameValueCollection values)
            : this(url) {
            PostItems = values;
        }

        /// <summary>
        /// Gets or sets the url to submit the post to.
        /// </summary>
        public string Url { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the name value collection of items to post.
        /// </summary>
        public NameValueCollection PostItems { get; set; } = new NameValueCollection();
        /// <summary>
        /// Gets or sets the type of action to perform against the url.
        /// </summary>
        public PostTypeEnum Type { get; set; } = PostTypeEnum.Get;
        /// <summary>
        /// Posts the supplied data to specified url.
        /// </summary>
        /// <returns>a string containing the result of the post.</returns>
        public string Post() {
            StringBuilder parameters = new StringBuilder();
            for (int i = 0; i < PostItems.Count; i++) {
                EncodeAndAddItem(ref parameters, PostItems.GetKey(i), PostItems[i]);
            }
            string result = PostData(Url, parameters.ToString());
            return result;
        }
        /// <summary>
        /// Posts the supplied data to specified url.
        /// </summary>
        /// <param name="url">The url to post to.</param>
        /// <returns>a string containing the result of the post.</returns>
        public string Post(string url) {
            Url = url;
            return Post();
        }
        /// <summary>
        /// Posts the supplied data to specified url.
        /// </summary>
        /// <param name="url">The url to post to.</param>
        /// <param name="values">The values to post.</param>
        /// <returns>a string containing the result of the post.</returns>
        public string Post(string url, NameValueCollection values) {
            PostItems = values;
            return Post(url);
        }
        /// <summary>
        /// Posts data to a specified url. Note that this assumes that you have already url encoded the post data.
        /// </summary>
        /// <param name="postData">The data to post.</param>
        /// <param name="url">the url to post to.</param>
        /// <returns>Returns the result of the post.</returns>
        private string PostData(string url, string postData) {
            HttpWebRequest request = null;
            if (Type == PostTypeEnum.Post) {
                Uri uri = new Uri(url);
                request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postData.Length;
                using (Stream writeStream = request.GetRequestStream()) {
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] bytes = encoding.GetBytes(postData);
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            } else {
                Uri uri = new Uri(url + "?" + postData);
                request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
            }
            string result = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                using (Stream responseStream = response.GetResponseStream()) {
                    using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8)) {
                        result = readStream.ReadToEnd();
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Encodes an item and ads it to the string.
        /// </summary>
        /// <param name="baseRequest">The previously encoded data.</param>
        /// <param name="dataItem">The data to encode.</param>
        /// <returns>A string containing the old data and the previously encoded data.</returns>
        private void EncodeAndAddItem(ref StringBuilder baseRequest, string key, string dataItem) {
            if (baseRequest == null) {
                baseRequest = new StringBuilder();
            }
            if (baseRequest.Length != 0) {
                baseRequest.Append("&");
            }
            baseRequest.Append(key);
            baseRequest.Append("=");
            baseRequest.Append(System.Web.HttpUtility.UrlEncode(dataItem));
        }
    }
}
