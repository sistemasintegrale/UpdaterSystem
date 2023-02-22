using SGE.UpdaterApp.DataAcces;
using SGE.UpdaterApp.Entities;
using SGE.UpdaterApp.Helpers;

namespace SGE.UpdaterApp
{
    public partial class FrmDatosEquipo : MetroFramework.Forms.MetroForm
    {
        public ControlEquipos objEquipo = new ControlEquipos();
        string initialDirectory = string.Empty;
        public BSMaintenanceStatus oState;
        private BSMaintenanceStatus mStatus;
        public BSMaintenanceStatus Status
        {
            get { return (mStatus); }
            set
            {
                mStatus = value;

            }
        }
        public void SetModify()
        {
            Status = BSMaintenanceStatus.ModifyCurrent;
        }
        public FrmDatosEquipo()
        {
            InitializeComponent();
        }

        public void SetValues()
        {
            initialDirectory = objEquipo.cep_vubicacion_sistema;
            txtNombreEquipo.Text = objEquipo.ceq_vnombre_equipo;
            txtUbicacionSistema.Text = objEquipo.cep_vubicacion_sistema;
            txtVersionSistema.Text = objEquipo.cvr_vversion;
        }

        private void FrmDatosEquipo_Load(object sender, EventArgs e)
        {
            if (Status != BSMaintenanceStatus.CreateNew)
            {
                SetValues();
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            objEquipo.ceq_vnombre_equipo = txtNombreEquipo.Text;
            objEquipo.cep_vubicacion_sistema = txtUbicacionSistema.Text;
            objEquipo.cvr_vversion = txtVersionSistema.Text;
            if (Status == BSMaintenanceStatus.CreateNew)
            {

            }
            else
            {
                new GeneralData().Equipo_Modificar(objEquipo);
            }
            DialogResult = DialogResult.OK;
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = objEquipo.cep_vubicacion_sistema;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtUbicacionSistema.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
