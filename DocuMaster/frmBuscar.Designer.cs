using System.Windows.Forms;

namespace DocuMaster
{
    partial class frmBuscar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 100);
            this.CenterToScreen();
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = "Buscar";

            txtTextoXBuscar = new TextBox();
            txtTextoXBuscar.Name = "txtTextoXBuscar";
            txtTextoXBuscar.Location = new System.Drawing.Point(10, 20);
            txtTextoXBuscar.Width = 220;
            txtTextoXBuscar.Height = 30;
            txtTextoXBuscar.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
            Controls.Add(txtTextoXBuscar);

            Button btnBuscar = new Button();
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Text = "Buscar";
            btnBuscar.Location = new System.Drawing.Point(150, 60);
            btnBuscar.Width = 80;
            btnBuscar.Height = 30;
            btnBuscar.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            btnBuscar.Click += btnBuscar_Click;
            Controls.Add(btnBuscar);
        }

        bool c_bBuscar = false;

        public string Ejecutar(ref bool bBuscar)
        {
            this.c_bBuscar = false;
            this.ShowDialog();
            bBuscar = this.c_bBuscar;
            return txtTextoXBuscar.Text;
        }
        TextBox txtTextoXBuscar = null;

        private void btnBuscar_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.c_bBuscar = true;
                this.Close();
            }
            catch (System.Exception objException)
            {
                MessageBox.Show(objException.Message);

            }
        }

        #endregion
    }
}