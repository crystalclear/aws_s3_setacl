using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Diagnostics;
using Amazon.S3.Model;
using Amazon.S3;
using Amazon;
using System.Threading;

namespace MultiS3ACLSetting
{
    class Program
    {
        static void Main(string[] args)
        {
            Hints();

            try
            {
                Param param = new Param(args);

                if (param.ShowHelp)
                {
                    param.PrintUsage();
                    return; 
                }

                List<Media> medien = Helper.GetListOfMediaObjects(param);

                if (medien != null)
                {
                    AmazonS3Client s3client = new CUAWS(param).GetS3Client();

                    IEnumerable<Media> medienNew = medien.Take<Media>(param.NumberOfSimultaneousRequests);

                    if (medienNew.Count() > 0)
                    {
                        int n = 1;
                        do
                        {
                            Thread.Sleep(param.NextRequestInMilliSeconds);

                            StartMultiRequests(medienNew, param, s3client);

                            medienNew = medien.Skip<Media>(param.NumberOfSimultaneousRequests * n).Take<Media>(param.NumberOfSimultaneousRequests);

                            n++;
                        } while (medienNew.Any<Media>());
                    }
                }
                else
                {
                    Display("Keine Daten vorhanden.");
                }
                Display("##################################### Fertig #############################################");
            }
            catch (Exception e)
            {
                Display("Es ist ein unerwarteter Fehler aufgetreten. " + e.Message);
            }
            Console.Read();
        }

        private static void StartMultiRequests(IEnumerable<Media> medienTake, Param param, AmazonS3Client s3client)
        {
            var tasks = new Task<string>[medienTake.Count()];
            int loop = 0;
            foreach (Media media in medienTake)
            {
                tasks[loop] = Task.Run(async () => {  
                                                      var res = await PutObjectACL(media.key, param, s3client);
                                                      Display("Done! " + res);
                                                      return res;
                                                    });

                loop++;
            }
            var batch = Task.WhenAll(tasks);           
        }
        
        private static async Task<string> PutObjectACL(string key, Param param, AmazonS3Client s3client)
        {
            Exception ex = null;
            try
            {
                PutACLResponse response = await s3client.PutACLAsync(new PutACLRequest
                {
                    BucketName = param.Bucketname,
                    Key = key,
                    CannedACL = (param.ACL == "private") ? S3CannedACL.Private : S3CannedACL.PublicRead 
                });               
                return "Success: MediaObjectKey: " + key + " HTTP Status:" + response.HttpStatusCode;
            }
            catch(Exception e)
            {
                ex = e;                
            }

            // #####################################################################################################################
            // # WORKAROUND: Erst ab Version C#6.0 koennen asynchrone Methoden innerhalb von try-catch-Bloecke ausgefuehrt werden!!!
            // #####################################################################################################################
            if(!String.IsNullOrWhiteSpace(param.Log))
            {
                try
                {
                    if (ex != null)
                    {
                        await WriteToFileAsync(key, param);
                        return "ERROR: Logging - MediaObjectKey: " + key + " Error: " + ex.Message;                    
                    }
                }
                catch (Exception e)
                {
                    return "ERROR: Konnte nicht die Log-Datei schreiben: " + e.Message; 
                }
            }
            return "ERROR: Nicht behandelter Fehler!!! MediaObjectKey: " + key;
            // #####################################################################################################################
            // #####################################################################################################################
        }

        private static async Task WriteToFileAsync(string key, Param param) 
        {
            const int TIMEOUT = 15000;

            bool isTimeout = false;
            bool stoppLooping = false;            
            string filePath = param.Log;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!stoppLooping && !isTimeout)
            {
                if(!IsFileLocked(filePath))
                {                    
                    try
                    {
                        byte[] encodedKey = Encoding.Unicode.GetBytes(key + "\n");

                        using (FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                        {
                            stoppLooping = true;

                            await stream.WriteAsync(encodedKey, 0, encodedKey.Length);
                            
                            Console.WriteLine("+++++++++++++++++++ Log geschrieben.... ++++++++++++++++++++"); 
                            
                            break;
                        };
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("!!!!!!!!!!!!!!! Fehler beim schreiben in LogFile. !!!!!!!!!!!!!!! " + e.Message);
                        stoppLooping = true;                        
                        break;
                    }                         
                }
                isTimeout = (stopwatch.ElapsedMilliseconds < TIMEOUT);
                if (isTimeout)
                    Console.WriteLine("----------------- Vorzeitiger Abbruch. Zeitueberschreitung. ------------------");
            }
            stopwatch.Reset(); 
            stopwatch.Stop();            
        }

        private static async Task<bool> WritePrepareAsync(FileStream stream, byte[] encodedKey, FileInfo info)
        {
            do
            {
                await stream.WriteAsync(encodedKey, 0, encodedKey.Length);
                Thread.Sleep(200);
            } while (false); //  (IsFileLocked(info));
           
            return true;
        }

        static async Task WriteTextAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }

        protected static bool IsFileLocked(string filePath)
        {
            // http://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-file-is-in-use

            FileStream stream = null;

            try
            {
                if (!File.Exists(filePath))
                {
                    CreateEmptyFile(filePath);
                }

                FileInfo file = new FileInfo(filePath);

                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                //Console.WriteLine("IsFileLocked = true");
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            //Console.WriteLine("IsFileLocked = false");
            return false;
        }
      
        public static void CreateEmptyFile(string filename)
        {
            File.Create(filename).Dispose();
        }
      
        private static void Hints()
        {
            StringBuilder hints = new StringBuilder();
            hints.AppendLine("\n\n+++++++++++++++++++++ CU-AWS ++++++++++++++++++++++\n");
            hints.AppendLine("Description: CU AWS-S3 ACL Modification\n");
            hints.AppendLine("Version: 0.0.1 (beta)\n");
            hints.AppendLine("Author: MNE\n");
            hints.AppendLine("+++++++++++++++++++++++++++++++++++++++++++++++++++\n");

            Console.WriteLine(hints.ToString());
        }

        private static void Useage()
        {
            Console.Write("usage: cuaws [options] <command> <subcommand> <parameters>");
            
            Console.WriteLine("Beispiel: # cuaws --bucket bucketname --acl public --req 100 --interval 1000 --file C:\\Users\\xxx\\Downloads\\aws\\tools\\data\\s3acl.txt --log C:\\Users\\xxx\\Downloads\\aws\\tools\\data\\log.txt");
        }

        private static void Display(string message)
        {
            Console.Write(message + "\n");
        }
    }
}





