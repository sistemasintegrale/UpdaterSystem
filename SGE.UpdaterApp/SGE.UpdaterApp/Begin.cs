using SGE.UpdaterApp.DataAcces;
using SGE.UpdaterApp.Entities;
using SGE.UpdaterApp.Helpers;
using System.Windows.Forms;

namespace SGE.UpdaterApp
{
    public partial class Begin : Form
    {
        List<Usuario> mlist = new();
        int usua_flag;
        string mensaje = string.Empty;
        public Begin()
        {
            InitializeComponent();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            int valid = 0;

            valid = Nuevo();

            if (valid != 0)
            {
                mlist = new GeneralData().listarUsuarios();

                switch (User_Verification(txtUsuario.Text, txtContraseña.Text))
                {

                    case 0:
                        clearControl();
                        this.Hide();
                        FrmActualizador main = new FrmActualizador();
                        main.Show();
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
            else
            {
                FrmConfigurarEquipo frm = new FrmConfigurarEquipo();
                frm.Show();
                this.Hide();
            }


        }

        private void clearControl()
        {
            txtUsuario.Text = "";
            txtUsuario.Text = "";
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
                result = 2;
            return result;
        }

        int Nuevo()
        {
            int result = 0;
            if (!txtUsuario.Text.ToUpper().Equals("SISTEMA"))
            {
                //MsgShow msgShow = new MsgShow(Constantes.msgError, "Usuario Inconrrecto");
                //msgShow.ShowDialog();
                result = 2;
            }
            if (!txtContraseña.Text.Equals("rogola2012"))
            {
                //MsgShow msgShow = new MsgShow(Constantes.msgError, "Contraseña Inconrrecta");
                //msgShow.ShowDialog();
                result = 1;
            }
            return result;
            
        }

        private string[] GetInfoTxt(string ruta)
        {

            string Linea;
            string[] Valores = null!;
            if (File.Exists(ruta))
            {
                using (StreamReader lector = new StreamReader(ruta))
                {
                    Linea = lector.ReadLine()!;
                    Valores = Linea.Split(",".ToCharArray());
                }
            }
            return Valores;
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Begin_Load(object sender, EventArgs e)
        {
            //VALIDAR SI EL EQUIPO YA ESTÁ REGISTRADO

            for (int i = 1; i <= Constantes.ConnNovaMotos; i++)
            {
                string path = Constantes.rutaCarpetaInfo + HelperConnection.GeneratePath(i) + Constantes.rutaInfotxt;

                if (File.Exists(path))
                {
                    string[] values = GetInfoTxt(path);
                    Constantes.Connection = Convert.ToInt32(CoDec.DesencriptarConn(values[0]));
                }
            }

        }
    }
}