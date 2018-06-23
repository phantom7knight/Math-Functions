using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project_Math500
{
    public partial class Form1 : Form
    {
        //initializations
        List<PointF> cordpoints = new List<PointF>();
        int degree = 1;
        const int screen_width = 565;
        const int screen_height= 423;
        const int screenpixeleqX = 520;
        const int screenpixeleqY = 75;
        float[] controlpoints = new float[21];
        float[] controlpoints_copy = new float[21];
        bool point_click = false;
        int index_of_new_point = 0;
        bool isitBB = false;
        bool isitNIL = false;

        public Form1()
        {
            InitializeComponent();
            cordpoints.Add(new Point(0, 1));
            cordpoints.Add(new Point(1, 1));
            controlpoints[0] = 1.0f;
            controlpoints[1] = 1.0f;
        }

        //this is used to convert that weird translation from the window size system to the appropriate pixel system
        private PointF mathtoscreen(PointF cordpoints)
        {
            PointF py = new PointF(0,0);

            py.X = 20;
            py.Y = screen_height / 2;

            py.X = (cordpoints.X * screenpixeleqX + py.X);
            py.Y = (cordpoints.Y * (-screenpixeleqY) + py.Y);
            
            return py;
        }

        //to clear points for the update of the points on the screen

        

        private PointF screentomath(Point cordpoints)
        {
            PointF p1 = new PointF(0, 0);

            p1.X = (cordpoints.X - 20) / (float)screenpixeleqX;
            p1.Y = ((screen_height / 2) - cordpoints.Y) / (float)screenpixeleqY;

            return p1;
        }


            //to select degrees and also it adds the ellipses in the graph 
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            degree = (int)numericUpDown1.Value;

            //this is used for the adding of the points based on the degree change
            cordpoints.Clear();

            for(int i=0 ; i <= degree ; ++i)
            {
                PointF newpoint = new PointF((i / (float)degree), 1.0f);
                cordpoints.Add(newpoint);
              
                controlpoints[i] = 1.0f;
            }

            this.Refresh();


        }

        
    

        //used to display lines
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

            float t = 0.0f;
            System.Drawing.Pen P = new Pen(Color.Black, 1.0f);
            
            e.Graphics.DrawLine(P, mathtoscreen(new Point(0,0)), mathtoscreen(new Point(3,0)));//X-axis
            e.Graphics.DrawLine(P, mathtoscreen(new Point(0,3)), mathtoscreen(new Point(0,-3)));//y-axis
            
            PointF p1;
            Brush dotcolor = new SolidBrush(Color.Red);
           
            for(int i = 0 ; i< cordpoints.Count ; ++i)
            {
                 p1 = mathtoscreen(cordpoints[i]);
                 e.Graphics.FillEllipse(dotcolor, p1.X-7,p1.Y-7,15,15);
            }

          //  e.Graphics.DrawLines(P, cordpoints.ToArray());

            List<PointF> new_list_points = new List<PointF>();
            PointF new_control_pts = new PointF();
            for (;t<=1; t+=0.01f)
            {
                if(isitBB)
                {
                    new_control_pts.X = t;
                    new_control_pts.Y = (BBform(controlpoints, degree, new_control_pts.X));
                
                }
                else if(isitNIL)
                {
                    new_control_pts.X = t;

                    float[] copy = new float[degree + 1];
                    for (int k = 0; k < degree+1; ++k)
                    {
                        copy[k] = controlpoints[k];
                    }


                    new_control_pts.Y = (NLIform(copy, degree, new_control_pts.X));
                  
                }
                 new_list_points.Add(mathtoscreen(new_control_pts));
            }
           
            e.Graphics.DrawLines(P, new_list_points.ToArray());

        }
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        public float BBform(float[] polynomial, int degree, float t)
        {
            float result = 0;
            int[,] get_pascal_value = new int[21, 21];
            int i, k;

            for(i = 0;i<degree+1;++i)//1
            {
                for(k=0;k<=i;++k)//i=0
                {
                    if (i == 0 || i == k || k ==0)
                        get_pascal_value[i, k] = 1;
                    else
                    {
                        get_pascal_value[i, k] = get_pascal_value[i-1, k-1] + get_pascal_value[i-1, k];
                    }
                }
            }


            for(int a=0; a<=degree;++a)
            {
                result += (float)(controlpoints[a] * get_pascal_value[degree, a] * ((Math.Pow(1 - t, degree - a)) * (Math.Pow(t, a))) );

            }

            return result;
        }

        public float NLIform(float[] polynomial ,int degree,float t)
        {
     
            int[,] get_pascal_value = new int[21, 21];
           // float[] Q = new float[50];
            int i, j;
            float Q;
 

            for ( i = degree; i > 0; --i)
            {
                float[] fur = new float[i];
                for ( j = 0; j < i; ++j)
                {
                    Q = polynomial[j] * (1 - t)+ polynomial[(j + 1)] * t;
                    fur[j] = Q;
                }
                for (int k = 0; k < i; ++k)
                {
                    polynomial[k] = fur[k];
                }
            }


            return polynomial[0];
        }

     
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(!point_click && e.Button == MouseButtons.Left)   //this checks if the left mouse button is clicked or not
            {
                point_click = true;
            }
            
            Refresh();
        }


        //no need for
       

        //this is used to move the point along Y-axis
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // Point p1 = new Point(0,0);

             if(point_click)
             {
                 for(int i = 0;i<cordpoints.Count;++i)
                 {
                     PointF P1 = mathtoscreen(cordpoints[i]);

                     if(P1.X - 7.5 < e.X && P1.X + 7.5 > e.X && P1.Y - 7.5 < e.Y && P1.Y + 7.5 > e.Y)
                     {
                         index_of_new_point = i;
                     }   
                 }
              
                 Point p1 = new Point(e.X, e.Y);
                 PointF pnew = screentomath(p1);

                 if(pnew.Y>-3.0 && pnew.Y < 3.0)
                 {
                     cordpoints[index_of_new_point] = new PointF( cordpoints[index_of_new_point].X, pnew.Y );
                     controlpoints[index_of_new_point] = pnew.Y;
                 }

                 
                 this.Refresh();

             }
           

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            point_click = false;
            this.Refresh();
        }


        //to select the type of algorithm
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex == 1) 
            {
                isitBB = true;
            }

            if(comboBox1.SelectedIndex == 0)
            {
                isitNIL = true;
            }
           // Console.Write(comboBox1.SelectedIndex);
        }

    }
}
