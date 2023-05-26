using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Drawing_pad
{
    public partial class Form1 : Form
    {
        Graphics graph;
        // время 
        int seconds;
        // отвечает за все вычисления 
        int cupx = -1, cupy = -1, centerx = 410, centery = 211;
        double radius = -1, medium, sum = 0, cnt = 0, toch = 80.0;
        int record = 0;
        // цвет ручки
        int alpha = 100, red = 60, green = 255, blue = 0;

        // hardest one - путь ручки
        double k;
        int beginx, beginy;
        // значения отвечающие за начало игры
        bool moving = false, mayp = false;


        Pen pen;
        public Form1()
        {
            InitializeComponent();
            graph = panel1.CreateGraphics();
            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            pen = new Pen(Color.FromArgb(alpha, red, green, blue), 5);
            pen.StartCap = pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
        }

        // останавливает время когда оно истекает
        private void timer1_Tick(object sender, EventArgs e)
        {
            seconds++;
            if(seconds == 10 && moving == true)
            {
                labelres.Text = "Too slow";
                stop(0);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if(mayp == true)
            {
                // начальные параметры
                seconds = 0;
                cupx = e.X;
                beginx = changex(cupx);
                cupy = beginy = e.Y;
                beginy = changey(cupy);
                panel1.Invalidate();
                panel1.Update();

                label6.Visible = label5.Visible = true;
                labelres.Visible = false;

                // узнать можно ли начинать
                calc_rad();
                medium = radius;
                if (radius > 60)
                {
                    moving = true;

                    // сбор данных для заключительного этапа
                    k = Getk();
                }
                else
                {
                    labelres.Text = "Too close to a dot";
                    stop(0);
                }
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if(moving && cupx != -1 && cupy != -1)
            {
                graph.DrawLine(pen, new Point(cupx, cupy), e.Location);
                cupx = e.X;
                cupy = e.Y;
                calc_rad();
                if (radius < 30){
                    labelres.Text = "Too close to dot";
                    stop(0);
                }
                // Меняю цвет ручки за счет вычисления расстояния
                double dif = Math.Abs(radius - medium);
                int x = ((int)dif * 45) / 4;
                if (x <= 195)
                {
                    pen.Color = Color.FromArgb(alpha, red + x, green, blue);
                }
                else
                {
                    pen.Color = Color.FromArgb(alpha, 255, green - Math.Min((x - 195), 255), blue);
                }

                // вычисление процента который отвечает за то насколько верно нарисовал круг пользователь
                cnt += 1.0;
                double percent = (1.0 * 100 * Math.Max(toch - dif, 0.0)) / toch;
                sum += percent;
                give_percent();

                // остановка работы при рисований полного круга

                double localk = Getk();
                label4.Text = beginx.ToString();
                label2.Text = changex(cupx).ToString();
                label3.Text = changey(cupy).ToString();
                label7.Text = beginy.ToString();
                int alx = changex(cupx), aly = changey(cupy);
                if(((beginx >= 0 &&  alx >= 0) || (beginx <= 0 && alx <= 0))
                    && ((beginy >= 0 && aly >= 0) || (beginy <= 0 && aly <= 0))){
                        if (Math.Abs(localk - k) < 0.1 && seconds > 1) stop(1);
                }
            }
        }
        double Getk()
        {
            int difx = changex(cupx), dify = changey(cupy);
            return 1.0 * dify / difx;
        }
        int changex(int num)
        {
            int difx;
            if (num > centerx) difx = num - centerx;
            else difx = -(centerx - num);
            return difx;
        }
        int changey(int num)
        {
            int dify;
            if (num > centery) dify = -(num - centery);
            else dify = (centery - num);
            return dify;
        }
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (moving == true)
            {
                labelres.Text = "Draw a full circle";
                stop(0);
            }
        }

            private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            label1.Visible = false;
            mayp = true;
            pictureBox4.Visible = true;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }
        public void calc_rad()
        {
            radius = Math.Sqrt((Math.Pow(cupx - centerx, 2) + Math.Pow(cupy - centery, 2)));
        }
        void stop(int code)
        {
            if(code == 0)
            {
                label5.Text = "XX";
                label6.Text = "x%";
            }
            else
            {
                int res = (int)(sum / cnt * 10) - 1;
                if (res <= record) labelres.Text = "Best: " + (1.0 * record / 10).ToString() + "%";
                else
                {
                    record = res;
                    labelres.Text = "New best score";
                }
                give_percent();
            }

            labelres.Visible = true;
            moving = false;
            cupx = -1;
            cupy = -1;

            sum = 0;
            cnt = 0;
        }

        void give_percent()
        {
            int res = (int)(sum / cnt * 10) - 1;
            label5.Text = (res / 10).ToString();
            label6.Text = (res % 10).ToString() + "%";
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Originally created my Matt Round");
        }
    }
}
