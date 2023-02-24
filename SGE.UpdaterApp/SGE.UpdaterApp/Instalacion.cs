using SGE.UpdaterApp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGE.UpdaterApp
{
    public partial class Instalacion : Form
    {
        public Instalacion()
        {
            InitializeComponent();
        }

        private void metroLink1_Click(object sender, EventArgs e)
        {
            string ruta = HelperConnection.GeneratePath(Constantes.Connection);
            Process.Start(new ProcessStartInfo { FileName = @"C:\\Publish-" + ruta, UseShellExecute = true });
        }

        void siguiente(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = guna2TabControl1.SelectedIndex + 1;
        }
        void Anterior(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = guna2TabControl1.SelectedIndex - 1;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
