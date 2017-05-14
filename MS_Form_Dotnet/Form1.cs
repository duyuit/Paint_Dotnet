using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MS_Form_Dotnet
{

    public partial class Form1 : Form
    {
        private Point _p1 = new Point();//Point mouse down
        private Point _p2 = new Point();//Point mouse up
        private bool _isDown; //Check mouse is clicking
        private Bitmap _bm;//Bitmap to draw
        private Graphics _gp;//Graphic supply draw function
        private Pen penleft = new Pen(Color.Red);//Pen contain left color
        private Pen penright = new Pen(Color.Black);//Pen contain right color
        private List<Bitmap> stack = new List<Bitmap>();//List bitmap use for undo,redo
        int position = 0;//Postion of current Bitmap in stack;
        string shape;//What shape user wanting draw(normal,line,rectangle,elip)
        int flagText = 0;//Flag to check DrawText using
        private Point temp;//Point temp to use in draw normal line
        private Font myfont;//Font for draw text

        private Bitmap bmPic;
        private Graphics gpPic;
        private bool _isDownPic;
        private Point _pic;

        private string Shapetemp;
        public Form1()
        {
            InitializeComponent();
            tabHome.Select();
            shape = "normal";//Set start shape is normal(pencil)
            this.MouseDown += Form1_MouseDown; ;
            this.MouseMove += Form1_MouseMove;
            this.MouseUp += Form1_MouseUp;
            this.Paint += Form1_Paint;

            penleft.Color = btnLeft.SelectedColor;
            penright.Color =btnRight.SelectedColor;// Set start color penright
            _bm = new Bitmap(this.Width, this.Height);//Create bitmap to draw
            stack.Add((Bitmap)_bm.Clone());//Add first bitmap to stack
            _gp = Graphics.FromImage(_bm);//Draw from _bm

            this.SetStyle(ControlStyles.UserPaint, true);//Code for smooth draw
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);//Code for smooth draw
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);//Code for smooth draw
            bmPic = new Bitmap(this.Width, this.Height);
            gpPic = Graphics.FromImage(bmPic);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (_isDown)
            {
                _gp = Graphics.FromImage(_bm);
                SolidBrush sb = new SolidBrush(penright.Color);//Make brush to fill shape
                int x = _p2.X - _p1.X;//Width of shape
                int y = _p2.Y - _p1.Y;//Height of shape
                if (ckbFill.Checked) //Fill checkbox is checked?
                {
                    switch (shape)
                    {
                        case "tron":
                            e.Graphics.FillEllipse(sb, _p1.X, _p1.Y, x, y); break;//Fill elip
                        case "vuong":
                            e.Graphics.FillRectangle(sb, _p1.X, _p1.Y, x, y);
                            break;//Fill rectangle
                        case "thoi"://Fill hinh thoi
                            int leng = _p2.X + _p1.X;
                            int leng2 = _p2.X - _p1.X;
                            if (leng2 < 0) leng2 *= -1;
                            Point x1 = new Point(leng / 2, _p2.Y - leng2);
                            Point x2 = new Point(leng / 2, _p2.Y + leng2);
                            Point x3 = new Point(_p2.X, _p1.Y);
                            Point[] hoi = new Point[] { _p1, x1, x3, x2 };
                            e.Graphics.FillPolygon(sb, hoi);
                            break;
                        case "line":
                            e.Graphics.DrawLine(penleft, _p1, _p2); //Draw straight line
                            break;
                        case "normal":
                            _gp.DrawLine(penleft, temp, _p2); break; ;//Draw normal(pencil)
                        case "text"://Make border for Text box to insert
                            e.Graphics.DrawRectangle(penleft, _p1.X, _p1.Y, x, y);
                            break;
                    }
                }
                else
                {
                    switch (shape)
                    {

                        case "tron":
                            e.Graphics.DrawEllipse(penleft, _p1.X, _p1.Y, x, y);//Draw elip
                            break;
                        case "vuong":   //Draw rectangle 
                            e.Graphics.DrawRectangle(penleft, _p1.X, _p1.Y, x, y);
                            break;
                        case "thoi"://Draw hinh thoi
                            int leng = _p2.X + _p1.X;
                            int leng2 = _p2.X - _p1.X;
                            if (leng2 < 0) leng2 *= -1;
                            Point x1 = new Point(leng / 2, _p1.Y - leng2);
                            Point x2 = new Point(leng / 2, _p1.Y + leng2);
                            Point x3 = new Point(_p2.X, _p1.Y);
                            Point[] hoi = new Point[] { _p1, x1, x3, x2 };
                            e.Graphics.DrawPolygon(penleft, hoi);
                            break;
                        case "line": e.Graphics.DrawLine(penleft, _p1, _p2); break;
                        case "normal": _gp.DrawLine(penleft, temp, _p2); break;
                        case "text":
                            e.Graphics.DrawRectangle(penleft, _p1.X, _p1.Y, x, y);
                            break;
                    }
                }
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {
                SolidBrush sb = new SolidBrush(penright.Color);
                int x = _p2.X - _p1.X;
                int y = _p2.Y - _p1.Y;
                if (x <= 0 || y <= 0)
                    x *= -1;
                if (y <= 0)
                    y *= -1;
                if (x > 0 && y > 0)
                    bmPic = new Bitmap(x + 1, y + 1);
                gpPic = Graphics.FromImage(bmPic);
                ptb.Visible = true;
                ptb.Width = x + 3;
                ptb.Height = y + 3;
                ptb.Location = _p1;

                if (ckbFill.Checked)
                {
                    switch (shape)
                    {
                        case "tron":
                            gpPic.FillEllipse(sb, 0, 0, x, y);
                            break;
                        case "vuong":
                            gpPic.FillRectangle(sb,0, 0, x, y); break;
                        case "thoi":
                            int leng = _p2.X + _p1.X;
                            int leng2 = _p2.X - _p1.X;
                            if (leng2 > 0)
                                bmPic = new Bitmap(leng2 + 1, 2 * leng2);
                            gpPic = Graphics.FromImage(bmPic);
                            ptb.Location = new Point(_p1.X, _p1.Y - leng2);
                            ptb.Width = leng2 + 1;
                            ptb.Height = 2 * leng2;
                            Point p1New = new Point(0, ptb.Height / 2);
                            Point p2New = new Point(ptb.Width - 1, ptb.Height / 2);
                            leng = p2New.X + p1New.X;
                            leng2 = p2New.X - p1New.X;
                            if (leng2 < 0) leng2 *= -1;
                            Point x1 = new Point(leng / 2, p1New.Y - leng2);
                            Point x2 = new Point(leng / 2, p1New.Y + leng2);
                            Point x3 = new Point(p2New.X, p1New.Y);
                            Point[] thoi = new Point[] { p1New, x1, x3, x2 };
                            gpPic.FillPolygon(sb, thoi);
                            break;
                        case "line": gpPic.DrawLine(penleft, new Point(0, 0), new Point(ptb.Width, ptb.Height)); break;
                        case "normal":
                            AddStack();//_gp.DrawLine(penleft, temp, _p2);
                            ptb.Visible = false; break;
                        case "text":  //Set position,properties for Text box    
                           
                            tabText.Select();
                            ptb.Visible = false;
                            flagText = 1;
                            txtText.ForeColor = penleft.Color;
                            txtText.Location = new Point(_p1.X, _p1.Y);
                            txtText.Height = y;
                            txtText.Width = x;
                            txtText.Visible = true;
                            txtText.Focus();
                            break;
                    }
                }
                else
                {
                    switch (shape)
                    {
                        case "tron":
                            gpPic.DrawEllipse(penleft, 0, 0, x, y);
                            break;
                        case "vuong":
                            gpPic.DrawRectangle(penleft, 0, 0, x, y);
                            break;
                        case "thoi":
                            int leng = _p2.X + _p1.X;
                            int leng2 = _p2.X - _p1.X;
                            if (leng2 > 0)
                                bmPic = new Bitmap(leng2 + 1, 2 * leng2);
                            gpPic = Graphics.FromImage(bmPic);
                            ptb.Location = new Point(_p1.X, _p1.Y - leng2);
                            ptb.Width = leng2 + 1;
                            ptb.Height = 2 * leng2;
                            Point p1New = new Point(0, ptb.Height / 2);
                            Point p2New = new Point(ptb.Width - 1, ptb.Height / 2);
                            leng = p2New.X + p1New.X;
                            leng2 = p2New.X - p1New.X;
                            if (leng2 < 0) leng2 *= -1;
                            Point x1 = new Point(leng / 2, p1New.Y - leng2);
                            Point x2 = new Point(leng / 2, p1New.Y + leng2);
                            Point x3 = new Point(p2New.X, p1New.Y);
                            Point[] thoi = new Point[] { p1New, x1, x3, x2 };
                            gpPic.DrawPolygon(penleft, thoi);
                            break;
                        case "line": gpPic.DrawLine(penleft, new Point(0, 0), new Point(ptb.Width, ptb.Height)); break;
                        case "normal":
                            AddStack();//gp.DrawLine(penleft,temp,_p2);
                            ptb.Visible = false; break;
                        case "text":
                           
                            tabText.Select();
                            ptb.Visible = false;
                            flagText = 1;
                            txtText.ForeColor = penleft.Color;
                            txtText.Location = new Point(_p1.X, _p1.Y);
                            txtText.Height = y;
                            txtText.Width = x;
                            txtText.Visible = true;
                            txtText.Focus();
                            break;
                    } 
                }
               
            }
            ptb.Image = (Bitmap)bmPic.Clone();
            ptb.Focus();

            Shapetemp = shape;
            _isDown = false;
            temp.X = e.Location.X;//Set location for Point temp use to draw normal(pencil)
            temp.Y = e.Location.Y;
            _p2.X = e.Location.X;//Set location for Point p2 use to draw shape
            _p2.Y = e.Location.Y;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {
                this.BackgroundImage = (Bitmap)_bm.Clone();
                if (shape.Equals("normal")) //Update point temp use to draw normal(pencil)
                {
                    temp.X = _p2.X;
                    temp.Y = _p2.Y;
                    _p2.X = e.Location.X;
                    _p2.Y = e.Location.Y;
                    this.Refresh();
                }
                else
                {
                    _p2.X = e.Location.X;
                    _p2.Y = e.Location.Y;
                    this.Refresh();
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (flagText == 1)//Update when change focus out of text box,draw string on text box
            {
                ptb.Visible = true;
                int x = _p2.X - _p1.X;
                int y = _p2.Y - _p1.Y;
                RectangleF rectF1 = new RectangleF(_p1.X, _p1.Y, x, y);
                SolidBrush sb = new SolidBrush(penleft.Color);
                _gp.DrawString(txtText.Text, myfont, sb, Rectangle.Round(rectF1));
                _gp.Dispose();
                txtText.Clear();
                txtText.Visible = false;
                flagText = 0;
            }

            if (shape == "do")//Check user are using fill Color,if true->fill shape with left color
            {
                int x = e.Location.X;
                int y = e.Location.Y;
                try
                {
                    Color color = _bm.GetPixel(x, y);
                    ToMau(new Point(x, y), color);
                    this.BackgroundImage = (Bitmap)_bm.Clone();
                    AddStack();
                }
                catch { };
            }
            else
            {
                if (ptb.Focused)
                    if (ptb.Width > 5 && ptb.Height > 5)
                    {
                        Draw(Shapetemp, ptb.Location, new Point(ptb.Location.X + ptb.Width, ptb.Location.Y + ptb.Height), ptb.Width, ptb.Height, ckbFill.Checked);
                        ptb.Visible = false;
                        ptb.Location = new Point(0, 0);
                    }
            }

            _p1 = new Point(e.Location.X, e.Location.Y);
            _p2 = new Point(e.Location.X, e.Location.Y);
            temp.X = e.Location.X;
            temp.Y = e.Location.Y;
            _isDown = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnLeft.SelectedColor = Color.Red;
            btnRight.SelectedColor = Color.Black;
            InitComboBoxSize();//Add item to combo box Size of line to draw shape
            for (int i = 10; i <= 100; i += 10)//Add item to combo box Size of text to draw string
            {
                cmbTextSize.Items.Add(i);
            }
            cmbTextSize.SelectedIndex = 4;
            foreach (FontFamily font in System.Drawing.FontFamily.Families)//Add item to combo box Font of text to draw string
            {
                cmbFont.Items.Add(font.Name);
            }
            cmbFont.SelectedIndex = 0;

        }

  
        private void AddStack() //ADD currently bitmap to stack to undo,redo
        {
            if (stack.Count > 10) //Check stack have more 10bitmap, remove for storage
            {
                stack.RemoveAt(0);
                stack.Add((Bitmap)_bm.Clone());
                position = 10;
            }

            if (stack.Count <= 10)//Add bitmap to stack after draw shape
            {
                stack.Add((Bitmap)_bm.Clone());
                position++;
            }
            if (position < stack.Count - 1)//If change something on bitmap,delete all bitmap after this on stack
            {
                stack.RemoveRange(position + 1, stack.Count - 2 - position);
            }
        }
        private void Draw(String shape, Point p1, Point p2, int width, int height, bool fill) //Draw to bitmap
        {
            SolidBrush sb = new SolidBrush(penright.Color);
            if (fill)
            {
                switch (shape)
                {
                    case "tron":
                        _gp.FillEllipse(sb, p1.X, p1.Y, width, height);
                        btnElip.Focus();
                        break;
                    case "vuong":
                        _gp.FillRectangle(sb, p1.X, p1.Y, width, height);
                        btnRec.Focus();
                        break;
                    case "thoi":
                        p1.Y = p1.Y + height / 2;
                        p2.Y = p2.Y - height / 2;
                        int leng = p2.X + p1.X;
                        int leng2 = p2.X - p1.X;
                        if (leng2 < 0) leng2 *= -1;
                        Point x1 = new Point(leng / 2, p2.Y - leng2);
                        Point x2 = new Point(leng / 2, p2.Y + leng2);
                        Point x3 = new Point(p2.X, p1.Y);
                        Point[] hoi = new Point[] { p1, x1, x3, x2 };
                        _gp.FillPolygon(sb, hoi);
                        break;
                    case "line": _gp.DrawLine(penleft, p1, p2); break;
                    //case "normal": _gp.DrawLine(penleft, temp, p2); break;
                    case "text":  //Set position,properties for Text box         
                        flagText = 1;
                        txtText.ForeColor = penleft.Color;
                        txtText.Location = new Point(p1.X, p1.Y);
                        txtText.Height = height;
                        txtText.Width = width;
                        txtText.Visible = true;
                        txtText.Focus();
                        break;
                }
            }
            else
            {
                switch (shape)
                {
                    case "tron":
                        _gp.DrawEllipse(penleft, p1.X, p1.Y, width, height);
                        btnElip.Focus();
                        break;
                    case "vuong":
                        _gp.DrawRectangle(penleft, p1.X, p1.Y, width, height);
                        btnRec.Focus();
                        break;
                    case "thoi":
                        p1.Y = p1.Y + height / 2;
                        p2.Y = p2.Y - height / 2;
                        int leng = p2.X + p1.X;
                        int leng2 = p2.X - p1.X;
                        if (leng2 < 0) leng2 *= -1;
                        Point x1 = new Point(leng / 2, p2.Y - leng2);
                        Point x2 = new Point(leng / 2, p2.Y + leng2);
                        Point x3 = new Point(p2.X, p1.Y);
                        Point[] hoi = new Point[] { p1, x1, x3, x2 };
                        _gp.DrawPolygon(penleft, hoi);
                        break;
                    case "line": _gp.DrawLine(penleft, p1, p2); break;
                    //case "normal": ptb.Visible = false; break;
                    case "text":
            
                        flagText = 1;
                        txtText.ForeColor = penleft.Color;
                        txtText.Location = new Point(p1.X, p1.Y);
                        txtText.Height = height;
                        txtText.Width = width;
                        txtText.Visible = true;
                        txtText.Focus();
                        break;
                }
            }
            AddStack();
            //Refresh();
        }
        private void ToMau(Point p, Color colortemp)//Use to fill Shape with colortemp
        {
            int x = p.X;
            int y = p.Y;
            Stack<Point> S = new Stack<Point>();
            System.Drawing.Color OriColor = _bm.GetPixel(x, y);
            _bm.SetPixel(x, y, penleft.Color);
            S.Push(p);
            while (S.Count != 0)
            {
                p = S.Pop();
                if ((p.X - 1 >= 0) && OriColor == _bm.GetPixel(p.X - 1, p.Y))
                {
                    _bm.SetPixel(p.X - 1, p.Y, penleft.Color);
                    S.Push(new System.Drawing.Point(p.X - 1, p.Y));
                }
                if ((p.X + 1 < _bm.Width) && OriColor == _bm.GetPixel(p.X + 1, p.Y))
                {
                    _bm.SetPixel(p.X + 1, p.Y, penleft.Color);
                    S.Push(new System.Drawing.Point(p.X + 1, p.Y));
                }
                if ((p.Y - 1 >= 0) && OriColor == _bm.GetPixel(p.X, p.Y - 1))
                {
                    _bm.SetPixel(p.X, p.Y - 1, penleft.Color);
                    S.Push(new System.Drawing.Point(p.X, p.Y - 1));
                }
                if ((p.Y + 1 < _bm.Height) && OriColor == _bm.GetPixel(p.X, p.Y + 1))
                {
                    _bm.SetPixel(p.X, p.Y + 1, penleft.Color);
                    S.Push(new System.Drawing.Point(p.X, p.Y + 1));
                }
            }

        }
        private void InitComboBoxSize()//Add item to combo box Size of line to draw shape
        {
            for (int i = 1; i <= 10; i++)
            {
                cmbSize.Items.Add(i);
            }
            cmbSize.SelectedIndex = 0;
        }

        private void cmbSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            penleft.Width = int.Parse(cmbSize.Text);
        }

        private void cmbSize_TextUpdate(object sender, EventArgs e)
        {
            if (cmbSize.Text != "")
            {
                penleft.Width = int.Parse(cmbSize.Text);
            }
            else
            {
                penleft.Width = 1;
                cmbSize.Text = 1 + "";
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            penleft.Color = Color.White;
            PictureBox pb = new PictureBox() { Image = global::MS_Form_Dotnet.Properties.Resources.Designcontest_Outline_Eraser };
            this.Cursor = new Cursor(((Bitmap)pb.Image).GetHicon());
            shape = "normal";
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (position > 0)
                try
                {
                    this.BackgroundImage = (Bitmap)stack[--position].Clone();

                    _bm = (Bitmap)this.BackgroundImage.Clone();
                }
                catch { };
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            if (position < stack.Count - 1)
                try
                {

                    this.BackgroundImage = (Bitmap)stack[++position].Clone();

                    _bm = (Bitmap)this.BackgroundImage.Clone();
                }
                catch { };
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            shape = "line";
            penleft.Color = btnLeft.SelectedColor;
            this.Cursor = Cursors.Cross;
        }

        private void btnRec_Click(object sender, EventArgs e)
        {
            shape = "vuong";
            penleft.Color = btnLeft.SelectedColor;
            this.Cursor = Cursors.Cross;
        }

        private void btnThoi_Click(object sender, EventArgs e)
        {
            shape = "thoi";
            penleft.Color = btnLeft.SelectedColor;
            this.Cursor = Cursors.Cross;
        }

        private void btnElip_Click(object sender, EventArgs e)
        {
            shape = "tron";
            penleft.Color = btnLeft.SelectedColor;
            this.Cursor = Cursors.Cross;
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            shape = "normal";
            PictureBox pb = new PictureBox() { Image = global::MS_Form_Dotnet.Properties.Resources.pen };
            this.Cursor = new Cursor((new Bitmap(pb.Image,new Size(30,40)).GetHicon()));
            penleft.Color = btnLeft.SelectedColor;
        }

        private void btnFill_Click(object sender, EventArgs e)
        {
            shape = "do";
            PictureBox pb = new PictureBox() { Image = global::MS_Form_Dotnet.Properties.Resources.fill };
            this.Cursor = new Cursor((new Bitmap(pb.Image, new Size(30, 40)).GetHicon()));
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            _bm = new Bitmap(_bm, this.Width, this.Height);
        }

        private void btnText_Click(object sender, EventArgs e)
        {
            PictureBox pb = new PictureBox() { Image = global::MS_Form_Dotnet.Properties.Resources.font_png_image_23842 };
            shape = "text";
            this.Cursor = new Cursor((new Bitmap(pb.Image, new Size(20, 20)).GetHicon()));
        }

    

       

        private void ptb_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDownPic)
            {
                ptb.Top = e.Location.Y + ptb.Top - _pic.Y;
                ptb.Left = e.Location.X + ptb.Left - _pic.X;
            }
            Refresh();
        }

        private void ptb_MouseDown(object sender, MouseEventArgs e)
        {
            _isDownPic = true;
            _pic = e.Location;
        }

        private void ptb_MouseUp(object sender, MouseEventArgs e)
        {
            _isDownPic = false;
        }

        private void btnLeft_SelectedColorChanged(object sender, EventArgs e)
        {
            penleft.Color = btnLeft.SelectedColor;
            txtText.ForeColor = penleft.Color;

        }

        private void btnRight_SelectedColorChanged(object sender, EventArgs e)
        {
            penright.Color = btnRight.SelectedColor;
        }

        private void cmbFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            int size = int.Parse(cmbTextSize.Text);
            myfont = new Font(cmbFont.Text, size);
            txtText.Font = myfont;
        }

        private void cmbFont_TextUpdate(object sender, EventArgs e)
        {
            myfont = new Font(cmbFont.Text, Int32.Parse(cmbTextSize.Text));
            txtText.Font = myfont;
        }

        private void cmbTextSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int size = int.Parse(cmbTextSize.Text);
            myfont = new Font(cmbFont.Text, size);
            txtText.Font = myfont;
        }

        private void cmbTextSize_TextUpdate(object sender, EventArgs e)
        {
            if (cmbTextSize.Text != "")
            {
                int size = int.Parse(cmbTextSize.Text);
                myfont = new Font(cmbFont.Text, size);
                txtText.Font = myfont;
            }
            else
            {
                cmbTextSize.Text = "10";
                myfont = new Font(cmbFont.Text, 10);
                txtText.Font = myfont;
            }
        }
    }
}
