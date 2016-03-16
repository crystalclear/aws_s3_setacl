using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiS3ACLSetting
{
    class Param
    {
        private int defaultNumberOfSimultaneousRequests = 100;
        private int defaultNextRequestInMilliSeconds = 1000;
        private string defaultACL = "public";
        private string defaultBucketname = "BUCKETNAME";
        private string defaultFilename = @"../../data/s3acl_test.txt"; 
        private string defaultLogFile = null; // @"../../data/log.txt";
        private char defaultFileDelimiter = ' ';
        private int defaultFileRowNumber = 6;
        private string defaultFileExclude = @"images/";
        private bool defaultShowHelp = true;

        public int NumberOfSimultaneousRequests { get; set; }
        public int NextRequestInMilliSeconds { get; set; }
        public string ACL { get; set; }
        public string Filename { get; set; }
        public string Bucketname { get; set; }
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public string Region { get; set; }
        public string Log { get; set; }
        public char FileDelimiter { get; set; }
        public int FileRowNumber { get; set; }
        public string FileExclude { get; set; }
        public bool ShowHelp { get; set; }

        public Param()
        {
            this.NumberOfSimultaneousRequests = defaultNumberOfSimultaneousRequests;
            this.NextRequestInMilliSeconds = defaultNextRequestInMilliSeconds;
            this.Bucketname = defaultBucketname;
            this.ACL = defaultACL;
            this.Filename = defaultFilename;
            this.Log = defaultLogFile;
            this.FileDelimiter = defaultFileDelimiter;
            this.FileRowNumber = defaultFileRowNumber;
            this.FileExclude = defaultFileExclude;
            this.ShowHelp = defaultShowHelp;
        }

        public Param(string[] args) 
        {
            this.NumberOfSimultaneousRequests = defaultNumberOfSimultaneousRequests;
            this.NextRequestInMilliSeconds = defaultNextRequestInMilliSeconds;
            this.Bucketname = defaultBucketname;
            this.ACL = defaultACL;
            this.Filename = defaultFilename;
            this.Log = defaultLogFile;
            this.FileDelimiter = defaultFileDelimiter;
            this.FileRowNumber = defaultFileRowNumber;
            this.FileExclude = defaultFileExclude;
            this.ShowHelp = defaultShowHelp;

            if (args != null)
                this.SetInputParams(args);
        }

        private void SetInputParams(string[] args)
        {
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string argument = args[i];

                    switch (argument)
                    {
                        case "--bucket":
                            if (!String.IsNullOrWhiteSpace(args[i + 1]))
                            {
                                this.ShowHelp = false;
                                this.Bucketname = args[i + 1].Trim();
                            }
                            break;
                        case "--acl":
                            if (!String.IsNullOrWhiteSpace(args[i + 1]))
                            {
                                this.ShowHelp = false;
                                this.ACL = args[i + 1].Trim();
                            }
                            break;
                        case "--req":
                            if (!String.IsNullOrWhiteSpace(args[i + 1]))
                            {
                                this.ShowHelp = false;
                                this.NumberOfSimultaneousRequests = Int32.Parse(args[i + 1]);
                            }
                            break;
                        case "--interval":
                            if (!String.IsNullOrWhiteSpace(args[i + 1]))
                            {
                                this.ShowHelp = false;
                                this.NextRequestInMilliSeconds = Int32.Parse(args[i + 1]);
                            }
                            break;
                        case "--file":
                            if (!String.IsNullOrWhiteSpace(args[i + 1]))
                            {
                                this.ShowHelp = false;
                                this.Filename = args[i + 1].Trim();
                            }
                            break;
                        case "--accesskey":
                            if (!String.IsNullOrWhiteSpace(args[i + 1]))
                            {
                                this.ShowHelp = false;
                                this.AWSAccessKey = args[i + 1].Trim();
                            }
                            break;
                        case "--secretkey":
                            if (!String.IsNullOrWhiteSpace(args[i + 1]))
                            {
                                this.ShowHelp = false;
                                this.AWSSecretKey = args[i + 1].Trim();
                            }
                            break;
                        case "--region":
                            if (!String.IsNullOrWhiteSpace(args[i + 1]))
                            {
                                this.ShowHelp = false;
                                this.Region = args[i + 1].Trim();
                            }
                            break;
                        case "--log":                            
                            if (!String.IsNullOrWhiteSpace(args[i + 1]))
                            {
                                this.ShowHelp = false;
                                this.Log = args[i + 1].Trim();
                            }
                            break;
                        case "--file-delimiter":                            
                            if (!String.IsNullOrWhiteSpace(args[i + 1]))
                            {
                                this.ShowHelp = false;
                                this.FileDelimiter = Char.Parse(args[i + 1].Trim());
                            }
                            break;
                        case "--file-rownum":
                            if (!String.IsNullOrWhiteSpace(args[i + 1]))
                            {
                                this.ShowHelp = false;
                                this.FileRowNumber = Int32.Parse(args[i + 1]); 
                            }
                            break;
                        case "--file-exclude":                            
                            if (!String.IsNullOrWhiteSpace(args[i + 1]))
                            {
                                this.ShowHelp = false;
                                this.FileExclude = args[i + 1].Trim();
                            }
                            break;    
                        case "--help":
                            this.ShowHelp = true;
                            //PrintUsage();
                            break;
                    }
                }
            }
        }

        public void PrintUsage()
        {
            Console.WriteLine("usage: cuaws [options] <command> <subcommand> <parameters>");
            
            Console.WriteLine("\nOptionen:");
            Console.WriteLine("\t--bucket\t\tGibt den Namen des Buckets an.");
            Console.WriteLine("\t--acl\t\t\tGibt die Methode an welche ausgefuehrt werden soll.");
            Console.WriteLine("\t--req\t\t\tAnzahl gleichzeitig ausgefuehrten Anfragen.");
            Console.WriteLine("\t--interval\t\tDas Zeitintervall bis die naechsten Anfragen gesendet werden.");
            Console.WriteLine("\t--file\t\t\tDatei mit den S3 Objekt-Keys.");
            Console.WriteLine("\t--accesskey\t\tAmazon AWS Access Key.");
            Console.WriteLine("\t--secretkey\t\tAmazon AWS Secret Key.");
            Console.WriteLine("\t--region\t\tGibt die Region des S3 Buckets an.");
            Console.WriteLine("\t--log\t\t\tDateiname in das Fehlerlog geschrieben werden sollen.");
            Console.WriteLine("\t--file-delimiter\tAngabe des Trennzeichens welches für CSV Dateien verwendet wird.");
            Console.WriteLine("\t--file-rownum\t\tAngabe der Spalte welche die S3 Objekt-Keys enthaelt.");
            Console.WriteLine("\t--file-exclude\t\tAusschluss bestimmter Inhalte.");
            Console.WriteLine("\t--help\t\t\tGibt die Hilfe aus.");

            Console.WriteLine("\nExample:");
            Console.WriteLine("cuaws --bucket bucketname --acl public --req 100 --interval 1000 --file C:\\Users\\xxx\\Downloads\\aws\\data\\s3acl_test.txt");
  
            Console.Read();
        }
    }
}


