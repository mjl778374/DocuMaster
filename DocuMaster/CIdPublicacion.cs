using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DocuMaster
{
    class CIdPublicacion
    {
        const string NOMBRE_ARCHIVO_ID_PUBLICACION = "IdPublicacion.txt";
        string c_sRutaCarpetaLibroElectronico = "";
        public CIdPublicacion(string sRutaCarpetaLibroElectronico)
        {
            c_sRutaCarpetaLibroElectronico = sRutaCarpetaLibroElectronico;
        } // CArbol(String sDatabaseFolderPath)

        private string DemeRutaArchivoIdPublicacion()
        {
            return Path.Combine(c_sRutaCarpetaLibroElectronico, NOMBRE_ARCHIVO_ID_PUBLICACION);
        } // private string DemeRutaArchivoIdPublicacion()

        private bool EsNombreArchivoValido(string sNombreArchivo)
        {
            const string CARACTERES_VALIDOS = "abcdefghijklmnopqrstuvwxyz0123456789-";

            bool m_bEsNombreValido = true;

            if (sNombreArchivo.Trim().Length == 0)
                m_bEsNombreValido = false;

            int i = 0;
            while (i < sNombreArchivo.Length && m_bEsNombreValido)
            {
                if (CARACTERES_VALIDOS.ToLower().IndexOf(sNombreArchivo.Substring(i, 1).ToLower()) < 0)
                    m_bEsNombreValido = false;

                i += 1;
            } // while (i < sNombreArchivo.Length && m_bEsNombreValido)

            return m_bEsNombreValido;
        } // private bool EsNombreArchivoValido(string sNombreArchivo)

        public string FijarNombreArchivo(String sNombreArchivoXDefectoSinExtension, string sExtensionArchivo)
        {
            string m_sNombreArchivoXRetornar = CIO.GetTextInFile(DemeRutaArchivoIdPublicacion());

            if (EsNombreArchivoValido(m_sNombreArchivoXRetornar))
                m_sNombreArchivoXRetornar = m_sNombreArchivoXRetornar + sExtensionArchivo;
            else
                m_sNombreArchivoXRetornar = sNombreArchivoXDefectoSinExtension + sExtensionArchivo;

            return m_sNombreArchivoXRetornar;
        } // public string FijarNombreArchivo(String sNombreArchivoXDefectoSinExtension, string sExtensionArchivo)
    }
}
