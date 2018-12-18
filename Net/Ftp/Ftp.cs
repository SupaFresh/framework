// This file is part of Mystery Dungeon eXtended.

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

using System.Collections.Generic;
using System.IO;
using System.Net;

namespace PMDCP.Net.Ftp
{
    public class Ftp
    {
        // The hostname or IP address of the FTP server
        private readonly string remoteHost;

        // The remote username
        private readonly string remoteUser;

        // Password for the remote user
        private readonly string remotePass;

        public Ftp(string remoteHost, string remoteUser, string remotePassword, bool debug)
        {
            this.remoteHost = remoteHost;
            this.remoteUser = remoteUser;
            remotePass = remotePassword;
        }

        /// <summary>
        /// Get a list of files and folders on the FTP server
        /// </summary>
        /// <returns></returns>
        public List<string> DirectoryListing()
        {
            return DirectoryListing(string.Empty);
        }

        /// <summary>
        /// List files and folders in a given folder on the server
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public List<string> DirectoryListing(string folder)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remoteHost + folder);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(remoteUser, remotePass);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            List<string> result = new List<string>();

            while (!reader.EndOfStream)
            {
                result.Add(reader.ReadLine());
            }

            reader.Close();
            response.Close();
            return result;
        }

        /// <summary>
        /// Download a file from the FTP server to the destination
        /// </summary>
        /// <param name="filename">filename and path to the file, e.g. public_html/test.zip</param>
        /// <param name="destination">The location to save the file, e.g. c:\\test.zip</param>
        public void Download(string filename, string destination)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remoteHost + filename);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(remoteUser, remotePass);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            StreamWriter writer = new StreamWriter(destination);
            writer.Write(reader.ReadToEnd());

            writer.Close();
            reader.Close();
            response.Close();
        }

        public void Download(string filename, Stream destinationStream)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remoteHost + filename);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(remoteUser, remotePass);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();

            int bufferSize = 2048;
            int readCount;
            byte[] buffer = new byte[bufferSize];

            readCount = responseStream.Read(buffer, 0, bufferSize);
            while (readCount > 0)
            {
                destinationStream.Write(buffer, 0, readCount);
                readCount = responseStream.Read(buffer, 0, bufferSize);
            }

            response.Close();
        }

        /// <summary>
        /// Remove a file from the server.
        /// </summary>
        /// <param name="filename">filename and path to the file, e.g. public_html/test.zip</param>
        public void DeleteFileFromServer(string filename)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remoteHost + filename);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential(remoteUser, remotePass);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            response.Close();
        }

        /// <summary>
        /// Upload a file to the server
        /// </summary>
        /// <param name="source">Full path to the source file e.g. c:\test.zip</param>
        /// <param name="destination">destination folder and filename e.g. public_html/test.zip</param>
        public void UploadFile(string source, string destination)
        {
            //string filename = Path.GetFileName(source);

            //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remoteHost + destination);
            //request.Method = WebRequestMethods.Ftp.UploadFile;
            //request.Credentials = new NetworkCredential(remoteUser, remotePass);

            //StreamReader sourceStream = new StreamReader(source);
            //byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());

            //request.ContentLength = fileContents.Length;

            //Stream requestStream = request.GetRequestStream();
            //requestStream.Write(fileContents, 0, fileContents.Length);

            //FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            //response.Close();
            //requestStream.Close();
            //sourceStream.Close();
            UploadFile(new FileStream(source, FileMode.Open, FileAccess.Read), destination);
        }

        public void UploadFile(Stream sourceStream, string destination)
        {
            //string filename = Path.GetFileName(source);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remoteHost + destination);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(remoteUser, remotePass);

            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;

            //request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();

            // Read from the file stream 2kb at a time
            contentLen = sourceStream.Read(buff, 0, buffLength);

            // Till Stream content ends
            while (contentLen != 0)
            {
                // Write Content from the file stream to the FTP Upload Stream
                requestStream.Write(buff, 0, contentLen);
                contentLen = sourceStream.Read(buff, 0, buffLength);
            }

            requestStream.Close();
        }
    }
}