using SGE.UpdaterApp.DataAcces;
using SGE.UpdaterApp.Entities;
using SGE.UpdaterApp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGE.UpdaterApp
{
    public partial class FrmConfigurarEquipo : MetroFramework.Forms.MetroForm
    {
        private ControlVersiones objVersion = new ControlVersiones();
        private WebClient cliente;
        private ControlEquipos objEquipo = new ControlEquipos();
        string pathArchivoRar, pathAplicacion;
        bool Frmcargado = false;
        public FrmConfigurarEquipo()
        {
            InitializeComponent();
            cliente = new WebClient();
            cliente.DownloadFileCompleted += new AsyncCompletedEventHandler(cargado!);
            cliente.DownloadProgressChanged += new DownloadProgressChangedEventHandler(cargando);
        }
        public class Empresas
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private void FrmConfigurarEquipo_Load(object sender, EventArgs e)
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = new IPHostEntry();
            ipEntry = Dns.GetHostEntry(strHostName);
            string NombreEquipo = Convert.ToString(ipEntry.HostName);
            txtEquipo.Text = NombreEquipo;


            List<Empresas> list = new List<Empresas>();
            var obj1 = new Empresas(); obj1.Id = Constantes.ConnGrenPeru; obj1.Name = "GREEN PERU"; list.Add(obj1);
            var obj2 = new Empresas(); obj2.Id = Constantes.ConnGalyCompany; obj2.Name = "GALY COMPANY"; list.Add(obj2);
            var obj3 = new Empresas(); obj3.Id = Constantes.ConnMotoTorque; obj3.Name = "MOTO TORQUE"; list.Add(obj3);
            var obj4 = new Empresas(); obj4.Id = Constantes.ConnNovaGlass; obj4.Name = "NOVA GLASS"; list.Add(obj4);
            var obj5 = new Empresas(); obj5.Id = Constantes.ConnNovaFlat; obj5.Name = "NOVA FLAT"; list.Add(obj5);
            var obj6 = new Empresas(); obj6.Id = Constantes.ConnNovaMotos; obj6.Name = "NOVA MOTOS"; list.Add(obj6);

            BSControls.Guna2Combo(lkpEmpresa1, list, "Name", "Id", true);
            BSControls.Guna2Combo(lkpEmpresa2, list, "Name", "Id", true);
            Frmcargado = true;
            validarExistencia();
        }


        private void cargando(object sender, DownloadProgressChangedEventArgs e)
        {
            DesHabilitarControles(false);
            ProgressBar.Value = e.ProgressPercentage;
        }

        private void cargado(object sender, AsyncCompletedEventArgs e)
        {
            Instalar();
            DesHabilitarControles(true);
        }
        void Instalar()
        {
            try
            {
                Process procesoExtaccion = new Process();
                ProcessStartInfo informacionProcceso = new ProcessStartInfo();
                informacionProcceso.FileName = @"C:\Program Files\WinRAR\WinRAR.exe";
                informacionProcceso.Arguments = "x " + FormatoDoleComilal(pathArchivoRar) + " " + FormatoDoleComilal(objEquipo.cep_vubicacion_sistema);
                procesoExtaccion.StartInfo = informacionProcceso;
                procesoExtaccion.Start();


                //INSERTAMOS EN LA BASE DE DATOS
                objEquipo.cvr_icod_version = objVersion.cvr_icod_version;
                objEquipo.ceq_sfecha_actualizacion = DateTime.Now;
                objEquipo.ceq_vnombre_equipo = txtEquipo.Text;
                new GeneralData().Equipo_Insertar(objEquipo);

                //INSTALAMOS LA NUEVA VERSION
                Process.Start(objEquipo.cep_vubicacion_sistema + @"\SGE.WindowForms.application");
                validarExistencia();

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }
        private String FormatoDoleComilal(string sRuta)
        {
            return Convert.ToChar(34).ToString() + sRuta + Convert.ToChar(34).ToString();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Constantes.Connection = Convert.ToInt32(lkpEmpresa1.SelectedValue);



            //EJECUTAMOS LA PRIMERA INSTALACION
            objVersion = new GeneralData().Listar_Versiones().OrderByDescending(x => x.cvr_sfecha_version).FirstOrDefault()!;


            //PRIMERO ELIMINAMOS LA VERSION ANTERIOR
            string pathPrincipal = @"C:\\Publish-" + HelperConnection.GeneratePath(Constantes.Connection);
            if (!Directory.Exists(pathPrincipal))
            {
                Directory.CreateDirectory(pathPrincipal);
            }
            objEquipo.cep_vubicacion_sistema = pathPrincipal;


            //DESCARGAMOS DE DROPBOX
            pathArchivoRar = objEquipo.cep_vubicacion_sistema + @"\" + objVersion.cvr_vversion + ".zip";
            cliente.DownloadFileAsync(new Uri(objVersion.cvr_vurl), pathArchivoRar);

            //CREAMOS LA CARPERTA DE CONFIGURACION
            if (!Directory.Exists(Constantes.rutaCarpetaInfo + HelperConnection.GeneratePath(Constantes.Connection)))
            {
                Directory.CreateDirectory(Constantes.rutaCarpetaInfo + HelperConnection.GeneratePath(Constantes.Connection));
            }
            StreamWriter fs = File.CreateText(Constantes.rutaCarpetaInfo + HelperConnection.GeneratePath(Constantes.Connection) + Constantes.rutaInfotxt);
            fs.Write(CoDec.EncriptarConn(Constantes.Connection.ToString()));
            File.SetAttributes(Constantes.rutaCarpetaInfo + HelperConnection.GeneratePath(Constantes.Connection) + Constantes.rutaInfotxt, FileAttributes.ReadOnly);
            fs.Close();

        }

        private void lkpEmpresa1_SelectedIndexChanged(object sender, EventArgs e)
        {
            validarExistencia();
        }

        void validarExistencia()
        {
            if (Frmcargado)
                if (Directory.Exists(Constantes.rutaCarpetaInfo + HelperConnection.GeneratePath(Convert.ToInt32(lkpEmpresa1.SelectedValue))))
                {
                    guna2Button1.Enabled = false;
                }
                else
                {
                    guna2Button1.Enabled = true;
                }
        }

        private void guna2Button1_ClientSizeChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Constantes.Connection = Convert.ToInt32(lkpEmpresa2.SelectedValue);
            FrmActualizador frm = new FrmActualizador();
            frm.ShowDialog();
            this.Hide();
        }

        void DesHabilitarControles(bool enable)
        {
            guna2Button1.Enabled = enable;
            guna2Button2.Enabled = enable;
            lkpEmpresa1.Enabled = enable;
            lkpEmpresa2.Enabled = enable;   
        }
    }
}
