using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pmfst_GameSDK
{
    public partial class Kraj : Form
    {
        public Kraj()
        {
            InitializeComponent();
        }

        private int brojac;

        public int Brojac
        {
            get
            {
                return brojac;
            }

            set
            {
                brojac = value;
            }
        }

        private void Kraj_Load(object sender, EventArgs e)
        {
            label1.Text = "Bodovi: " + brojac;
        }
    }
}
