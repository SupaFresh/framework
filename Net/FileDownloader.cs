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

using System;
using System.IO;
using System.Net;
using System.ComponentModel;
using PMDCP.Core;

namespace PMDCP.Net
{
    public class FileDownloader
    {
        public event EventHandler<FileDownloadingEventArgs> DownloadUpdate;
        public event EventHandler<FileDownloadingEventArgs> DownloadComplete;
        public event EventHandler<FileDownloadErrorEventArgs> DownloadFailed;

        BackgroundWorker downloadBWorker;

        public bool HasDownloadFailed { get; private set; }

        public void DownloadFile(string downloadPath, string filePath) {
            downloadBWorker = new BackgroundWorker();
            downloadBWorker.DoWork += new DoWorkEventHandler(downloadBWorker_DoWork);
            downloadBWorker.WorkerReportsProgress = true;
            downloadBWorker.ProgressChanged += new ProgressChangedEventHandler(downloadBWorker_ProgressChanged);
            FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            downloadBWorker.RunWorkerAsync(new object[] { downloadPath, stream });
            //do {
            //} while (downloadBWorker.IsBusy == true);
        }

        public void DownloadFile(string downloadPath, Stream saveStream) {
            downloadBWorker = new BackgroundWorker();
            downloadBWorker.DoWork += new DoWorkEventHandler(downloadBWorker_DoWork);
            downloadBWorker.WorkerReportsProgress = true;
            downloadBWorker.ProgressChanged += new ProgressChangedEventHandler(downloadBWorker_ProgressChanged);
            downloadBWorker.RunWorkerAsync(new object[] { downloadPath, saveStream });
        }

        void downloadBWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            object[] data = e.UserState as object[];
            switch ((string)data[0]) {
                case "downloading": {
                        FileDownloadingEventArgs downloadInfo = data[1] as FileDownloadingEventArgs;
                        DownloadUpdate?.Invoke(this, downloadInfo);
                    }
                    break;
                case "done": {
                        FileDownloadingEventArgs downloadInfo = data[1] as FileDownloadingEventArgs;
                        DownloadComplete?.Invoke(this, downloadInfo);
                    }
                    break;
                case "error": {
                        DownloadFailed?.Invoke(this, new FileDownloadErrorEventArgs(data[1] as Exception));
                    }
                    break;
            }
        }

        void downloadBWorker_DoWork(object sender, DoWorkEventArgs e) {
            string downloadPath = ((object[])e.Argument)[0] as string;
            using (Stream stream = ((object[])e.Argument)[1] as Stream) {
                try {
                    HttpWebResponse theResponse;
                    HttpWebRequest theRequest;
                    //Checks if the file exist

                    try {
                        theRequest = (HttpWebRequest)WebRequest.Create(downloadPath);
                        theResponse = (HttpWebResponse)theRequest.GetResponse();
                    }
                    catch (Exception ex) {
                        HasDownloadFailed = true;
                        downloadBWorker.ReportProgress(0, new object[] { "error", ex });
                        return;
                    }
                    long length = theResponse.ContentLength;
                    //Size of the response (in bytes)

                    //FileStream writeStream = new FileStream(filePath + ".tmp", FileMode.Create);

                    //Replacement for Stream.Position (webResponse stream doesn't support seek)
                    long nRead = 0;

                    do {
                        byte[] readBytes = new byte[1024];
                        int bytesread = theResponse.GetResponseStream().Read(readBytes, 0, 1024);

                        nRead += bytesread;

                        if (bytesread == 0)
                            break;

                        int percent = (int)MathFunctions.CalculatePercent(nRead, length);
                        stream.Write(readBytes, 0, bytesread);
                        if (DownloadUpdate != null) {
                            downloadBWorker.ReportProgress(percent, new object[] { "downloading", new FileDownloadingEventArgs(length, "", percent, nRead) });
                        }
                    }
                    while (true);

                    //Close the streams
                    theResponse.GetResponseStream().Close();
                    if (DownloadComplete != null) {
                        downloadBWorker.ReportProgress(100, new object[] { "done", new FileDownloadingEventArgs(length, "", 100, length) });
                    }
                }
                catch (Exception ex) {
                    downloadBWorker.ReportProgress(0, new object[] { "error", ex });
                }
            }
        }

        public void DownloadFileQuick(string downloadUri, Stream saveStream) {
            HttpWebResponse theResponse;
            HttpWebRequest theRequest;
            //Checks if the file exist
            try {
                theRequest = (HttpWebRequest)WebRequest.Create(downloadUri);
                theResponse = (HttpWebResponse)theRequest.GetResponse();
            }
            catch (Exception) {
                HasDownloadFailed = true;
                return;
            }
            long length = theResponse.ContentLength;
            //Size of the response (in bytes)
            long nRead = 0;

            do {
                byte[] readBytes = new byte[1024];
                int bytesread = theResponse.GetResponseStream().Read(readBytes, 0, 1024);

                nRead += bytesread;

                if (bytesread == 0)
                    break;

                //int percent = (int)MathFunctions.CalculatePercent(nRead, length);
                saveStream.Write(readBytes, 0, bytesread);
            }
            while (true);
        }
    }
}
