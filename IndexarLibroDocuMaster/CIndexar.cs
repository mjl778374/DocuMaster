using System;
using System.IO;
using System.Diagnostics;
using System.Timers;
using System.Threading;

namespace DocuMaster
{
    class CIndexar
    {
        string c_sRutaArchivoIndice = "";
        System.Collections.SortedList c_ListaNodos = null;
        string c_sCarpetaLibroElectronico = "";

        public CIndexar(string sRutaArchivoIndice, System.Collections.SortedList ListaNodos, string sCarpetaLibroElectronico)
        {
            c_sRutaArchivoIndice = sRutaArchivoIndice;
            c_ListaNodos = ListaNodos;
            c_sCarpetaLibroElectronico = sCarpetaLibroElectronico;
        }
        public void Ejecutar()
        {
            string m_sNuevaRutaArchivoIndice = Path.Combine(c_sCarpetaLibroElectronico, Program.INDEX_FILE_NAME);
            Console.WriteLine("Copiando el archivo " + c_sRutaArchivoIndice + " en " + m_sNuevaRutaArchivoIndice);
            File.Copy(c_sRutaArchivoIndice, m_sNuevaRutaArchivoIndice, true);

            Console.WriteLine("Número de archivos por indexar: " + c_ListaNodos.Count);

            System.Collections.SortedList m_ListaTodasPalabras = new System.Collections.SortedList();
            int m_iNumSiguientePalabra = 0;

            for (int i = 0; i < c_ListaNodos.Count; i++)
            {
                CNodo m_Nodo = (CNodo) c_ListaNodos.GetByIndex(i);
                int m_iIdNodo = Int32.Parse(m_Nodo.DemeIdNodo());
                IndexarNodo(m_iIdNodo, m_sNuevaRutaArchivoIndice, m_ListaTodasPalabras, ref m_iNumSiguientePalabra);
            } // for (int i = 0; i < c_ListaNodos.Count; i++)
        } // public void Ejecutar()

        const string EXTENSION_ARCHIVOS_X_INDEXAR = ".htm";

        private void IndexarNodo(int iIdNodo, string sRutaArchivoIndice, System.Collections.SortedList ListaTodasPalabras, ref int iNumSiguientePalabra)
        {
            string m_sNombreArchivoXIndexar = iIdNodo + EXTENSION_ARCHIVOS_X_INDEXAR;
            string m_sRutaArchivoXIndexar = Path.Combine(c_sCarpetaLibroElectronico, m_sNombreArchivoXIndexar);
            Console.WriteLine("Procesando el archivo: " + m_sRutaArchivoXIndexar);
            string m_sContenidoArchivoXIndexar = CIO.GetTextInFile(m_sRutaArchivoXIndexar);
            const string BODY_TAG = "<body ";
            int m_iPosicionInicio = m_sContenidoArchivoXIndexar.ToLower().IndexOf(BODY_TAG.ToLower(), 0);

            System.Collections.SortedList m_ListaPalabrasEnNodo = new System.Collections.SortedList();

            if (m_iPosicionInicio >= 0)
            {
                string m_sNextToken = GetNextWord(m_sContenidoArchivoXIndexar, ref m_iPosicionInicio);

                while (m_sNextToken.Length > 0)
                {
                    string m_sPalabraXInsertar = m_sNextToken.ToLower();

                    if (ListaTodasPalabras.IndexOfKey(m_sPalabraXInsertar) < 0)
                    {
                        iNumSiguientePalabra += 1;
                        ListaTodasPalabras.Add(m_sPalabraXInsertar, iNumSiguientePalabra);
                        InsertarPalabraEnIndice(iNumSiguientePalabra, m_sPalabraXInsertar, sRutaArchivoIndice);
                    } // if (ListaTodasPalabras.IndexOfKey(m_sPalabraXInsertar) < 0)
                    
                    int m_iIndexOfIdPalabra = ListaTodasPalabras.IndexOfKey(m_sPalabraXInsertar);
                    int m_iIdPalabra = (int) ListaTodasPalabras.GetByIndex(m_iIndexOfIdPalabra);

                    if (m_ListaPalabrasEnNodo.IndexOfKey(m_iIdPalabra) < 0)
                    {
                        m_ListaPalabrasEnNodo.Add(m_iIdPalabra, m_iIdPalabra);
                        RelacionarIdNodoIdPalabra(iIdNodo, m_iIdPalabra, sRutaArchivoIndice);
                    } // if (m_ListaPalabrasEnNodo.IndexOfKey(m_iIdPalabra) < 0)

                    //Console.WriteLine(m_sNextToken + ", " + m_iIdPalabra + ", " + m_bBitAgregado);
                    m_sNextToken = GetNextWord(m_sContenidoArchivoXIndexar, ref m_iPosicionInicio);
                } // while (m_sNextToken.Length > 0)
            } // if (m_iPosicionInicio >= 0)
        } // private void IndexarNodo(int iIdNodo, string sRutaArchivoIndice, System.Collections.SortedList ListaTodasPalabras, ref int iNumSiguientePalabra)

        private System.Timers.Timer c_Timer;
        //private Boolean c_bContinuarConProceso;

