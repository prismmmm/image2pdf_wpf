using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace img2pdf
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Image_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, true))
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            var dropFiles = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];
            if (dropFiles == null) return;
            label2.Content = dropFiles[0];
            Convert(dropFiles[0]);
        }


        //image to pdf method
        public void Convert(string dir)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(dir);
            System.IO.FileInfo[] pngfiles = di.GetFiles("*.png", System.IO.SearchOption.AllDirectories);
            System.IO.FileInfo[] jpgfiles = di.GetFiles("*.jpg", System.IO.SearchOption.AllDirectories);

            Console.WriteLine(pngfiles.Length);
            Console.WriteLine(jpgfiles.Length);

            string[] paths = new string[pngfiles.Length + jpgfiles.Length];
            Console.WriteLine(paths.Length);
            var i = -1;
            foreach(var p in pngfiles)
            {
                i++;
                paths[i] = dir + "/" + p.ToString();
           
            }
            foreach (var p in jpgfiles)
            {
                i++;
                paths[i] = dir + "/" + p.ToString();
             
            }


            iTextSharp.text.Rectangle pageSize = null;

            using (var srcImage = new Bitmap(paths[0].ToString()))
            {
                pageSize = new iTextSharp.text.Rectangle(0, 0, srcImage.Width, srcImage.Height);
            }

            using (var ms = new MemoryStream())
            {   
                var document = new iTextSharp.text.Document(pageSize, 0, 0, 0, 0);
                iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms).SetFullCompression();
                document.Open();

                //progress.Maximum = paths.Length;
                //progress.Value = 0;
                var prg = 0;
                foreach (var ii in paths)
                {
                    label.Content = ii;
                    var image = iTextSharp.text.Image.GetInstance(ii.ToString());
                    document.SetPageSize(new iTextSharp.text.Rectangle(0, 0, image.Width, image.Height));
                    document.Add(image);

                    //progress.Value = prg;
                }
                document.Close();

                File.WriteAllBytes(dir + ".pdf", ms.ToArray());
                label.Content = "Conversion complete !";
            }
        }
    }

    
}
