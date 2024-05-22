using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace DocuMaster
{
    class CArbol
    {
        const string DATABASE_FILE_NAME = "Arbol.sqlite3";
        const string SQL_QUERY_FOR_LOAD_NODES = "select IdPadre, IdNodo, Titulo from Arbol order by IdNodo";
        //const string SQL_QUERY_FOR_LOAD_NODES = "select IdNodo from Arbol where Titulo like '%\"%' order by IdNodo";

        string c_sDatabaseFolderPath;
        string c_sSQLiteShellFilePath;

        public CArbol(String sDatabaseFolderPath, String sSQLiteShellFilePath)
        {
            c_sDatabaseFolderPath = sDatabaseFolderPath;
            c_sSQLiteShellFilePath = sSQLiteShellFilePath;
        } // CArbol(String sDatabaseFolderPath)

        public string GetDatabaseFolderPath()
        {
            return c_sDatabaseFolderPath;
        }
        public string GetDatabaseFilePath()
        {
            return Path.Combine(GetDatabaseFolderPath(), DATABASE_FILE_NAME);
        } // public string GetDatabaseFilePath()

        public void CargarArbol()
        {
            string m_sQuery = SQL_QUERY_FOR_LOAD_NODES;
            string m_sParameters = CProcess.FormatParameter(GetDatabaseFilePath()) + " " + CProcess.FormatParameter(m_sQuery);
            CProcess.ExecuteCommand(c_sSQLiteShellFilePath, m_sParameters);
        } // public void CargarArbol()

        public string GetErrorText()
        {   
            string m_sErrorText = CIO.GetErrorText();
            return m_sErrorText;
        } // private void DisplayErrors(System.Collections.Generic.IEnumerable<string> ErrorLines)
        
        public System.Collections.SortedList LoadTreeNodes()
        {
            System.Collections.SortedList m_NodesList = new System.Collections.SortedList();

            System.Collections.Generic.IEnumerable<string> m_Results = CIO.LoadResultsFile();
            int m_iCounter = 0;
            int m_iNumberOfColumns = 3;

            string m_sIdPadre = "";
            string m_sIdNodo = "";
            string m_sTitulo = "";

            foreach (var m_sLine in m_Results)
            {
                if ((m_iCounter % m_iNumberOfColumns) == 0)
                    m_sIdPadre = m_sLine;

                else if ((m_iCounter % m_iNumberOfColumns) == 1)
                    m_sIdNodo = m_sLine;

                else if ((m_iCounter % m_iNumberOfColumns) == 2)
                    m_sTitulo = m_sLine;

                m_iCounter += 1;

                if ((m_iCounter % m_iNumberOfColumns) == 0)
                {
                    CNodo m_Nodo = new CNodo(m_sIdPadre, m_sIdNodo, m_sTitulo);
                    m_NodesList.Add(m_NodesList.Count, m_Nodo);
                } // if ((m_iCounter % m_iNumberOfColumns) == 0)
            } // foreach (var m_sLine in m_Results)
            
            return m_NodesList;
        } // public System.Collections.SortedList LoadTreeNodes()
    }
}
