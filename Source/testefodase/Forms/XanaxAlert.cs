
using System;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Xanax
{
    [Obfuscation(Feature = "virtualization", Exclude = false)]
    public partial class XanaxAlert : Form
    {
        public XanaxAlert()
        {
            InitializeComponent();
        }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public enum enmAction
        {
            wait,
            start,
            close
        }
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public enum enmType
        {
            Applying,
            Applied,
            Error,
            Info
        }
        private XanaxAlert.enmAction action;

        private int x, y;
        [Obfuscation(Feature = "virtualization", Exclude = false)]
        private async void timer1_Tick(object sender, EventArgs e)
        {
            switch (this.action)
            {
                case enmAction.wait:
                    await Task.Delay(2000);
                    action = enmAction.close;
                    break;
                case XanaxAlert.enmAction.start:
                    this.timer1.Interval = 1;
                    this.Opacity += 0.1;
                    if (this.x < this.Location.X)
                    {
                        this.Left--;
                    }
                    else
                    {
                        if (this.Opacity == 1.0)
                        {
                            action = XanaxAlert.enmAction.wait;
                        }
                    }
                    break;
                case enmAction.close:
                    timer1.Interval = 1;
                    this.Opacity -= 0.1;

                    this.Left -= 3;
                    if (base.Opacity == 0.0)
                    {
                        base.Close();
                    }
                    break;
            }
        }


        [Obfuscation(Feature = "virtualization", Exclude = false)]
        public void showAlert(string msg, enmType type)
        {
            this.Opacity = 0.0;
            this.StartPosition = FormStartPosition.Manual;
            string fname;

            for (int i = 1; i < 10; i++)
            {
                fname = "alert" + i.ToString();
                XanaxAlert frm = (XanaxAlert)Application.OpenForms[fname];

                if (frm == null)
                {
                    this.Name = fname;
                    this.x = Screen.PrimaryScreen.WorkingArea.Width - this.Width + 15;
                    this.y = Screen.PrimaryScreen.WorkingArea.Height - this.Height * i - 5 * i;
                    this.Location = new Point(this.x, this.y);
                    break;

                }

            }
            this.x = Screen.PrimaryScreen.WorkingArea.Width - base.Width - 5;

            switch (type)
            {
                case enmType.Applying:
                    this.lblMsg.Text = msg;
                    break;
                case enmType.Applied:
                    this.lblMsg.Text = "Successfully Applied";
                    break;
                case enmType.Error:
                    this.lblMsg.Text = msg;
                    break;
            }

            this.Show();
            this.action = enmAction.start;
            this.timer1.Interval = 1;
            this.timer1.Start();
        }
    }
}
