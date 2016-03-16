using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiS3ACLSetting
{
    class Media : IMedia
    {
        public string key { get; set; }

        public Media() { ; }

        public async Task<string> DoWork()
        //public async Task DoWork()
        {
            //await Task.Run(() => PutObjectACL(key)  );                
            /*
            await s3client.PutACLAsync(new PutACLRequest
            {
                BucketName = Bucketname,
                Key = key,
                CannedACL = S3CannedACL.PublicRead
            });
            */

            await Task.Delay(2000);

            return "End Request: " + this.key;
        }
    }
}
