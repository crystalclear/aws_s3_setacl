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
    class CUAWS
    {
        private const string AWSACCESSKEY = "XXXXXXXXXXXXXXXXX";
        private const string AWSSECRETKEY = "YYYYYYYYYYYYYYYYYYYYYYYY+C";        
        private const string BUCKETNAME = "BUCKETNAME";  
        private const string REGION = "s3-eu-central-1";

        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public string Region { get; set; }
        public string Bucketname { get; set; }

        public CUAWS(Param param)
        {
            this.AWSAccessKey = (param.AWSAccessKey!=null) ? param.AWSAccessKey : AWSACCESSKEY;
            this.AWSSecretKey = (param.AWSSecretKey!=null) ? param.AWSSecretKey : AWSSECRETKEY;
            this.Bucketname = (param.Bucketname!=null) ? param.Bucketname : BUCKETNAME;
            this.Region = (param.Region!=null) ? param.Region : REGION;
        }

        public AmazonS3Client GetS3Client()
        {
            if (!CheckAWSParams()) return null;
            
            Amazon.S3.AmazonS3Client client = new Amazon.S3.AmazonS3Client(this.AWSAccessKey, this.AWSSecretKey, this.GetAWSRegion(this.Region));
            return client;
        }

        private bool CheckAWSParams()
        {
            if (!String.IsNullOrWhiteSpace(this.AWSAccessKey) && !String.IsNullOrWhiteSpace(this.AWSSecretKey) && !String.IsNullOrWhiteSpace(this.Bucketname))
                return true;
            else
                return false;
        }

        private RegionEndpoint GetAWSRegion(string region)
        {
            if (String.IsNullOrWhiteSpace(region)) return null;

            switch (region)
            {
                case "s3-eu-west-1": return RegionEndpoint.EUWest1;
                case "s3-eu-central-1": return RegionEndpoint.EUCentral1;
                case "s3-ap-northeast-1": return RegionEndpoint.APNortheast1;
                case "s3-ap-southeast-1": return RegionEndpoint.APSoutheast1;
                case "s3-ap-southeast-2": return RegionEndpoint.APSoutheast2;
                case "???": return RegionEndpoint.CNNorth1;
                case "s3-sa-east-1": return RegionEndpoint.SAEast1;
                case "s3-us-east-1": return RegionEndpoint.USEast1;
                case "s3-us-gov-west-1": return RegionEndpoint.USGovCloudWest1;
                case "s3-us-west-1": return RegionEndpoint.USWest1;
                case "s3-us-west-2": return RegionEndpoint.USWest2;

                default: return null;
            }
        }
    }
}