        private void SetTimer()
        {
            //c_bContinuarConProceso = false;
            // Create a timer with a two second interval.
            c_Timer = new System.Timers.Timer(1);
            // Hook up the Elapsed event for the timer. 
            c_Timer.Elapsed += OnTimedEvent;
            c_Timer.AutoReset = true;
            c_Timer.Enabled = true;
        } // private void SetTimer()

        private string c_sParametersForSQLCommand = "";

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            CProcess.ExecuteCommand(Program.SQLITE_SHELL_FILE_PATH, c_sParametersForSQLCommand);
            c_Timer.Stop();
            c_Timer.Dispose();
            //c_bContinuarConProceso = true;
        } // private void OnTimedEvent(Object source, ElapsedEventArgs e)

        private void EjecutarComandoSQL(string sSQL, string sDatabaseFilePath)
        {
            //c_sParametersForSQLCommand = CProcess.FormatParameter(sDatabaseFilePath) + " " + CProcess.FormatParameter(sSQL);
            string m_sParameters = CProcess.FormatParameter(sDatabaseFilePath) + " " + CProcess.FormatParameter(sSQL);
            CProcess.ExecuteCommand(Program.SQLITE_SHELL_FILE_PATH, m_sParameters);
            string m_sErrorText = CIO.GetErrorText();

            if (m_sErrorText.Length > 0)
                Console.WriteLine(m_sErrorText);
            //SetTimer();
            //CProcess.ExecuteCommand(Program.SQLITE_SHELL_FILE_PATH, c_sParametersForSQLCommand);
            //Console.ReadKey();
            //Thread.Sleep(2000);
        } // private void EjecutarComandoSQL(string sSQL, string sDatabaseFilePath)

        private void RelacionarIdNodoIdPalabra(int iIdNodo, int iIdPalabra, string sDatabaseFilePath)
        {
            string m_sSQL = "insert into PalabrasXNodo(IdPalabra, IdNodo) values(" + iIdPalabra + ", " + iIdNodo + ")";
            //string m_sParameters = CProcess.FormatParameter(sDatabaseFilePath) + " " + CProcess.FormatParameter(m_sSQL);
            EjecutarComandoSQL(m_sSQL, sDatabaseFilePath);
        } // private void InsertarPalabraEnIndice(int iIdPalabra, string sPalabra, string sDatabaseFilePath)

        private void InsertarPalabraEnIndice(int iIdPalabra, string sPalabra, string sDatabaseFilePath)
        {
            string m_sSQL = "insert into Palabras(IdPalabra, Palabra) values(" + iIdPalabra + ", " + scm(sPalabra) + ")";
            //string m_sParameters = CProcess.FormatParameter(sDatabaseFilePath) + " " + CProcess.FormatParameter(m_sSQL);
            EjecutarComandoSQL(m_sSQL, sDatabaseFilePath);
        } // private void InsertarPalabraEnIndice(int iIdPalabra, string sPalabra, string sDatabaseFilePath)

        private string scm(String sDato)
        {
            return "'" + sDato.Replace("'", "''") + "'";
        } // private string scm(String sDato)

        const string CARACTERES_VALIDOS_EN_PALABRA = "abcdefghijklmnñopqrstuvwxyzáéíóúü0123456789";
        const string INITIAL_CHARACTER_OF_TAG = "<";
        const string END_CHARACTER_OF_TAG = ">";

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
                if (String.Compare(m_sNextChar, INITIAL_CHARACTER_OF_TAG) == 0)
                    ExcludeTag(sInText, ref m_iNextPosition);
                else
                    m_iNextPosition += 1;

                if (m_iNextPosition < sInText.Length)
                    m_sNextChar = sInText.Substring(m_iNextPosition, 1);
            } // while(m_iNextPosition < sInText.Length ...

            while(m_iNextPosition < sInText.Length
            && CARACTERES_VALIDOS_EN_PALABRA.IndexOf(m_sNextChar.ToLower(), 0) >= 0)
            {
                m_sNextToken = m_sNextToken + m_sNextChar;
                m_iNextPosition += 1;

                if (m_iNextPosition < sInText.Length)
                    m_sNextChar = sInText.Substring(m_iNextPosition, 1);
            } // while(m_iNextPosition < sInText.Length ...

            iInitialPosition = m_iNextPosition;
            return m_sNextToken;
        } // private string GetNextWord(string sInText, ref int iInitialPosition)

        private void ExcludeTag(string sInText, ref int iInitialPosition)
        {
            int m_iNextPosition = iInitialPosition;
            string m_sNextChar = "";
            
            if (m_iNextPosition < sInText.Length)
                m_sNextChar = sInText.Substring(m_iNextPosition, 1);

            while(m_iNextPosition < sInText.Length && END_CHARACTER_OF_TAG.IndexOf(m_sNextChar.ToLower(), 0) < 0)
            {
                m_iNextPosition += 1;

                if (m_iNextPosition < sInText.Length)
                    m_sNextChar = sInText.Substring(m_iNextPosition, 1);
            } // while(m_iNextPosition < sInText.Length && END_CHARACTER_OF_TAG.IndexOf(m_sNextChar.ToLower(), 0) < 0)

            iInitialPosition = m_iNextPosition;
        } // private void ExcludeTag(string sInText, ref int iInitialPosition)
    } // class CIndexar
}