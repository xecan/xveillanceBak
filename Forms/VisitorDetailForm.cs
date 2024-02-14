using MySql.Data.MySqlClient;
using Neurotec.Images;
using Neurotec.Images.Processing;
using Neurotec.IO;
using Neurotec.Samples.Code;
using Neurotec.Surveillance;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Neurotec.Samples.Forms
{
    public partial class VisitorDetailForm : Form
    {
        #region Constants

        private const int PageSize = 50;
        private const int ImageWidth = 140;
        private const int ImageHeight = 180;

        #endregion


        #region Public constructors

        public VisitorDetailForm()
        {
            InitializeComponent();
            var faces = new FaceRecordCollection("");
            var records = new AllVisitorsCollection();
            Record = records;
            Faces = faces;
            ShowPage();
        }

        #endregion

        #region Public properties

        public FaceRecordCollection Faces { get; set; }
        public AllVisitorsCollection Record { get; set; }
        public NSurveillance Surveillance { get; set; }

        #endregion

        #region Private methods

        private IEnumerable<FaceRecord> GetRecords()
        {
            return Faces;
        }

        public Bitmap GetThumbnail(FaceRecord faceRecord)
        {
            using (var image = Faces.GetThumbnailById(faceRecord.Id))
            using (var rgb = image.PixelFormat != NPixelFormat.Rgb8U ? NImage.FromImage(NPixelFormat.Rgb8U, 0, image) : null)
            {
                var scaleX = (float)ImageWidth / image.Width;
                var scaleY = (float)ImageHeight / image.Height;
                var scale = Math.Min(scaleX, scaleY);
                var dx = (ImageWidth - scale * image.Width) / 2;
                var dy = (ImageHeight - scale * image.Height) / 2;
                using (var dst = NImage.Create(NPixelFormat.Rgb8U, ImageWidth, ImageHeight, 0))
                using (var scaled = Nrgbip.Scale(rgb ?? image, 0, 0, image.Width, image.Height, (uint)(scale * image.Width), (uint)(scale * image.Height), Drawing.Drawing2D.InterpolationMode.Bilinear))
                {
                    scaled.CopyTo(dst, (uint)dx, (uint)dy);
                    return dst.ToBitmap();
                }
            }
        }
        public NImage GetThumbnailById(int id)
        {
            NImage image = null;
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand($"SELECT faceThumbnail FROM recordstable WHERE id = {id}", connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            long fieldSize = reader.GetBytes(0, 0, null, 0, 0);
                            byte[] bytes = new byte[fieldSize];
                            reader.GetBytes(0, 0, bytes, 0, (int)fieldSize);
                            using (MemoryStream stream = new MemoryStream(bytes))
                            {
                                image = NImage.FromMemory(bytes);
                            }
                        }
                    }
                }
            }
            return image;
        }
        public Bitmap GetVisitorsThumbnail(Records faceRecord)
        {
            using (var image = GetThumbnailById(faceRecord.Id))
            using (var rgb = image.PixelFormat != NPixelFormat.Rgb8U ? NImage.FromImage(NPixelFormat.Rgb8U, 0, image) : null)
            {
                var scaleX = (float)ImageWidth / image.Width;
                var scaleY = (float)ImageHeight / image.Height;
                var scale = Math.Min(scaleX, scaleY);
                var dx = (ImageWidth - scale * image.Width) / 2;
                var dy = (ImageHeight - scale * image.Height) / 2;
                using (var dst = NImage.Create(NPixelFormat.Rgb8U, ImageWidth, ImageHeight, 0))
                using (var scaled = Nrgbip.Scale(rgb ?? image, 0, 0, image.Width, image.Height, (uint)(scale * image.Width), (uint)(scale * image.Height), Drawing.Drawing2D.InterpolationMode.Bilinear))
                {
                    scaled.CopyTo(dst, (uint)dx, (uint)dy);
                    return dst.ToBitmap();
                }
            }
        }
        private void ShowPage()
        {
           

            var images = new ImageList()
            {
                ImageSize = new Size(ImageWidth, ImageHeight),
                ColorDepth = ColorDepth.Depth32Bit
            };

           if(SessionManager.IsAllVisitors == true)
            {
                var record = Record.FirstOrDefault(x => x.Id == Convert.ToInt32(SessionManager.FaceId));

                var thumnails = GetVisitorsThumbnail(record);

                images.Images.Add(thumnails);
                listView.LargeImageList = images;

                listView.Items.Add(record.FaceId, 0);
            }
            else
            {
                var faceRecord = GetRecords().FirstOrDefault(x => x.FaceId == SessionManager.FaceId.Trim());

                var thumnails = GetThumbnail(faceRecord);

                images.Images.Add(thumnails);
                listView.LargeImageList = images;

                listView.Items.Add(faceRecord.FaceId, 0);

            }
           

        }

       
        #endregion
    }
}
