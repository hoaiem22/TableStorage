using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Web.Mvc;

namespace BookStore.Controllers
{
    public class BlobStorageService : Controller
    {
        public static CloudBlobContainer GetCloudBlobContainer()
        {
            //Kết nối đến storage của Cloud thông qua chuổi kết nói ImageStorageAccountConn
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("BlobConnectionString"));
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("BlobConnectionString"));
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("CloudBlobConnetrionString"));
            //Khai báo CloudBlobClient để lưu trữ hình ảnh
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            //Tạo BlobStorage tên "myimages" để lưu trữ hình ảnh cho website
            CloudBlobContainer blobContainer = blobClient.GetContainerReference("myimages");
            //Tạo Blob nếu không tồn tại
            if (blobContainer.CreateIfNotExists())
            {
                //Thiết lập các quyền truy cập Blob
                blobContainer.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
            return blobContainer;
        }
    }
}