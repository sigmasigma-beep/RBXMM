using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace Kyoshades_RMacro
{
    public partial class Form1: Form
    {
        private bool isRecording = false;

        public Form1()
        {
            InitializeComponent();

        }



        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);
        private void guna2Button2_Click(object sender, EventArgs e)
        {
           this.WindowState = FormWindowState.Minimized;
        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {
            Application.Exit();
        }
        MacroRecorder recorder = new MacroRecorder();
        MacroEngine engine = new MacroEngine();
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            engine.Play(recorder.Actions);
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            recorder.Start();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            recorder.Stop();
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Macro Files (*.json)|*.json";
                sfd.DefaultExt = "json";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    MacroFileManager.Save(sfd.FileName, recorder.Actions);
                }
            }
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Macro Files (*.json)|*.json";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    recorder.Actions = MacroFileManager.Load(ofd.FileName);
                }
            }
        }

        private void guna2CheckBox5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            
            if (!guna2CheckBox1.Checked)
                return;

           
            if ((Control.ModifierKeys & Keys.None) == Keys.None) 
            {
                if (GetAsyncKeyState(Keys.X) < 0) // blank key pressed
                {
                    if (recorder.Actions.Count > 0)
                        engine.Play(recorder.Actions);
                }
            }
        }

        private void guna2CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!guna2CheckBox2.Checked)
                return;


            if ((Control.ModifierKeys & Keys.None) == Keys.None)
            {
                if (GetAsyncKeyState(Keys.LButton) < 0) // blank key pressed
                {
                    if (recorder.Actions.Count > 0)
                        engine.Play(recorder.Actions);
                }
            }
        }

        private void guna2CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (!guna2CheckBox3.Checked)
                return;


            if ((Control.ModifierKeys & Keys.None) == Keys.None)
            {
                if (GetAsyncKeyState(Keys.RButton) < 0) // blank key pressed
                {
                    if (recorder.Actions.Count > 0)
                        engine.Play(recorder.Actions);
                }
            }
        }

        private void guna2CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (!guna2CheckBox4.Checked)
                return;


            if ((Control.ModifierKeys & Keys.None) == Keys.None)
            {
                if (GetAsyncKeyState(Keys.Z) < 0) // blank key pressed
                {
                    if (recorder.Actions.Count > 0)
                        engine.Play(recorder.Actions);
                }
            }
        }

        private void guna2CheckBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (!guna2CheckBox7.Checked)
                return;


            if ((Control.ModifierKeys & Keys.None) == Keys.None)
            {
                if (GetAsyncKeyState(Keys.X) < 0) // blank key pressed
                {
                    if (recorder.Actions.Count > 0)
                        engine.Play(recorder.Actions);
                }
            }
        }

       

        private void guna2CheckBox6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {
            engine.Stop(recorder.Actions);
        }
    }
}
