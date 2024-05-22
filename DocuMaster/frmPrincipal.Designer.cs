namespace DocuMaster
{
    using System.Windows.Forms;
    using System.Linq;
    using System.IO;

    partial class frmPrincipal
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
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.CenterToScreen();
            this.Text = "DocuMaster";

            btnBuscar = new Button();
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Text = "Buscar";
            btnBuscar.Location = new System.Drawing.Point(555, 410);
            btnBuscar.Width = 80;
            btnBuscar.Height = 30;
            btnBuscar.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            btnBuscar.Visible = false;
            btnBuscar.Click += btnBuscar_Click;
            Controls.Add(btnBuscar);

            Button btnAbrirLibroElectronico = new Button();
            btnAbrirLibroElectronico.Name = "btnAbrirLibroElectronico";
            btnAbrirLibroElectronico.Text = "Abrir Libro Electrónico";
            btnAbrirLibroElectronico.Location = new System.Drawing.Point(640, 410);
            btnAbrirLibroElectronico.Width = 150;
            btnAbrirLibroElectronico.Height = 30;
            btnAbrirLibroElectronico.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            btnAbrirLibroElectronico.Click += btnAbrirLibroElectronico_Click;
            Controls.Add(btnAbrirLibroElectronico);

            twTreeView = new TreeView();
            twTreeView.Name = "twTreeView";
            twTreeView.Location = new System.Drawing.Point(10, 10);
            twTreeView.Width = 335;
            twTreeView.Height = 390;
            twTreeView.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom);
            twTreeView.TabStop = false;
            twTreeView.AfterSelect += twTreeView_AfterSelect;
            twTreeView.Visible = true;
            Controls.Add(twTreeView);

            twResultsTreeView = new TreeView();
            twResultsTreeView.Name = "twResultsTreeView";
            twResultsTreeView.Location = new System.Drawing.Point(10, 10);
            twResultsTreeView.Width = 335;
            twResultsTreeView.Height = 390;
            twResultsTreeView.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom);
            twResultsTreeView.TabStop = false;
            twResultsTreeView.AfterSelect += twResultsTreeView_AfterSelect;
            twResultsTreeView.Visible = false;
            Controls.Add(twResultsTreeView);

            wbWebBrowser = new WebBrowser();
            wbWebBrowser.Name = "wbWebBrowser";
            wbWebBrowser.Location = new System.Drawing.Point(350, 10);
            wbWebBrowser.Width = 440;
            wbWebBrowser.Height = 390;
            wbWebBrowser.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            wbWebBrowser.TabStop = false;
            this.wbWebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.wbWebBrowser_DocumentCompleted);
            Controls.Add(wbWebBrowser);

            btnVerArbol = new Button();
            btnVerArbol.Name = "btnVerArbol";
            btnVerArbol.Text = "Ver Arbol";
            btnVerArbol.Location = new System.Drawing.Point(470, 410);
            btnVerArbol.Width = 80;
            btnVerArbol.Height = 30;
            btnVerArbol.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            btnVerArbol.Click += btnVerArbol_Click;
            btnVerArbol.Visible = false;
            Controls.Add(btnVerArbol);

            btnVerResultados = new Button();
            btnVerResultados.Name = "btnVerResultados";
            btnVerResultados.Text = "Ver Resultados";
            btnVerResultados.Location = new System.Drawing.Point(440, 410);
            btnVerResultados.Width = 110;
            btnVerResultados.Height = 30;
            btnVerResultados.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            btnVerResultados.Click += btnVerResultados_Click;
            btnVerResultados.Visible = false;
            Controls.Add(btnVerResultados);
        }

        private void btnBuscar_Click(object sender, System.EventArgs e)
        {
            try
            {
                frmBuscar m_frmBuscar = new frmBuscar();
                bool m_bBuscar = false;
                string m_sTextoXBuscar = m_frmBuscar.Ejecutar(ref m_bBuscar);

                if (m_bBuscar)
                {
                    if (!File.Exists(c_sRutaArchivoIndice))
                    {
                        MessageBox.Show("No existe el archivo " + c_sRutaArchivoIndice);
                        return;
                    } // if (!File.Exists(c_sRutaArchivoIndice))

                    if (!File.Exists(SQLITE_SHELL_FILE_PATH))
                    {
                        MessageBox.Show("No existe el archivo " + SQLITE_SHELL_FILE_PATH);
                        return;
                    } // if (!File.Exists(SQLITE_SHELL_FILE_PATH))

                    string m_sPalabrasXBuscar = m_sTextoXBuscar;

                    CBuscar m_Buscar = new CBuscar(c_sRutaArchivoIndice, SQLITE_SHELL_FILE_PATH);
                    m_Buscar.ConsultarXPalabras(m_sPalabrasXBuscar);

                    //MessageBox.Show("Presione el botón o la tecla ENTER para continuar");

                    string m_sErrorText = m_Buscar.GetErrorText();

                    if (m_sErrorText.Length != 0)
                        MessageBox.Show(m_sErrorText);
                    else
                    {
                        c_ResaltarTextoHTML = new CResaltarTextoHTML(m_Buscar.PalabrasBuscadas(), m_Buscar.DemeCaracteresValidos());
                        c_bResaltar = false; // Todavía no se resalta
                        System.Collections.SortedList m_ListaNodos = m_Buscar.LoadTreeNodes();
                        this.LoadResultNodes(m_ListaNodos);
                        twTreeView.Visible = false;
                        twResultsTreeView.Visible = true;
                        btnVerArbol.Visible = true;
                        btnVerResultados.Visible = false;
                        btnVerArbol.Focus();
                        c_CurrentNodeInResultsTreeView = null;
                        
                        twResultsTreeView.TabStop = false;

                        if (twResultsTreeView.GetNodeCount(false) > 0)
                        {
                            twResultsTreeView.TabStop = true;
                            twResultsTreeView.Focus();
                        } // if (twResultsTreeView.GetNodeCount(false) > 0)
                    } // else
                } //if (m_bBuscar)
            }
            catch (System.Exception objException)
            {
                MessageBox.Show(objException.Message);

            }
        }

        private string GetFileNameOfURL(string sURL)
        {
            string[] m_Parts = sURL.Split('/');
            string m_sFileName = "";

            if (m_Parts.Length > 0)
                m_sFileName = m_Parts[m_Parts.Length - 1];

            return m_sFileName;
        } // private string GetFileNameOfURL(string sURL)
        
        const string DOCUMENTS_EXTENSION = ".htm";
        string c_sNavigatingToNodeFileName = "";
        bool c_bResaltar = false;
        private CResaltarTextoHTML c_ResaltarTextoHTML = null;

        private void wbWebBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //wbWebBrowser.TabStop = c_sNavigatingToNodeFileName.Equals(this.GetFileNameOfURL(this.wbWebBrowser.Url.ToString())) ? true: false;
        }

        private void wbWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (c_bResaltar)
            {
                string m_sTextoResaltado = c_ResaltarTextoHTML.Resaltar(wbWebBrowser.Document.Body.InnerHtml, "");
                wbWebBrowser.Document.Body.InnerHtml = m_sTextoResaltado;
            } // if (c_bResaltar)
        }

        private void Navegar(string sIdNodo)
        {
            this.c_sNavigatingToNodeFileName = sIdNodo + DOCUMENTS_EXTENSION;
            string m_sFilePath = "file:///" + this.c_sElectronicBookFolderPath.Replace("\\", "/") + "/" + this.c_sNavigatingToNodeFileName;
            wbWebBrowser.Navigate(m_sFilePath);
        }
        private void twTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            c_bResaltar = false;
            string m_sIdNodo = e.Node.Tag.ToString();
            Navegar(m_sIdNodo);
        }

        TreeNode c_CurrentNodeInResultsTreeView = null;

        private void twResultsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            c_bResaltar = true; // Ya se puede resaltar
            string m_sIdNodo = e.Node.Tag.ToString();
            Navegar(m_sIdNodo);
            c_CurrentNodeInResultsTreeView = e.Node;
        }

        TreeView twTreeView = null;
        TreeView twResultsTreeView = null;
        WebBrowser wbWebBrowser = null;
        Button btnBuscar = null;
        Button btnVerArbol = null;
        Button btnVerResultados = null;

        const string SQLITE_SHELL_FILE_PATH = ".\\sqlite3_shell.exe";

        string c_sElectronicBookFolderPath = "";
        string c_sRutaArchivoIndice = "";
        System.Collections.SortedList c_NodesListInTreeView = null;

        private void ExpandirNode(TreeNode NodeToExpand)
        {
            if (NodeToExpand != null && NodeToExpand.Parent != null)
            {
                ExpandirNode(NodeToExpand.Parent);
                NodeToExpand.Parent.Expand();
            } // if (NodeToExpand != null && NodeToExpand.Parent != null)
        } // private void ExpandirNode(TreeNode NodeToExpand)

        private void btnVerArbol_Click(object sender, System.EventArgs e)
        {
            c_bResaltar = false; // Todavía no se resalta
            this.twResultsTreeView.Visible = false;
            this.twTreeView.Visible = true;
            this.btnVerArbol.Visible = false;
            this.btnVerResultados.Visible = true;
            this.btnVerResultados.Focus();

            if (c_CurrentNodeInResultsTreeView != null)
            {
                string m_sIdNodo = c_CurrentNodeInResultsTreeView.Tag.ToString();
                int m_iIndex = c_NodesListInTreeView.IndexOfKey(m_sIdNodo);

                if (m_iIndex >= 0)
                {
                    twTreeView.CollapseAll();
                    TreeNode m_CurrentNode = (TreeNode)c_NodesListInTreeView.GetByIndex(m_iIndex);
                    ExpandirNode(m_CurrentNode);
                    twTreeView.SelectedNode = null;
                    twTreeView.SelectedNode = m_CurrentNode;
                } // if (m_iIndex >= 0)
            } // if (c_CurrentNodeInResultsTreeView != null)

            else if (twTreeView.GetNodeCount(false) > 0 && twTreeView.SelectedNode != null)
            {
                TreeNode m_CurrentNode = twTreeView.SelectedNode;
                twTreeView.SelectedNode = null;
                twTreeView.SelectedNode = m_CurrentNode;
            } // else if (twTreeView.GetNodeCount(false) > 0 && twTreeView.SelectedNode != null)

            if (twTreeView.GetNodeCount(false) > 0 && twTreeView.SelectedNode != null)
                twTreeView.Focus();
            
        }

        private void btnVerResultados_Click(object sender, System.EventArgs e)
        {
            c_bResaltar = false; // Todavía no se resalta
            this.twTreeView.Visible = false;
            this.twResultsTreeView.Visible = true;
            this.btnVerResultados.Visible = false;
            this.btnVerArbol.Visible = true;
            this.btnVerArbol.Focus();

            if (twResultsTreeView.GetNodeCount(false) > 0)
            {
                if (c_CurrentNodeInResultsTreeView != null)
                {
                    twResultsTreeView.SelectedNode = null;
                    twResultsTreeView.SelectedNode = c_CurrentNodeInResultsTreeView;
                } // if (c_CurrentNodeInResultsTreeView != null)

                twResultsTreeView.Focus();
            } // if (twResultsTreeView.GetNodeCount(false) > 0)
        }

        private void btnAbrirLibroElectronico_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (!File.Exists(SQLITE_SHELL_FILE_PATH))
                {
                    MessageBox.Show("No existe el archivo " + SQLITE_SHELL_FILE_PATH);
                    return;
                } // if (!File.Exists(SQLITE_SHELL_FILE_PATH))

                FolderBrowserDialog m_FolderDialog = new FolderBrowserDialog();

                if (m_FolderDialog.ShowDialog() != DialogResult.OK)
                    return;

                string m_sProbableNewElectronicBookFolderPath = m_FolderDialog.SelectedPath;

                CArbol m_Arbol = new CArbol(m_sProbableNewElectronicBookFolderPath, SQLITE_SHELL_FILE_PATH);

                if (File.Exists(m_Arbol.GetDatabaseFilePath()))
                {
                    m_Arbol.CargarArbol();

                    //MessageBox.Show("Presione el botón o la tecla ENTER para continuar");
                    bool m_bBotonBuscarVisible = false;

                    string m_sErrorText = m_Arbol.GetErrorText();

                    if (m_sErrorText.Length != 0)
                        MessageBox.Show(m_sErrorText);
                    else
                    {
                        c_bResaltar = false; // Todavía no se resalta
                        System.Collections.SortedList m_ListaNodos = m_Arbol.LoadTreeNodes();
                        c_NodesListInTreeView = this.LoadTreeNodes(m_ListaNodos);
                        this.c_sElectronicBookFolderPath = m_Arbol.GetDatabaseFolderPath();
                        twResultsTreeView.Visible = false;
                        twTreeView.Visible = true;
                        btnVerArbol.Visible = false;
                        btnVerResultados.Visible = false;
                        c_sRutaArchivoIndice = FijarRutaArchivoIndice();
                        c_CurrentNodeInResultsTreeView = null;

                        twTreeView.TabStop = false;

                        if (twTreeView.GetNodeCount(false) > 0)
                        {
                            twTreeView.TabStop = true;
                            twTreeView.Focus();
                        } // if (twTreeView.GetNodeCount(false) > 0)
                    } // else

                    if (twTreeView.GetNodeCount(false) > 0)
                        m_bBotonBuscarVisible = true;

                    btnBuscar.Visible = m_bBotonBuscarVisible;
                }
                else
                    MessageBox.Show("No se encontró el archivo " + m_Arbol.GetDatabaseFilePath());
            }
            catch (System.Exception objException)
            {
                MessageBox.Show(objException.Message);
            }
        }

        const string INDEX_EXTENSION = ".sqlite3";
        const string INDEX_FILE_NAME_WITHOUT_EXTENSION = "Indice";
        const string INDEX_FILE_NAME = INDEX_FILE_NAME_WITHOUT_EXTENSION + INDEX_EXTENSION;

        private string FijarRutaArchivoIndice()
        {
            CIdPublicacion m_IdPublicacion = new CIdPublicacion(c_sElectronicBookFolderPath);
            string m_sRutaArchivoIndice = Path.Combine(c_sElectronicBookFolderPath, INDEX_FILE_NAME);

            if (File.Exists(m_sRutaArchivoIndice))
            {
                string m_sNombreArchivoDestino = m_IdPublicacion.FijarNombreArchivo(INDEX_FILE_NAME_WITHOUT_EXTENSION, INDEX_EXTENSION);
                string m_sNuevaRutaArchivoIndice = Path.Combine(Directory.GetCurrentDirectory(), m_sNombreArchivoDestino);
                File.Copy(m_sRutaArchivoIndice, m_sNuevaRutaArchivoIndice, true);
                m_sRutaArchivoIndice = m_sNuevaRutaArchivoIndice;
            } // if (File.Exists(m_sRutaArchivoIndice))

            return m_sRutaArchivoIndice;
        } // private string FijarRutaArchivoIndice()

        private System.Collections.SortedList LoadTreeNodes(System.Collections.SortedList NodesList)
        {
            this.twTreeView.Nodes.Clear();
            c_sNavigatingToNodeFileName = "";
            this.wbWebBrowser.Navigate("about:blank");
            System.Collections.SortedList m_NodesListInTreeView = new System.Collections.SortedList();

            for (int i = 0; i < NodesList.Count; i++)
            {
                CNodo m_Nodo = (CNodo) NodesList.GetByIndex(i);

                TreeNode m_NewNode = new TreeNode();
                m_NewNode.Tag = m_Nodo.DemeIdNodo();
                m_NewNode.Text = m_Nodo.DemeTitulo();
                
                if (m_Nodo.DemeIdPadre() == "-1")
                    twTreeView.Nodes.Add(m_NewNode);
                else
                {
                    int m_iIndexOfIdPadre = m_NodesListInTreeView.IndexOfKey(m_Nodo.DemeIdPadre().ToString());
                    TreeNode m_ParentNode = (TreeNode) m_NodesListInTreeView.GetByIndex(m_iIndexOfIdPadre);
                    m_ParentNode.Nodes.Add(m_NewNode);
                } // else

                m_NodesListInTreeView.Add(m_Nodo.DemeIdNodo(), m_NewNode);
            } // for (int i = 0; i < NodesList.Count; i++ )  {

            return m_NodesListInTreeView;
        } // private System.Collections.SortedList LoadTreeNodes(System.Collections.SortedList NodesList)

        private void LoadResultNodes(System.Collections.SortedList NodesList)
        {
            this.twResultsTreeView.Nodes.Clear();
            c_sNavigatingToNodeFileName = "";
            this.wbWebBrowser.Navigate("about:blank");
            
            for (int i = 0; i < NodesList.Count; i++)
            {
                CNodo m_Nodo = (CNodo)NodesList.GetByIndex(i);
                int m_iIndexOfIdNodo = c_NodesListInTreeView.IndexOfKey(m_Nodo.DemeIdNodo().ToString());

                if (m_iIndexOfIdNodo >= 0)
                {
                    TreeNode m_OriginalNode = (TreeNode) c_NodesListInTreeView.GetByIndex(m_iIndexOfIdNodo);
                    m_Nodo.FijarTitulo(m_OriginalNode.Text);
                    TreeNode m_NewNode = new TreeNode();
                    m_NewNode.Tag = m_Nodo.DemeIdNodo();
                    m_NewNode.Text = m_Nodo.DemeTitulo();
                    twResultsTreeView.Nodes.Add(m_NewNode);
                } // if (m_iIndexOfIdNodo >= 0)    
            } // for (int i = 0; i < NodesList.Count; i++ )  {
        } // private void LoadResultNodes(System.Collections.SortedList NodesList)

        #endregion
    }
}