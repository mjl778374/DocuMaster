using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuMaster
{
    enum TiposToken
    {
        Ninguno,
        PalabraValida,
        CaracteresInvalidos,
        Etiqueta
    } // enum TiposToken

    class CResaltarTextoHTML
    {
        System.Collections.Generic.List<string> c_ListaPalabrasBuscadas = null;
        string c_sCaracteresValidos = "";

        public CResaltarTextoHTML(System.Collections.Generic.List<string> ListaPalabrasBuscadas, string sCaracteresValidos)
        {
            c_ListaPalabrasBuscadas = ListaPalabrasBuscadas;
            c_sCaracteresValidos = sCaracteresValidos;
        } // public CResaltarTextoHTML(System.Collections.Generic.List<string> ListaPalabrasBuscadas, string sCaracteresValidos)

        public string Resaltar(string sTexto, string sSeparadorTokens)
        {
            string m_sTextoResaltado = "";
            int m_iNextPosition = 0;
            TiposToken m_TipoToken = TiposToken.Ninguno;
            string m_sNextToken = GetNextToken(sTexto, ref m_iNextPosition, out m_TipoToken);

            while(m_sNextToken.Length > 0)
            {
                if (m_TipoToken == TiposToken.PalabraValida && EsUnaPalabraBuscada(m_sNextToken))
                    m_sNextToken = ResaltarPalabra(m_sNextToken);

                m_sTextoResaltado += m_sNextToken;
                m_sTextoResaltado += sSeparadorTokens;

                m_sNextToken = GetNextToken(sTexto, ref m_iNextPosition, out m_TipoToken);
            } // while(m_sNextToken.Length > 0)

            return m_sTextoResaltado;
        } // public string Resaltar(string sTexto, string sSeparadorTokens)

        private bool EsUnaPalabraBuscada(string sPalabra)
        {
            int i = 0;
            bool m_bEsUnaPalabraBuscada = false;

            while (i < c_ListaPalabrasBuscadas.Count && !m_bEsUnaPalabraBuscada)
            {
                if (sPalabra.ToLower().IndexOf(c_ListaPalabrasBuscadas[i].ToLower()) == 0)
                    m_bEsUnaPalabraBuscada = true;

                i += 1;
            } // while (i < c_ListaPalabrasBuscadas.Count && !m_bEsUnaPalabraBuscada)

            return m_bEsUnaPalabraBuscada;
        }
        private string ResaltarPalabra(string sPalabra)
        {
            const String APERTURA_ETIQUETA_RESALTADO = "<span style=\"color:red;font-weight:bold;\">";
            const String CIERRE_ETIQUETA_RESALTADO = "</span>";
            String m_sPalabraResaltada = APERTURA_ETIQUETA_RESALTADO + sPalabra + CIERRE_ETIQUETA_RESALTADO;
            return m_sPalabraResaltada;
        } // private string ResaltarPalabra(string sPalabra)

        private string GetNextToken(string sInText, ref int iInitialPosition, out TiposToken TipoToken)
        {
            string m_sNextToken = "";
            int m_iNextPosition = iInitialPosition;
            string m_sNextChar = "";
            TipoToken = TiposToken.Ninguno;

            if (m_iNextPosition < sInText.Length)
            {
                m_sNextChar = sInText.Substring(m_iNextPosition, 1);

                if (c_sCaracteresValidos.IndexOf(m_sNextChar.ToLower(), 0) >= 0)
                {
                    m_sNextToken = GetNextValidWord(sInText, ref m_iNextPosition);
                    TipoToken = TiposToken.PalabraValida;
                } // if (c_sCaracteresValidos.IndexOf(m_sNextChar.ToLower(), 0) >= 0)

                else if (CARACTER_APERTURA_ETIQUETAS.IndexOf(m_sNextChar.ToLower(), 0) >= 0)
                {
                    m_sNextToken = GetNextTag(sInText, ref m_iNextPosition);
                    TipoToken = TiposToken.Etiqueta;
                } // else if (CARACTER_APERTURA_ETIQUETAS.IndexOf(m_sNextChar.ToLower(), 0) >= 0)

                else if (c_sCaracteresValidos.IndexOf(m_sNextChar.ToLower(), 0) < 0)
                {
                    m_sNextToken = GetNextInvalidChars(sInText, ref m_iNextPosition);
                    TipoToken = TiposToken.CaracteresInvalidos;
                } // else if (c_sCaracteresValidos.IndexOf(m_sNextChar.ToLower(), 0) < 0)                
            } // if (m_iNextPosition < sInText.Length)

            iInitialPosition = m_iNextPosition;
            return m_sNextToken;
        } // private string GetNextToken(string sInText, ref int iInitialPosition, out TiposToken TipoToken)

        private string GetNextValidWord(string sInText, ref int iInitialPosition)
        {
            string m_sNextValidWord = "";
            int m_iNextPosition = iInitialPosition;
            string m_sNextChar = "";

            if (m_iNextPosition < sInText.Length)
                m_sNextChar = sInText.Substring(m_iNextPosition, 1);

            while (m_iNextPosition < sInText.Length && c_sCaracteresValidos.IndexOf(m_sNextChar.ToLower(), 0) >= 0)
            {
                m_sNextValidWord = m_sNextValidWord + m_sNextChar;
                m_iNextPosition += 1;

                if (m_iNextPosition < sInText.Length)
                    m_sNextChar = sInText.Substring(m_iNextPosition, 1);
            } // while(m_iNextPosition < sInText.Length && CARACTERES_VALIDOS_EN_PALABRA.IndexOf(m_sNextChar.ToLower(), 0) >= 0)

            iInitialPosition = m_iNextPosition;
            return m_sNextValidWord;
        } // private string GetNextValidWord(string sInText, ref int iInitialPosition)

        const string CARACTER_APERTURA_ETIQUETAS = "<";
        const string CARACTER_CIERRE_ETIQUETAS = ">";

        private string GetNextInvalidChars(string sInText, ref int iInitialPosition)
        {
            string m_sAllCharsToExclude = c_sCaracteresValidos + CARACTER_APERTURA_ETIQUETAS;
            string m_sNextInvalidChars = "";
            int m_iNextPosition = iInitialPosition;
            string m_sNextChar = "";

            if (m_iNextPosition < sInText.Length)
                m_sNextChar = sInText.Substring(m_iNextPosition, 1);

            while (m_iNextPosition < sInText.Length
            && m_sAllCharsToExclude.IndexOf(m_sNextChar.ToLower(), 0) < 0)
            {
                m_sNextInvalidChars = m_sNextInvalidChars + m_sNextChar;
                m_iNextPosition += 1;

                if (m_iNextPosition < sInText.Length)
                    m_sNextChar = sInText.Substring(m_iNextPosition, 1);
            } // while(m_iNextPosition < sInText.Length

            iInitialPosition = m_iNextPosition;
            return m_sNextInvalidChars;
        } // private string GetNextInvalidChars(string sInText, ref int iInitialPosition)
        private string GetNextTag(string sInText, ref int iInitialPosition)
        {
            string m_sNextTag = "";
            int m_iNextPosition = iInitialPosition;
            string m_sNextChar = "";

            if (m_iNextPosition < sInText.Length)
                m_sNextChar = sInText.Substring(m_iNextPosition, 1);

            while (m_iNextPosition < sInText.Length
            && CARACTER_CIERRE_ETIQUETAS.IndexOf(m_sNextChar.ToLower(), 0) < 0)
            {
                m_sNextTag = m_sNextTag + m_sNextChar;
                m_iNextPosition += 1;

                if (m_iNextPosition < sInText.Length)
                    m_sNextChar = sInText.Substring(m_iNextPosition, 1);
            } // while(m_iNextPosition < sInText.Length

            if (m_iNextPosition < sInText.Length
            && CARACTER_CIERRE_ETIQUETAS.IndexOf(m_sNextChar.ToLower(), 0) >= 0)
            {
                m_sNextTag = m_sNextTag + m_sNextChar;
                m_iNextPosition += 1;
            } // if (m_iNextPosition < sInText.Length

            iInitialPosition = m_iNextPosition;
            return m_sNextTag;
        } // private string GetNextTag(string sInText, ref int iInitialPosition)
    } // class CResaltarTextoHTML
}
