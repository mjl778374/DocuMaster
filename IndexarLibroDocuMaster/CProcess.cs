using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DocuMaster
{
    class CProcess
    {
        public static void ExecuteCommand(string sCommand, string sParameters)
        {
            Console.WriteLine("Ejecutando el comando: " + sCommand + " " + sParameters);
            
            Process m_Proceso = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = sCommand,
                    Arguments = sParameters,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            m_Proceso.Start();
            m_Proceso.WaitForExit();
        } // public static void ExecuteCommand(string sCommand, string sParameters)

        public static string FormatParameter(String sParameter)
        {
            return "\"" + sParameter.Replace("\"", "\\\"") + "\"";
        } // public static string FormatParameter(String sParameter)
    }
}
