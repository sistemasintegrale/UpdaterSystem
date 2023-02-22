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
using System.Windows.Documents;
using System.Windows.Forms;

namespace SGE.UpdaterApp
{
    public partial class FrmActualizador : MetroFramework.Forms.MetroForm
    {
        private Parametros objParametro = new Parametros();
        private ControlEquipos objEquipo = new ControlEquipos();
        private ControlVersiones objVersion = new ControlVersiones();
        private WebClient cliente;
        string pathArchivoRar, pathAplicacion;
        public FrmActualizador()
        {
            InitializeComponent();
            cliente = new WebClient();
            cliente.DownloadFileCompleted += new AsyncCompletedEventHandler(cargado!);
            cliente.DownloadProgressChanged += new DownloadProgressChangedEventHandler(cargando);
        }

        private void FrmActualizador_Load(object sender, EventArgs e)
        {
            ObtenerNombreDelEquipo();
            ObtenerParametros();
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


                //MODIFICAMOS EN LA BASE DE DATOS
                objEquipo.cvr_icod_version = objVersion.cvr_icod_version;
                objEquipo.ceq_sfecha_actualizacion = DateTime.Now;
                new GeneralData().Equipo_Modificar(objEquipo);

                //INSTALAMOS LA NUEVA VERSION
                Process.Start(pathAplicacion);

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

        void ObtenerNombreDelEquipo()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = new IPHostEntry();
            ipEntry = Dns.GetHostEntry(strHostName);
            string NombreEquipo = Convert.ToString(ipEntry.HostName);
            lblnombreEquipo.Text = $"Equipo : {NombreEquipo}";
            ObtenerDatosEquipo(NombreEquipo.Trim());
        }
        void ObtenerParametros()
        {
            objParametro = new GeneralData().Parametros_Listar().FirstOrDefault()!;
            lblNombreEmpresa.Text = objParametro.pm_nombre_empresa;
        }

        void ObtenerDatosEquipo(string nombre)
        {
            objEquipo = new GeneralData().Equipo_Obtner_Datos(nombre);
            txtVersionInstalada.Text = objEquipo.cvr_vversion;
            lblUbicacion.Text = objEquipo.cep_vubicacion_sistema;

            ObtenerActualizaciones();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            MsgShow frm = new MsgShow(Constantes.msgAlert, "La actualización esta por Comenzar, asegúrese que el sistema no este en ejecución");
            if (frm.ShowDialog() == DialogResult.OK)
            {

                Actualizar();
            }
        }
        private void Actualizar()
        {

            string pathPrincipal = objEquipo.cep_vubicacion_sistema;
            try
            {

                //PRIMERO ELIMINAMOS LA VERSION ANTERIOR
                pathAplicacion = pathPrincipal + @"\SGE.WindowForms.application";
                string PathFiles = pathPrincipal + @"\Application Files";
                Directory.Delete(PathFiles, true);
                File.Delete(pathAplicacion);

                //DESCARGAMOS DE DROPBOX
                pathArchivoRar = objEquipo.cep_vubicacion_sistema + @"\" + objVersion.cvr_vversion + ".zip";
                cliente.DownloadFileAsync(new Uri(objVersion.cvr_vurl), pathArchivoRar);

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }


        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            FrmDatosEquipo frm = new FrmDatosEquipo();
            frm.SetModify();
            frm.objEquipo = objEquipo;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                ObtenerNombreDelEquipo();
            }
        }

        void ObtenerActualizaciones()
        {
            objVersion = new GeneralData().Listar_Versiones().OrderByDescending(x => x.cvr_sfecha_version).FirstOrDefault()!;
            txtVersionDisponible.Text = objVersion.cvr_vversion;
            if (objVersion.cvr_icod_version == objEquipo.cvr_icod_version)
            {
                btnActualizar.Enabled = false;
            }
            else
            {
                btnActualizar.Enabled = true;
            }
        }

        void DesHabilitarControles(bool enable)
        {
            btnActualizar.Enabled = enable;
            iconButton1.Enabled = enable;
            if (enable)
            {
                btnActualizar.Text = "ACTUALIZAR";
            }
            else
            {
                btnActualizar.Text = "ACTUALIZANDO...";
            }
        }
    }
}
