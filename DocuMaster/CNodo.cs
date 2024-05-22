using System;
using System.Collections.Generic;
using System.Text;

namespace DocuMaster
{
    class CNodo
    {
        string c_sIdPadre;
        string c_sIdNodo;
        string c_sTitulo;

        public CNodo(string sIdPadre, string sIdNodo, string sTitulo)
        {
            c_sIdPadre = sIdPadre;
            c_sIdNodo = sIdNodo;
            c_sTitulo = sTitulo;
        } // public CNodo(string sIdPadre, string sIdNodo, string sTitulo)

        public void FijarTitulo(string sTitulo)
        {
            c_sTitulo = sTitulo;
        } // public void FijarTitulo(string sTitulo)

        public string DemeIdPadre()
        {
            return c_sIdPadre;
        }
        public string DemeIdNodo()
        {
            return c_sIdNodo;
        }
        
        public string DemeTitulo()
        {
            return c_sTitulo;
        } // public string DemeTitulo()
    }
}
