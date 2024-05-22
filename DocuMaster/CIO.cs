using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace DocuMaster
{
    class CIO
    {        
        const string RESULTS_FILE_PATH = "results.txt";
        const string ERRORS_FILE_PATH = "errors.txt";
                
        public static System.Collections.Generic.IEnumerable<string> LoadFile(string sFilePath)
        {
            if (!File.Exists(sFilePath))
                return Enumerable.Empty<string>();

            using (FileStream m_fs = File.OpenRead(sFilePath))
                return File.ReadLines(sFilePath);
        } // public static System.Collections.Generic.IEnumerable<string> LoadFile(string sFilePath)
        
        public static string GetErrorText()
        {
            string m_sErrorText = GetTextInFile(ERRORS_FILE_PATH);
            return m_sErrorText;
        } // public static string GetErrorText()

        public static string GetTextInFile(string sFilePath)
        {
            System.Collections.Generic.IEnumerable<string> m_Lines = LoadFile(sFilePath);
            string m_sData = "";

            foreach (var m_sLine in m_Lines)
            {
                if (m_sData.Length > 0)
                    m_sData = m_sData + "\n";

                m_sData = m_sData + m_sLine;
            } // foreach (var m_sLine in m_Lines)

            return m_sData;
        } // public static string GetTextInFile(string sFilePath)
        
        public static System.Collections.Generic.IEnumerable<string> LoadResultsFile()
        {
            System.Collections.Generic.IEnumerable<string> m_Results = LoadFile(RESULTS_FILE_PATH);
            return m_Results;
        } // public static System.Collections.Generic.IEnumerable<string> LoadResultsFile()
    }
}
