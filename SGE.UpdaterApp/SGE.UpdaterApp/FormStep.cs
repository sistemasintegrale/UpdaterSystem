using Guna.UI2.WinForms;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SGE.UpdaterApp
{

    public partial class FormStep : Form
    {
        private ControlVersiones objVersion = new ControlVersiones();
        private ControlEquipos objEquipo = new ControlEquipos();
        string pathArchivoRar;
        private WebClient cliente;
        List<Usuario> mlist = new();
        int usua_flag;
        string mensaje = string.Empty;
        int indicador = 0;
        int instalado = 1;
        int actualizando = 2;
        string pathAplicacion = string.Empty;
        bool enabledTabLogin = true;
        bool enabledTabSeleccionar = false;
        bool enabledTabInstalar = false;
        bool enabledTabActualizar = false;
        int tabActual = 0;
        List<bool> tabs = new List<bool>();
        public FormStep()
        {
            InitializeComponent();
            cliente = new WebClient();
            cliente.DownloadFileCompleted += new AsyncCompletedEventHandler(cargado);
            cliente.DownloadProgressChanged += new DownloadProgressChangedEventHandler(cargando);

            tabs.Add(enabledTabLogin);
            tabs.Add(enabledTabSeleccionar);
            tabs.Add(enabledTabInstalar);
            tabs.Add(enabledTabActualizar);
        }

        private void cargando(object sender, DownloadProgressChangedEventArgs e)
        {
            if (indicador == instalado)
            {
                barInstall.Value = e.ProgressPercentage;
            }
            if (indicador == actualizando)
            {
                barActualizar.Value = e.ProgressPercentage;
            }

        }

        private void cargado(object sender, AsyncCompletedEventArgs e)
        {
            Instalar();
            if (indicador == instalado)
            {

            }
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

                if (indicador == instalado)
                {
                    //INSERTAMOS EN LA BASE DE DATOS
                    objEquipo.cvr_icod_version = objVersion.cvr_icod_version;
                    objEquipo.ceq_sfecha_actualizacion = DateTime.Now;
                    objEquipo.ceq_vnombre_equipo = this.Text;
                    new GeneralData().Equipo_Insertar(objEquipo);
                    Instalacion frm = new Instalacion();
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        Process.Start(new ProcessStartInfo { FileName = objEquipo.cep_vubicacion_sistema + @"\SGE.WindowForms.application", UseShellExecute = true });
                        Application.Exit();
                    }
                }
                else
                {
                    //MODIFICAMOS EN LA BASE DE DATOS
                    objEquipo.cvr_icod_version = objVersion.cvr_icod_version;
                    objEquipo.ceq_sfecha_actualizacion = DateTime.Now;
                    new GeneralData().Equipo_Modificar(objEquipo);
                    Process.Start(new ProcessStartInfo { FileName = objEquipo.cep_vubicacion_sistema + @"\SGE.WindowForms.application", UseShellExecute = true });
                    Application.Exit();
                }


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
            switch (User_Verification(txtUsuario.Text, txtContraseña.Text))
            {

                case 0:
                    accees();
                    break;
                case 1:
                    mensaje = "Contraseña Incorrecta";
                    MsgShow msgShow = new MsgShow(Constantes.msgError, mensaje);
                    msgShow.ShowDialog();
                    break;
                case 2:

                    mensaje = "Nombre de usuario no existe";
                    MsgShow msgShow2 = new MsgShow(Constantes.msgError, mensaje);
                    msgShow2.ShowDialog();
                    break;
            }
        }
        public int User_Verification(string usua_usuario, string usua_pass)
        {
            int result;
            // 0 datos correctos
            // 1 password incorrecto
            // 2 usuario incorrecto
            usua_flag = mlist.FindIndex(x => x.usua_codigo_usuario == usua_usuario);

            if (usua_flag >= 0)
            {
                if (mlist[usua_flag].usua_password_usuario == CoDec.codec(usua_pass))
                {

                    result = 0;
                }
                else
                {
                    result = 1;
                }
            }
            else
            {
                result = 2;


            }

            return result;
        }

        void accees()
        {
            tabs[Constantes.tabLogin] = false;
            mlist.Where(x => x.usua_codigo_usuario == txtUsuario.Text && x.usua_password_usuario == CoDec.codec(txtContraseña.Text)).ToList().ForEach(data =>
            {
                Constantes.conneciones.Add(data.connection);
            });

            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = new IPHostEntry();
            ipEntry = Dns.GetHostEntry(strHostName);
            string NombreEquipo = Convert.ToString(ipEntry.HostName);
            this.Text = NombreEquipo;


            EnabledControls(false);

            if (Constantes.conneciones.Count == 1) // CUANDO EL USUARIO SOLO PERTENESE A UNA EMPRESA
            {
                Constantes.Connection = Constantes.conneciones.First();
                verificar();
            }
            else// CUANDO EL USUARIO SOLO PERTENESE A MAS DE UNA EMPRESA
            {
                tabSelect();
            }

        }

        void verificar()
        {
            //VERIFICAR SI EL EQUIPO ESTA REGISTRADO            
            objEquipo = new GeneralData().Equipo_Obtner_Datos(this.Text);
            if (objEquipo is null)
            {
                MsgShow msg = new MsgShow(Constantes.msgError, $"El equipo {this.Text} no tiene permisos para el sistema");
                if (msg.ShowDialog() == DialogResult.OK)
                {
                    Application.Exit();
                }
            }

            //VERIFICAMOS SI YA ESTA INSTALADO EL SISTEMA
            string pathSistema = @"C:\\Publish-" + HelperConnection.GeneratePath(Constantes.Connection);
            if (!Directory.Exists(pathSistema))
            {
                Directory.CreateDirectory(pathSistema);
                IrTabInstalacion();
            }
            else
            {
                IrTabActualizacion();
            }
        }



        void EnabledControls(bool enab)
        {
            txtUsuario.Enabled = enab;
            txtContraseña.Enabled = enab;
            guna2Button1.Enabled = enab;
        }

        private void FormStep_Load(object sender, EventArgs e)
        {
            List<Usuario> list = new List<Usuario>();
            for (int i = 1; i <= Constantes.ConnNovaMotos; i++)
            {
                Constantes.Connection = i;
                var lista = new GeneralData().listarUsuarios();
                lista.ForEach((data) =>
                {
                    data.connection = i;

                });
                list.AddRange(lista);
            }
            mlist = list;
        }

        private void txtContraseña_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                guna2Button1_Click(sender, e);
            }
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        void tabSelect()
        {
            tabs[Constantes.tabSeleccionar] = true;
            guna2TabControl1.SelectedIndex = 1;
            var list = new List<Empresas>();
            Constantes.conneciones.ForEach(x =>
            {
                if (x == Constantes.ConnGrenPeru)
                { var obj1 = new Empresas(); obj1.Id = Constantes.ConnGrenPeru; obj1.Name = "GREEN PERU"; list.Add(obj1); }
                if (x == Constantes.ConnGalyCompany)
                { var obj2 = new Empresas(); obj2.Id = Constantes.ConnGalyCompany; obj2.Name = "GALY COMPANY"; list.Add(obj2); }
                if (x == Constantes.ConnMotoTorque)
                { var obj3 = new Empresas(); obj3.Id = Constantes.ConnMotoTorque; obj3.Name = "MOTO TORQUE"; list.Add(obj3); }
                if (x == Constantes.ConnNovaGlass)
                { var obj4 = new Empresas(); obj4.Id = Constantes.ConnNovaGlass; obj4.Name = "NOVA GLASS"; list.Add(obj4); }
                if (x == Constantes.ConnNovaFlat)
                { var obj5 = new Empresas(); obj5.Id = Constantes.ConnNovaFlat; obj5.Name = "NOVA FLAT"; list.Add(obj5); }
                if (x == Constantes.ConnNovaMotos)
                { var obj6 = new Empresas(); obj6.Id = Constantes.ConnNovaMotos; obj6.Name = "NOVA MOTOS"; list.Add(obj6); }
            });

            BSControls.Guna2Combo(lkpSistema, list, "Name", "Id", true);
        }

        void IrTabInstalacion()
        {
            tabs[Constantes.tabInstalar] = true;
            indicador = instalado;
            guna2TabControl1.SelectedIndex = Constantes.tabInstalar;
            objVersion = new GeneralData().Listar_Versiones().OrderByDescending(x => x.cvr_sfecha_version).FirstOrDefault()!;
            string pathPrincipal = @"C:\\Publish-" + HelperConnection.GeneratePath(Constantes.Connection);
            objEquipo.cep_vubicacion_sistema = pathPrincipal;
            pathArchivoRar = objEquipo.cep_vubicacion_sistema + @"\" + objVersion.cvr_vversion + ".zip";
            cliente.DownloadFileAsync(new Uri(objVersion.cvr_vurl), pathArchivoRar);

        }

        void IrTabActualizacion()
        {
            tabs[Constantes.tabActualizar] = true;
            guna2TabControl1.SelectedIndex = Constantes.tabActualizar;
            objEquipo = new GeneralData().Equipo_Obtner_Datos(this.Text);
            txtVersionInstalada.Text = objEquipo.cvr_vversion;
            ObtenerActualizaciones();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Constantes.Connection = Convert.ToInt32(lkpSistema.SelectedValue);
            tabs[Constantes.tabSeleccionar] = false;
            verificar();
        }

        void ObtenerActualizaciones()
        {
            objVersion = new GeneralData().Listar_Versiones().OrderByDescending(x => x.cvr_sfecha_version).FirstOrDefault()!;
            txtVersionDisponible.Text = objVersion is null ? "" : objVersion.cvr_vversion;
            if (objVersion is not null)
            {
                if (objVersion.cvr_icod_version == objEquipo.cvr_icod_version)
                    btnActualizar.Enabled = false;
                else
                {
                    btnActualizar.Enabled = true;
                }
            }
            else
            {
                btnActualizar.Enabled = false;
            }

            if (!btnActualizar.Enabled)
            {
                MsgShow msg = new MsgShow(Constantes.msgAlert, "No se Encontraron Actualizaciones");
                if (msg.ShowDialog() == DialogResult.OK)
                {
                    Application.Exit();
                }
            }
            else
            {
                object sender = new object();
                EventArgs e = new EventArgs();
                btnActualizar_Click(sender, e);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            Actualizar();
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
                indicador = actualizando;
                cliente.DownloadFileAsync(new Uri(objVersion.cvr_vurl), pathArchivoRar);

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }


        }

        private void guna2TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int tabSiguiente = Convert.ToInt32(guna2TabControl1.SelectedIndex);
            if (!tabs[tabSiguiente])
            {
                guna2TabControl1.SelectedIndex = tabActual;
            }
            else
            {
                tabActual = tabSiguiente;
            }
        }

        private void lkpSistema_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                guna2Button2_Click(sender, e);
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
