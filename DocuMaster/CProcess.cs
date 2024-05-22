using System;
using System.Collections.Generic;
using System.Text;

namespace DocuMaster
{
    class CProcess
    {
        public static void ExecuteCommand(string sCommand, string[] Parameters)
        {
            System.Diagnostics.Process m_Process = new System.Diagnostics.Process();
            m_Process.StartInfo.FileName = sCommand;
            m_Process.StartInfo.CreateNoWindow = true;

            foreach (string m_sParameter in Parameters)
                m_Process.StartInfo.ArgumentList.Add(m_sParameter);

            m_Process.Start();
            m_Process.WaitForExit();
        } // public static void ExecuteCommand(string sCommand, string[] Parameters)
    }
}
