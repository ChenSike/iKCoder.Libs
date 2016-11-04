using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iKCoder_Platform_SDK_Kit.SDK.Forms
{
    public partial class Forms_NoBorderBaseform : Form
    {

        protected Point ponit_MouseOff; 
        protected bool bool_leftFlag;

        public Forms_NoBorderBaseform()
        {
            InitializeComponent();
        }

        private void Forms_NoBorderBaseform_Load(object sender, EventArgs e)
        {

        }

        private void Forms_NoBorderBaseform_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ponit_MouseOff = new Point(-e.X, -e.Y);  
                bool_leftFlag = true;                   
            }  
        }

        private void Forms_NoBorderBaseform_MouseMove(object sender, MouseEventArgs e)
        {
            if (bool_leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(ponit_MouseOff.X, ponit_MouseOff.Y);  
                (((System.Windows.Forms.PictureBox)sender).Parent).Location = mouseSet;
            }  
        }

        private void Forms_NoBorderBaseform_MouseUp(object sender, MouseEventArgs e)
        {
            if (bool_leftFlag)
            {
                bool_leftFlag = false;  
            }  
        }
    }
}
