using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iKCoder_Platform_SDK_Kit
{
    public partial class Forms_NoBorderBaseform : Form
    {

        protected Point ponit_MouseOff; 
        protected bool bool_leftFlag;
        protected string string_activeDragDropFilename;
        protected TextBox ref_TextBox_DragDropFilename = null;

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
                this.Location = mouseSet;                
            }  
        }

        private void Forms_NoBorderBaseform_MouseUp(object sender, MouseEventArgs e)
        {
            if (bool_leftFlag)
            {
                bool_leftFlag = false;  
            }  
        }

        private void Forms_NoBorderBaseform_DragDrop(object sender, DragEventArgs e)
        {            
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            string_activeDragDropFilename = path;
            if(ref_TextBox_DragDropFilename!=null)
            {
                ref_TextBox_DragDropFilename.Text = path;
            }
        }

        private void Forms_NoBorderBaseform_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void Forms_NoBorderBaseform_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }
    }
}
