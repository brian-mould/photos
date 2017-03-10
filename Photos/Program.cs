using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Drawing;
using System.Drawing.Imaging;

namespace Photos
{
    class Program
    {

        /// <summary>
        /// Saves the Exif info.
        /// </summary>
        public static void SaveData(string photoName, PropertyItem prop)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            using (var db = new ExifDbContext())
            {
                var exif = new Exif
                {
                    PhotoId = photoName,
                    Key = prop.Id.ToString(),
                    Value = (prop.Type == 2) ? encoding.GetString(prop.Value) : prop.Value.ToString(),
                    DataType = prop.Type
                };

                db.Exifs.Add(exif);
                db.SaveChanges();
            }
        }

        public static void PrintPhotoInfo(string query)
        {
            using (var db = new ExifDbContext())
            {
                var exifs = db.Exifs.Where(x => x.PhotoId == query || x.Key == query || x.Value.Contains(query));
                foreach (Exif exif in exifs)
                {
                    Console.WriteLine("File: " + exif.PhotoId + "  || Key: " + exif.Key + " || Value: " + exif.Value);
                }
            }
        }

        static void ReadPhotos(string directory, int numPhotos)
        {
            AmazonS3Client _amazonS3Client;
            AmazonS3Config config = new AmazonS3Config();
            config.ServiceURL = directory;
            config.UseHttp = true;
            
            //public == empty secret key.
            _amazonS3Client = new AmazonS3Client("","",RegionEndpoint.USEast1);
           
            var objects = _amazonS3Client.ListObjects(directory);
            int photosLoaded = 0;
            foreach (S3Object obj in objects.S3Objects)
            {
                if (photosLoaded >= numPhotos)
                    break;
                try
                {
                    //metadata includes such things as 'coverphoto, facesetid, albumid, uploaderid, filename
                    //var metadata = _amazonS3Client.GetObjectMetadata(directory, obj.Key);
                    var obj2 = _amazonS3Client.GetObject(directory, obj.Key);
                    using (var response = _amazonS3Client.GetObject(directory, obj.Key))
                    {
                        using (var responseStream = response.ResponseStream)
                        {
                            Image img = new Bitmap(responseStream);
                            foreach (PropertyItem prop in img.PropertyItems)
                            {
                                SaveData(obj.Key, prop);                                
                            }
                        }
                    }
                    photosLoaded++;
                    Console.WriteLine("Loaded: " + obj.Key);
                    //foreach (string key in metadata.Metadata.Keys)
                    //{
                    //   Console.WriteLine(key + ":" + metadata.Metadata[key]);
                    //}
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
               
            }
            


        }
        static void Main(string[] args)
        {
            Console.WriteLine("How many photos would you like to load?");
            string numInput = Console.ReadLine(); //#TODO: Add validation
            int numPhotos = Convert.ToInt32(numInput);
            ReadPhotos("waldo-recruiting", numPhotos);
            while (true)
            {
                Console.WriteLine("Search for photo info. exit to quit");
                string control = Console.ReadLine();
                if (control == "exit")
                    break;
                PrintPhotoInfo(control);
            }
        }
    }
}
