using System;
using System.IO;
using System.Diagnostics;
using System.Timers;
using System.Threading;

namespace DocuMaster
{
    class CBuscar
    {
        string c_sRutaArchivoIndice = "";
        string c_sSQLiteShellFilePath = "";
        System.Collections.Generic.List<string> c_ListaPalabrasXBuscar = null;

        public CBuscar(string sRutaArchivoIndice, String sSQLiteShellFilePath)
        {
            c_sRutaArchivoIndice = sRutaArchivoIndice;
            c_sSQLiteShellFilePath = sSQLiteShellFilePath;
        } // public CBuscar(string sRutaArchivoIndice, String sSQLiteShellFilePath)

        private void EjecutarComandoSQL(string sSQL)
        {
            string[] m_Parameters = { c_sRutaArchivoIndice, sSQL };
            CProcess.ExecuteCommand(c_sSQLiteShellFilePath, m_Parameters);
        } // private void EjecutarComandoSQL(string sSQL)

        public void ConsultarXPalabras(string sPalabras)
        {
            c_ListaPalabrasXBuscar = new System.Collections.Generic.List<string>();
            string m_sSQL = "select count(1) as NumAciertos, a.IdNodo from PalabrasXNodo a, Palabras b";
            m_sSQL = m_sSQL + " where a.IdPalabra = b.IdPalabra";
            m_sSQL = m_sSQL + " and a.IdPalabra = b.IdPalabra";
            m_sSQL = m_sSQL + " and (1 = 0";

            int m_iNextPosition = 0;
            string m_sSiguientePalabra = GetNextWord(sPalabras, ref m_iNextPosition);

            while (m_sSiguientePalabra.Length > 0)
            {
                if (!c_ListaPalabrasXBuscar.Contains(m_sSiguientePalabra.ToLower()))
                    c_ListaPalabrasXBuscar.Add(m_sSiguientePalabra.ToLower());

                m_sSQL = m_sSQL + " or b.Palabra like " + scm(m_sSiguientePalabra + "%");
                m_sSiguientePalabra = GetNextWord(sPalabras, ref m_iNextPosition);
            } // while (m_sSiguientePalabra.Length > 0)

            m_sSQL = m_sSQL + ")";
            m_sSQL = m_sSQL + " group by a.IdNodo";
            m_sSQL = m_sSQL + " order by NumAciertos desc, a.IdNodo asc";

            EjecutarComandoSQL(m_sSQL);
        } // public void ConsultarXPalabras(string sPalabras)

        public System.Collections.Generic.List<string> PalabrasBuscadas()
        {
            return c_ListaPalabrasXBuscar;
        } // public System.Collections.Generic.List<string> PalabrasBuscadas()
        public string DemeCaracteresValidos()
        {
            return CARACTERES_VALIDOS_EN_PALABRA;
        } // public string DemeCaracteresValidos()

        public string GetErrorText()
        {
            string m_sErrorText = CIO.GetErrorText();
            return m_sErrorText;
        } // public string GetErrorText()

        public System.Collections.SortedList LoadTreeNodes()
        {
            System.Collections.SortedList m_NodesList = new System.Collections.SortedList();

            System.Collections.Generic.IEnumerable<string> m_Results = CIO.LoadResultsFile();
            int m_iCounter = 0;
            int m_iNumberOfColumns = 2;

            string m_sIdPadre = "";
            string m_sIdNodo = "";
            string m_sTitulo = "";
            string m_sNumAciertos = "";

            foreach (var m_sLine in m_Results)
            {
                if ((m_iCounter % m_iNumberOfColumns) == 0)
                    m_sNumAciertos = m_sLine;

                else if ((m_iCounter % m_iNumberOfColumns) == 1)
                    m_sIdNodo = m_sLine;

                m_iCounter += 1;

                if ((m_iCounter % m_iNumberOfColumns) == 0)
                {
                    CNodo m_Nodo = new CNodo(m_sIdPadre, m_sIdNodo, m_sTitulo);
                    m_NodesList.Add(m_NodesList.Count, m_Nodo);
                } // if ((m_iCounter % m_iNumberOfColumns) == 0)
            } // foreach (var m_sLine in m_Results)

            return m_NodesList;
        } // public System.Collections.SortedList LoadTreeNodes()
        private string scm(String sDato)
        {
            return "'" + sDato.Replace("'", "''") + "'";
        } // private string scm(String sDato)

        const string CARACTERES_VALIDOS_EN_PALABRA = "abcdefghijklmnñopqrstuvwxyzáéíóúü0123456789";

        private string GetNextWord(string sInText, ref int iInitialPosition)
        {
            string m_sNextToken = "";
            int m_iNextPosition = iInitialPosition;
            string m_sNextChar = "";
            
            if (m_iNextPosition < sInText.Length)
                m_sNextChar = sInText.Substring(m_iNextPosition, 1);

            while(m_iNextPosition < sInText.Length
            && CARACTERES_VALIDOS_EN_PALABRA.IndexOf(m_sNextChar.ToLower(), 0) < 0)
            {
                m_iNextPosition += 1;

                if (m_iNextPosition < sInText.Length)
                    m_sNextChar = sInText.Substring(m_iNextPosition, 1);
            } // while(m_iNextPosition < sInText.Length

            while(m_iNextPosition < sInText.Length && CARACTERES_VALIDOS_EN_PALABRA.IndexOf(m_sNextChar.ToLower(), 0) >= 0)
            {
                m_sNextToken = m_sNextToken + m_sNextChar;
                m_iNextPosition += 1;

                if (m_iNextPosition < sInText.Length)
                    m_sNextChar = sInText.Substring(m_iNextPosition, 1);
            } // while(m_iNextPosition < sInText.Length && CARACTERES_VALIDOS_EN_PALABRA.IndexOf(m_sNextChar.ToLower(), 0) >= 0)

            iInitialPosition = m_iNextPosition;
            return m_sNextToken;
        } // private string GetNextWord(string sInText, ref int iInitialPosition)
   } // class CBuscar
}