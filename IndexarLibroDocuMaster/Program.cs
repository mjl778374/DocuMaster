#define SO_LINUX

using System;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace DocuMaster
{    
    class Program
    {
        const string TREE_FILE_NAME = "Arbol.sqlite3";
        public const string INDEX_FILE_NAME = "Indice.sqlite3";

        #if SO_LINUX
            const string PROCESS_FILE_NAME = "dotnet ./IndexarLibroDocuMaster.dll";
            public const string SQLITE_SHELL_FILE_PATH = "./sqlite3_shell";
        #elif SO_WINDOWS
            const string PROCESS_FILE_NAME = "dotnet .\\IndexarLibroDocuMaster.dll";
            public const string SQLITE_SHELL_FILE_PATH = ".\\sqlite3_shell.exe";
        #endif

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Utilice: " + PROCESS_FILE_NAME + " CARPETA_LIBRO_ELECTRONICO");
                return;
            } // if (args.Length != 1)

            string m_sCarpetaLibroElectronico = args[0];

            if (!Directory.Exists(m_sCarpetaLibroElectronico))
            {
                Console.WriteLine("No existe la carpeta " + m_sCarpetaLibroElectronico);
                return;
            } // if (!Directory.Exists(m_sCarpetaLibroElectronico))

            CArbol m_Arbol = new CArbol(m_sCarpetaLibroElectronico, SQLITE_SHELL_FILE_PATH);
            string m_sRutaArchivoArbol = m_Arbol.GetDatabaseFilePath();

            if (!File.Exists(m_sRutaArchivoArbol))
            {
                Console.WriteLine("No existe el archivo " + m_sRutaArchivoArbol);
                return;
            } // if (!File.Exists(m_sRutaArchivoArbol))

            string m_sRutaArchivoIndice = Path.Combine(Directory.GetCurrentDirectory(), INDEX_FILE_NAME);

            if (!File.Exists(m_sRutaArchivoIndice))
            {
                Console.WriteLine("No existe el archivo " + m_sRutaArchivoIndice);
                return;
            } // if (!File.Exists(m_sRutaArchivoIndice))

            if (!File.Exists(SQLITE_SHELL_FILE_PATH))
            {
                Console.WriteLine("No existe el archivo " + SQLITE_SHELL_FILE_PATH);
                return;
            } // if (!File.Exists(SQLITE_SHELL_FILE_PATH))

            try {
                m_Arbol.CargarArbol();
                //Thread.Sleep(20);
                string m_sErrorText = m_Arbol.GetErrorText();

                if (m_sErrorText.Length != 0)
                    Console.WriteLine(m_sErrorText);
                else
                {
                    System.Collections.SortedList m_ListaNodos = m_Arbol.LoadTreeNodes();
                    CIndexar m_Indexar = new CIndexar(m_sRutaArchivoIndice, m_ListaNodos, m_sCarpetaLibroElectronico);
                    m_Indexar.Ejecutar();
                } // else

/*
                int m_iNextPosition = 0;
                string m_sNextToken = GetNextWord("<body AbC  > cátodo<td> canciÓn </td> yigÜirro <def   ", ref m_iNextPosition);
                Console.WriteLine("{0}, {1}, {2}", m_sNextToken, m_sNextToken.Length, m_iNextPosition);

                m_sNextToken = GetNextWord("<body AbC  > cátodo<td> canciÓn </td> yigÜirro <def   ", ref m_iNextPosition);
                Console.WriteLine("{0}, {1}, {2}", m_sNextToken, m_sNextToken.Length, m_iNextPosition);

                m_sNextToken = GetNextWord("<body AbC  > cátodo<td> canciÓn </td> yigÜirro <def   ", ref m_iNextPosition);
                Console.WriteLine("{0}, {1}, {2}", m_sNextToken, m_sNextToken.Length, m_iNextPosition);

                m_sNextToken = GetNextWord("<body AbC  > cátodo<td> canciÓn </td> yigÜirro <def   ", ref m_iNextPosition);
                Console.WriteLine("{0}, {1}, {2}", m_sNextToken, m_sNextToken.Length, m_iNextPosition);

                m_sNextToken = GetNextWord("<body AbC  > cátodo<td> canciÓn </td> yigÜirro <def   ", ref m_iNextPosition);
                Console.WriteLine("{0}, {1}, {2}", m_sNextToken, m_sNextToken.Length, m_iNextPosition);
                */
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
