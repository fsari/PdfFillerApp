using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using Minio.Exceptions;

namespace PdfFillerApp.Helper
{
    public static class MinioHelper
    {
        public static IConfiguration? Configuration;


        public static async Task<string> Upload(string file, string folder, string contentType)
        {
            string endpoint = Configuration.GetSection("Minio").GetSection("EndPoint").Value;
            string accessKey = Configuration.GetSection("Minio").GetSection("Accesskey").Value;
            string secretKey = Configuration.GetSection("Minio").GetSection("Secretkey").Value;
            string bucketName = Configuration.GetSection("Minio").GetSection("BucketName").Value;

            MinioClient minio = (MinioClient)new MinioClient()
                                .WithEndpoint(endpoint)
                                .WithCredentials(accessKey, secretKey)
                                .WithSSL()
                                .Build();


            //Aes aesEncryption = Aes.Create();
            //aesEncryption.KeySize = 256;
            //aesEncryption.GenerateKey();
            //SSEC ssec = new SSEC(aesEncryption.Key);

            try
            {
                BucketExistsArgs beArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);

                bool found = await minio.BucketExistsAsync(beArgs).ConfigureAwait(false);

                if (!found)
                {
                    MakeBucketArgs mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);

                    minio.MakeBucketAsync(mbArgs).GetAwaiter().GetResult();
                }


                PutObjectArgs putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject($"/{folder.ToLower()}/{file.Split("\\").Last()}")
                    .WithFileName(file)
                    .WithContentType(contentType);


                PutObjectResponse result = await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                return result.ObjectName;
            }
            catch (MinioException e)
            {
                Console.WriteLine("File Upload Error: {0}", e.Message);
                return e.Message;
            }
        }

        public static async Task<List<string>> ListObjects()
        {
            string endpoint = Configuration.GetSection("Minio").GetSection("EndPoint").Value;  
            string accessKey = Configuration.GetSection("Minio").GetSection("Accesskey").Value; 
            string secretKey = Configuration.GetSection("Minio").GetSection("Secretkey").Value; 
            string bucketName = Configuration.GetSection("Minio").GetSection("BucketName").Value;
            List<string> list = new();

            MinioClient minio = (MinioClient)new MinioClient()
                                .WithEndpoint(endpoint)
                                .WithCredentials(accessKey, secretKey)
                                .WithSSL()
                                .Build();

            try
            {
                BucketExistsArgs beArgs = new BucketExistsArgs()
                   .WithBucket(bucketName);

                bool found = minio.BucketExistsAsync(beArgs).GetAwaiter().GetResult();

                if (!found)
                {
                    minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName)).GetAwaiter().GetResult();
                }

                ListObjectsArgs args = new ListObjectsArgs()
                                          .WithBucket(bucketName)
                                          //.WithPrefix("prefix")
                                          .WithRecursive(true);
                IObservable<Item> observable = minio.ListObjectsAsync(args);
                IDisposable subscription = observable.Subscribe(
                        item =>
                        {
                            list.Add(item.Key);
                            Console.WriteLine("OnNext: {0}", item.Key);
                        },
                        ex => Console.WriteLine("OnError: {0}", ex.Message),
                        () => Console.WriteLine("OnComplete: {0}"));
            }
            catch (MinioException e)
            {
                Console.WriteLine("Error occurred: " + e);
            }
            return list;

        }


        public static async Task DownloadFile(string fileName)
        {

            fileName = fileName.TrimStart('/');
            string endpoint = Configuration.GetSection("Minio").GetSection("EndPoint").Value;
            string accessKey = Configuration.GetSection("Minio").GetSection("Accesskey").Value;
            string secretKey = Configuration.GetSection("Minio").GetSection("Secretkey").Value;
            string bucketName = Configuration.GetSection("Minio").GetSection("BucketName").Value;


            MinioClient minio = (MinioClient)new MinioClient()
                                .WithEndpoint(endpoint)
                                .WithCredentials(accessKey, secretKey)
                                .WithSSL()
                                .Build();

            try
            {

                var args = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName)
                    .WithFile(@"wwwroot\Temp\Downloaded\" + fileName.Split('/').Last());

                var file = await minio.GetObjectAsync(args).ConfigureAwait(false);


            }
            catch (MinioException e)
            {
                Console.WriteLine($"[Bucket]  Exception: {e}");
            }
        }


           public static async Task GetPdfFile(string fileName)
        {

            fileName = fileName.TrimStart('/');
            string endpoint = Configuration.GetSection("Minio").GetSection("EndPoint").Value;
            string accessKey = Configuration.GetSection("Minio").GetSection("Accesskey").Value;
            string secretKey = Configuration.GetSection("Minio").GetSection("Secretkey").Value;
            string bucketName = Configuration.GetSection("Minio").GetSection("BucketName").Value;


            MinioClient minio = (MinioClient)new MinioClient()
                                .WithEndpoint(endpoint)
                                .WithCredentials(accessKey, secretKey)
                                .WithSSL()
                                .Build();

            try
            {

                var args = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName)
                    .WithFile(@"wwwroot\Temp\Downloaded\" + fileName.Split('/').Last());

                var file = await minio.GetObjectAsync(args).ConfigureAwait(false);


            }
            catch (MinioException e)
            {
                Console.WriteLine($"[Bucket]  Exception: {e}");
            }
        }









    }
}
