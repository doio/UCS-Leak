using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;//for BinaryReader and File
using System.Diagnostics;//for Debug.WriteLine
using System.Drawing.Imaging;//for pixelFormat
using static System.Environment;//for special paths (like myDocuments,...)

namespace KosmoSC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //var newbytes = reader.ReadBytes(50); // 50 empty bytes
            //Debug.WriteLine("empty: " + BitConverter.ToString(newbytes));
        }
        BinaryReader reader;
        BinaryReader texReader;
        string SCfileNameInfo = "";
        string SCfileNameTex = "";

        private void BtnSCfileInfo_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog()
            {
                Title = "Open SC File",
                Filter = "SC files|*.sc",
                InitialDirectory = @"C:\"
            };
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                SCfileNameInfo = theDialog.FileName;
                this.Text = SCfileNameInfo;
                //var reader = new BinaryReader(File.OpenRead(_fileName)
                reader = new BinaryReader(File.OpenRead(SCfileNameInfo));
            }
        }

        private void BtnSCfileTexture_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog()
            {
                Title = "Open SC Texture File",
                Filter = "SC files|*.sc",
                InitialDirectory = @"C:\"
            };
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                SCfileNameTex = theDialog.FileName;
                this.Text = SCfileNameTex;
                //var texReader = new BinaryReader(File.OpenRead(fileName.Replace(".sc", "_tex.sc")));
                texReader = new BinaryReader(File.OpenRead(SCfileNameTex));
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            //SCfileInfo(reader);
            SCfileTexture(texReader);
        }

        public void SCfileInfo(BinaryReader reader)
        {
            var shapeCount = reader.ReadUInt16();
            var movieClipCount = reader.ReadUInt16();
            var textureCount = reader.ReadUInt16();
            var textFieldCount = reader.ReadUInt16();
            var matrixCount = reader.ReadUInt16();
            var colorTransformCount = reader.ReadUInt16();

            Debug.WriteLine("ShapeCount: " + shapeCount);
            Debug.WriteLine("MovieClipCount: " + movieClipCount);
            Debug.WriteLine("TextureCount: " + textureCount);
            Debug.WriteLine("TextFieldCount: " + textFieldCount);
            Debug.WriteLine("Matrix2x3Count: " + matrixCount);
            Debug.WriteLine("ColorTransformCount: " + colorTransformCount);

            //long timelineOffset = reader.BaseStream.Position;//12 bytes
            //Debug.WriteLine("TimelineOffset: " + timelineOffset);

            var emptybytes = reader.ReadBytes(5); // 5 empty bytes
            Debug.WriteLine("empty: " + BitConverter.ToString(emptybytes));

            //var exportStartOffset = reader.BaseStream.Position;
            var exportCount = reader.ReadUInt16();
            Debug.WriteLine("ExportCount: " + exportCount);

            // Reads the Export IDs.
            for (int i = 0; i < exportCount; i++)
            {
                var exportID = reader.ReadInt16();
                Debug.WriteLine("ExportID: " + exportID);
            }

            // Reads the Export names.
            for (int i = 0; i < exportCount; i++)
            {
                var nameLength = reader.ReadByte();
                var exportName = Encoding.UTF8.GetString(reader.ReadBytes(nameLength));
                Debug.WriteLine("ExportName: " + exportName);
            }
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                var dataBlockTag = reader.ReadByte().ToString("X2");//Block type
                Debug.WriteLine("dataBlockTag: " + dataBlockTag);
                var dataBlockSize = reader.ReadUInt32();//Size in bytes
                Debug.WriteLine("dataBlockSize: " + dataBlockSize);
                //var dataBlockContent = reader.ReadBytes(Convert.ToInt32(dataBlockSize));
                //17 = empty?
                //1A = empty?
                //08 - 24 = A504 0000
                if (dataBlockTag == "01")
                {
                    var pixelFormat = reader.ReadByte();
                    var width = reader.ReadUInt16();
                    var height = reader.ReadUInt16();
                    Debug.WriteLine("\t pixelFormat: " + pixelFormat);
                    Debug.WriteLine("\t width: " + width);
                    Debug.WriteLine("\t height: " + height);
                    //pictureBox1.Size = new Size(width, height);
                    //pictureBox1.BackColor = Color.Red;
                }
                else if (dataBlockTag == "08")
                {
                    //Matrix2x3
                    //2x3 affine transform matrix
                    //1 , 2 , 3
                    //a , b , c
                    var Multiply2x3 = 6;
                    for (int i = 0; i < Multiply2x3; i++)
                    {
                        var matrixVal = reader.ReadUInt32();//unkown1 = mostly 00 (FE) //1
                        Debug.WriteLine("\t matrixVal: " + matrixVal);
                    }
                    //a  = m[0]/1024, = W (width)  , reflect/mirror about y axis = -
                    //b  = m[1]/1024, = to skew/shear?
                    //c  = m[2]/1024, = to skew/shear?
                    //d  = m[3]/1024, = H (height) , reflect/mirror about x axis = -
                    //tx = m[4]/-20,  = x pos translation
                    //ty = m[5]/-20   = y pos translation
                    //var dataBlockContent = reader.ReadBytes(Convert.ToInt32(dataBlockSize-2));
                    //Debug.WriteLine("next data: " + BitConverter.ToString(dataBlockContent));
                }
                else if (dataBlockTag == "12")
                {
                    var shapeID = reader.ReadUInt16();//sprite ID / shape nr
                    var polygonCount = reader.ReadUInt16();//nomber of polygon / regions to cut
                    var totalPointsCount = reader.ReadUInt16();//nr of total points (sum of polygon-points in this group) / total number of points that describe the regions
                    Debug.WriteLine("\t shapeID: " + shapeID);
                    Debug.WriteLine("\t polygonCount: " + polygonCount);
                    Debug.WriteLine("\t totalPointsCount: " + totalPointsCount);
                    //var dataBlockContent = reader.ReadBytes(Convert.ToInt32(dataBlockSize-6));
                    //Debug.WriteLine("next data: " + BitConverter.ToString(dataBlockContent));
                }
                else if (dataBlockTag == "16")
                {
                    //ShapeDrawBitmapCommand? 'texture', 'xys'/-20 , 'uvs
                    var vCountBytes = reader.ReadBytes(2);
                    var VertexCount = BitConverter.ToInt16(vCountBytes.Reverse().ToArray(), 0);//nr of points
                    Debug.WriteLine("\t VertexCount: " + VertexCount);
                    var dataBlockContent = reader.ReadBytes(Convert.ToInt32(dataBlockSize - 2));
                    //Debug.WriteLine("next data: " + BitConverter.ToString(dataBlockContent));
                }
                else if (dataBlockTag == "07")
                {
                    //Font ?
                    var fontID = reader.ReadUInt16();
                    var fontName = reader.ReadString();
                    var fontNameSize = fontName.Length * sizeof(Byte);
                    Debug.WriteLine("\t fontID: " + fontID);
                    Debug.WriteLine("\t fontName: " + fontName);
                    var dataBlockContent = reader.ReadBytes(Convert.ToInt32(dataBlockSize - 2 - 1 - fontNameSize));
                    Debug.WriteLine("\t next data: " + BitConverter.ToString(dataBlockContent));
                }
                else if (dataBlockTag == "0F")
                {
                    //Font ?
                    var fontID = reader.ReadUInt16();
                    var fontName = reader.ReadString();
                    var fontNameSize = fontName.Length * sizeof(Byte);
                    Debug.WriteLine("\t fontID: " + fontID);
                    Debug.WriteLine("\t fontName: " + fontName);
                    var dataBlockContent = reader.ReadBytes(Convert.ToInt32(dataBlockSize - 2 - 1 - fontNameSize));
                    Debug.WriteLine("\t next data: " + BitConverter.ToString(dataBlockContent));
                }
                else if (dataBlockTag == "0C")
                {
                    var clipID = reader.ReadUInt16();
                    var clipFPS = reader.ReadByte();//FrameRate in frames per second
                    var clipFrameCount = reader.ReadUInt16();//nr of 0b parts/blocks
                    Debug.WriteLine("\t clipID: " + clipID);
                    Debug.WriteLine("\t clipFPS: " + clipFPS);
                    Debug.WriteLine("\t clipFrameCount: " + clipFrameCount);
                    var cnt1 = reader.ReadInt32();//nr on end part frame (0b)?
                    Debug.WriteLine("\t cnt1: " + cnt1);//number of sprites/frames?
                    var cntOffset = cnt1 * 6;
                    for (int i = 0; i < cnt1; i++)
                    {
                        var sa10 = reader.ReadUInt16();//following nr (start from 0(1-1))(max5?no max = cnt2(-1))
                                                       //var sa11 = reader.ReadUInt16();//?counts up with 1?  (shapeID?no)
                                                       //var sa12 = reader.ReadUInt16();//islooping? always(65535 = -1)?no (66,67,..)
                                                       //Debug.WriteLine("\t cnt1 -> sa1: " + sa10 + "-" + sa11 + "-" + sa12);//9
                        Debug.WriteLine("\t cnt1 -> sa1: " + sa10);//9
                        var saColorMaybe = reader.ReadBytes(4); // 4 bytes , 2+2 bytes?
                                                                //1st byte , counts up (from 00) (or others 90,8F ,...)
                                                                //2nd byte , if sa1 = 0 => FF or 00?
                                                                //                  = 1 => counts up (from 00)
                                                                ///1+2 = refer to 0x08 block?
                        //3th byte , if sa1 = 0 => FF or 00?
                        //                  = 1 => counts up (from 01)
                        //4th byte , always 00 or FF?
                        //           00 = always 255 on end? no
                        ///3+4 = refer to 0x09 block?
                        Debug.WriteLine("\t cnt1 -> ColorMaybe: " + BitConverter.ToString(saColorMaybe));
                    }
                    var cnt2 = reader.ReadInt16();
                    Debug.WriteLine("\t cnt2: " + cnt2);//nr of shapes
                    var cntOffset2 = cnt2 * 2 * 2;
                    for (int i = 0; i < cnt2; i++)
                    {
                        var cnt2ShapeID = reader.ReadInt16();//shapeID (for get shape)
                        Debug.WriteLine("\t cnt2 -> shapeID: " + cnt2ShapeID);// reference to 0x12 block (shape)
                    }
                    var unknownData = reader.ReadBytes(Convert.ToInt32(cnt2));
                    Debug.WriteLine("\t cnt2 -> unknown data: " + BitConverter.ToString(unknownData));
                    //mostly 00
                    //but also 08, 
                    for (int i = 0; i < cnt2; i++)
                    {
                        byte stringLength = reader.ReadByte();
                        Debug.WriteLine("\t cnt2 -> strLength: " + stringLength);
                        if (stringLength < 255)
                        {
                            var unk2 = Encoding.UTF8.GetString(reader.ReadBytes(stringLength));
                            Debug.WriteLine("\t cnt2 -> unk2: " + unk2);
                        }
                    }
                    //var clipEmpty4 = reader.ReadBytes(4);//4empty?
                    //Debug.WriteLine("\t clipEmpty4: " + BitConverter.ToString(clipEmpty4));
                    /*var clipUnkownCountB = reader.ReadUInt16();//mostly FFFF, something normalised?
                    var clipUnkownCountB2 = reader.ReadUInt16();//always FFFF or 0000? no
                    Debug.WriteLine("\t clipUnkownCountB: " + clipUnkownCountB);
                    Debug.WriteLine("\t clipUnkownCountB2: " + clipUnkownCountB2);
                    var clipIsLooping = reader.ReadUInt16();//2+2+2=6
                    Debug.WriteLine("\t clipIsLooping: " + clipIsLooping);//1 or 0. IsLooping?*/
                    //1B-00-00-FF
                    //sometimes (with long ones)...01-00
                    //var clipUnkownCountB3 = reader.ReadUInt16();//always FFFF?
                    //Debug.WriteLine("\t clipUnkownCountB3: " + clipUnkownCountB3);//2
                    //var dataBlockContent = reader.ReadBytes(Convert.ToInt32(dataBlockSize - 7 - 4 - 6 - 2 - 13));
                    //var dataBlockContent = reader.ReadBytes(Convert.ToInt32(dataBlockSize - 9 - cntOffset - cntOffset2 - 13));
                    //Debug.WriteLine("\t next data: " + BitConverter.ToString(dataBlockContent));
                    //var clipEndContent = reader.ReadBytes(13);//13end
                    //Debug.WriteLine("\t clipEndContent: " + BitConverter.ToString(clipEndContent));
                    /* 
                        dataBlockTag: 0B
                        dataBlockSize: 4
                             next data: 01-00-01-31
                        dataBlockTag: 0B
                        dataBlockSize: 4
                             next data: 01-00-01-32
                        dataBlockTag: 0B
                        dataBlockSize: 6
                             next data: 04-00-03-79-6F-75
                     */
                }
                else
                {
                    var dataBlockContent = reader.ReadBytes(Convert.ToInt32(dataBlockSize));
                    Debug.WriteLine("\t next data: " + BitConverter.ToString(dataBlockContent));
                }
            }
        }

        public void SCfileTexture(BinaryReader texReader)
        {

            //var newbytes = texReader.ReadBytes(20); // 20 empty bytes
            //Debug.WriteLine("empty: " + BitConverter.ToString(newbytes));
            //(info-ancient) empty: 1C-C1-87-0E-00-00-DF-01-F1-01-00-00-00-00-00-00-00-00-00-00
            //(content)      empty: 1C-25-F5-0C-00-00-BE-01-DC-01-00-00-00-00-00-00-00-00-00-00
            //(loading)      empty: 1C-A9-18-0D-00-00-F3-00-73-03-00-00-00-00-00-00-00-00-00-00
            //(debug)        empty: 01-B9-14-00-00-02-32-00-35-00-3F-33-3F-33-3F-33-3F-33-3F-33

            var packetID = texReader.ReadByte();
            var packetSize = texReader.ReadUInt32();
            var texPixelFormat = texReader.ReadByte();
            var texWidth = texReader.ReadUInt16();
            var texHeight = texReader.ReadUInt16();

            Debug.WriteLine("packetID: " + packetID);
            Debug.WriteLine("packetSize: " + packetSize);
            Debug.WriteLine("texPixelFormat: " + texPixelFormat);
            /*
             * 0 = ImageRgba8888
             * 2 = ImageRgba4444
             * 4 = ImageRgb565
             */
            Debug.WriteLine("texWidth: " + texWidth);
            Debug.WriteLine("texHeight: " + texHeight);
            //texWidth = 32;
            pictureBox1.Size = new Size(texWidth, texHeight);
            /*double scale = 0.5f;
            pictureBox1.ClientSize = new Size(
                (int)(scale * texWidth),
                (int)(scale * texHeight));*/
            pictureBox1.BackColor = Color.GreenYellow;//Color.Red;

            //Bitmap bmp = new Bitmap(texWidth, texHeight, texWidth * 4, PixelFormat.Format32bppArgb,
            //           GCHandle.Alloc(pixels, GCHandleType.Pinned).AddrOfPinnedObject());
            Bitmap bmp = new Bitmap(texWidth, texHeight, PixelFormat.Format32bppArgb);

            var modTexWidth = texWidth % 32;//rest (modulo)
            var timesTexWidth = (texWidth - modTexWidth) / 32;

            var modTexHeight = texHeight % 32;//rest (modulo)
            var timesTexHeight = (texHeight - modTexHeight) / 32;

            Color[,] pixelArray = new Color[texHeight,texWidth];
            //make data
            for (int n = 0; n < (timesTexHeight+1); n++)
            {
                int islastline = 0;
                int lineheight = 32;
                if (n == timesTexHeight)
                {
                    lineheight = modTexHeight;
                    islastline = 1;
                }
                int pxOffset = 0;
                int pxOffsetY = 0;
                for (int t = 0; t < timesTexWidth; t++)
                {
                    for (int y = 0; y < lineheight; y++)
                    {
                        for (int x = 0; x < 32; x++)
                        {
                            //pixelArray[y, x+t*32] = Color.Black;
                            pxOffset = t * 32;
                            pxOffsetY = n * 32;//+ islastline*lineheight;
                            pixelArray[y + pxOffsetY, x + pxOffset] = GetColorForPixelFormat(texReader, texPixelFormat);
                        }
                    }
                }
                for (int y = 0; y < lineheight; y++)
                {
                    for (int x = 0; x < modTexWidth; x++)
                    {
                        //pixelArray[y, x+t*32] = Color.Black;
                        int pxOffset2 = timesTexWidth * 32;
                        int pxOffsetY2 = n * 32;//+ islastline * lineheight;
                        pixelArray[y+pxOffsetY2, x + pxOffset2] = GetColorForPixelFormat(texReader, texPixelFormat);
                    }
                }
            }
            //data show
            for (int row = 0; row < pixelArray.GetLength(0); row++)
            {
                for (int col = 0; col < pixelArray.GetLength(1); col++)
                {
                    //Color pxColor = GetColorForPixelFormat(texReader, texPixelFormat);
                    Color pxColor = pixelArray[row, col];
                    bmp.SetPixel(col, row, pxColor);
                }
            }

            pictureBox1.Image = bmp;
            string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocume‌​nts) + "\\" + Path.GetFileNameWithoutExtension(SCfileNameTex) + ".png";
            bmp.Save(FilePath);

        }
        public Color GetColorForPixelFormat(BinaryReader texReader, int texPixelFormat)
        {
            Color pxColor = Color.Red;
            if (texPixelFormat == 0)
            {
                byte r = texReader.ReadByte();
                byte g = texReader.ReadByte();
                byte b = texReader.ReadByte();
                byte a = texReader.ReadByte();
                pxColor = Color.FromArgb((int)((a << 24) | (r << 16) | (g << 8) | b));
            }
            if (texPixelFormat == 2)
            {
                ushort color = texReader.ReadUInt16();
                int red = (int)((color >> 12) & 0xF) << 4;
                int green = (int)((color >> 8) & 0xF) << 4;
                int blue = (int)((color >> 4) & 0xF) << 4;
                int alpha = (int)(color & 0xF) << 4;
                pxColor = Color.FromArgb(alpha, red, green, blue);
            }
            if (texPixelFormat == 4)
            {
                ushort color = texReader.ReadUInt16();
                int red = (int)((color >> 11) & 0x1F) << 3;
                int green = (int)((color >> 5) & 0x3F) << 2;
                int blue = (int)(color & 0X1F) << 3;
                pxColor = Color.FromArgb(red, green, blue);
            }
            return pxColor;
        }
    }
}
