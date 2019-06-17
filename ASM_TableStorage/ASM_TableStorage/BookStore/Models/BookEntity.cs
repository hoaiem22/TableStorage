using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Web;
using Microsoft.WindowsAzure.Storage.Blob;
using BookStore.Controllers;

namespace BookStore.Models
{
    //Khai báo các lớp Book
    public class BookEntity : TableEntity
    {
        public int BookID { get; set; }
        public string BookTitle { get; set; }
        public double BookPrice { get; set; }
        public string BookImage { get; set; }
    }
    //Khai báo lớp thực hiện thao tác: thêm, xóa và lấy dữ liệu bằng Books
    public class BookDataSource
    {
        //Khai báo đối tượng để thực hiện các thao tác.
        private CloudTable cloudTable = null;
        public BookDataSource()
        {
            //Tham chiếu đến CloudStorage đưa vào chuỗi kết nối DataConnectionString
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString: CloudConfigurationManager.GetSetting("DataConnectionString"));
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DataConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"));
            CloudTableClient clientTable = storageAccount.CreateCloudTableClient();
            //Tạo tham chiếu đến bằng Books
            cloudTable = clientTable.GetTableReference("Books");
            //Tạo bảng Books nếu chưa có.
            cloudTable.CreateIfNotExists();
        }
        //Lấy dữ liệu trong bảng Books
        public IEnumerable<BookEntity> GetBooks()
        {
            //Lấy bảng Book
            TableQuery<BookEntity> query = new TableQuery<BookEntity>();
            //Truy vấn dữ liệu từ bảng Books
            var result = cloudTable.ExecuteQuery(query);
            return result;
        }
        public IEnumerable<BookEntity> SearchBooks(string searchTitle)
        {
            //Lấy bảng Book
            TableQuery<BookEntity> query = new TableQuery<BookEntity>();
            //Filter
            //query = query.Where(TableQuery.GenerateFilterCondition("BookTitle", QueryComparisons.Equal, searchTitle));
            //Truy vấn dữ liệu từ bảng Books
            var tableResult = cloudTable.ExecuteQuery(query);
            var result = tableResult.Where(x => x.BookTitle.Contains(searchTitle));
            return result;
        }
        //Xóa bảng Books
        public void DeleteBook(int rowKey, string partitionKey)
        {
            //Lấy Book trong database đưa vào PartitionKey và RowKey
            TableOperation retrieveOperation = TableOperation.Retrieve<BookEntity>(partitionKey, rowKey.ToString());
            TableResult retrieveResult = cloudTable.Execute(retrieveOperation);
            BookEntity deleteBook = (BookEntity)retrieveResult.Result;
            //Xóa book
            TableOperation deleteOperation = TableOperation.Delete(deleteBook);
            cloudTable.Execute(deleteOperation);
        }
        public BookEntity FindBook(int rowKey, string partitionKey)
        {
            //Lấy Book trong database đưa vào PartitionKey và RowKey
            TableOperation retrieveOperation = TableOperation.Retrieve<BookEntity>(partitionKey, rowKey.ToString());
            TableResult retrieveResult = cloudTable.Execute(retrieveOperation);
            BookEntity book = (BookEntity)retrieveResult.Result;
            return book;
        }
        //Thêm Book
        public void AddBook (BookEntity newBook)
        {
            TableOperation insertOperation = TableOperation.Insert(newBook);
            cloudTable.Execute(insertOperation);
        }
        //Thêm Book
        public void Update(BookEntity newBook)
        {
            newBook.ETag = "*";
            TableOperation insertOperation = TableOperation.Replace(newBook);
            cloudTable.Execute(insertOperation);
        }
        public string ImageURL(HttpPostedFileBase imageUpload)
        {
            //Nếu có tập tin được upload
            if (imageUpload != null)
            {
                //Kiểm tra có phải là hình ảnh không
                if (imageUpload.ContentType.Contains("image"))
                {
                    //Kiểm tra số byte của hình được upload
                    if (imageUpload.ContentLength > 0)
                    {
                        //Kiểm sa nếu ảnh lớn hơn 4mb
                        if (imageUpload.ContentLength > 4 * 1048576)
                        {
                            return "File upload(" + ((double)imageUpload.ContentLength / 1048576).ToString("0.00") + ") is not larger than 4MB.";
                        }
                        else
                        {
                            CloudBlobContainer blobContainer = BlobStorageService.GetCloudBlobContainer();
                            //Khai báo Blob với tên hình được upload
                            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(imageUpload.FileName);
                            //Lưu trữ hình vào blob
                            blob.UploadFromStream(imageUpload.InputStream);
                            //Lấy đường dẫn Blob
                            var bUri = blob.Uri.AbsoluteUri;
                            return bUri;
                        }

                    }
                }
                else
                {
                   return "File upload must be image.";
                }

            }
            else
            {
                return "No file is selected";
            }

            return null;
        }
    }
}