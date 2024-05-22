#define _DEFAULT_SOURCE
#define _GNU_SOURCE

#include <stdio.h>    
#include <stdlib.h>
#include <unistd.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <string.h>
#include "sqlite3.h"
#include <dirent.h>

#define INVALID_POSITION -1

int load_text_file(char * sSourceFile, char ** Contents, int * iContentsLength)
{
	int m_iResult = 1;
	*iContentsLength = 0;
	
	int m_iSourceFd = open(sSourceFile, O_RDONLY);
        
    if (m_iSourceFd == -1)
    {
        perror(sSourceFile);
		m_iResult = 0;
    } // if (m_iSourceFd == -1)

    else
    {
		struct stat m_SourceStat;
		
        if (fstat(m_iSourceFd, &m_SourceStat) == -1)
        {
            perror("fstat");
		    m_iResult = 0;
        } // if (fstat(m_iSourceFd, &m_SourceStat) == -1)

        else if (!S_ISREG(m_SourceStat.st_mode))
        {
                    fprintf(stderr, "%s no es un archivo regular\n", sSourceFile);		    
		    m_iResult = 0;
        } // else if (!S_ISREG(m_SourceStat.st_mode))
    
        else
        {		
	        *iContentsLength = m_SourceStat.st_size;
	        *Contents = (char *) malloc(*iContentsLength+1);
	        memset(*Contents, '\0', *iContentsLength+1);
	    
	        read(m_iSourceFd, *Contents, *iContentsLength);
        } // else
        
        close(m_iSourceFd);
    } // else
    
    return m_iResult;
} // int load_text_file(char * sSourceFile, char ** Contents, int * iContentsLength)


int create_text_file(char * sTargetFile, char * sContents)
{
	int m_iResult = 1;
	
    FILE * m_Target = fopen(sTargetFile, "w");

    if (m_Target == NULL)
    {
        fprintf(stderr, "%s no pudo ser creado\n", sTargetFile);
        m_iResult = 0;
    } // if (m_Target == NULL)
    
    else
    {
		char m_Character = 0;
		int m_iPosition = 0;
        while((m_Character = sContents[m_iPosition]) != '\0')
        {
            fputc(m_Character, m_Target);
            m_iPosition ++;
		} // while((m_Character = sContents[m_iPosition]) != '\0')
		
        fclose(m_Target);
    } // else
    
    return m_iResult;
} // void create_text_file(char * sTargetFile, char * sContents)
	
int indexOf(char * sTheString, char * sSearched, int iFromIndex)
{
  int m_iPosition = INVALID_POSITION;  
  char * m_sTheStringPointer = &sTheString[iFromIndex];
  char * m_sResult = strstr(m_sTheStringPointer, sSearched);
    
  if (m_sResult)
    m_iPosition = m_sResult - sTheString;
 
  return m_iPosition;
} // int indexOf(char * TheString, char * Searched, int FromIndex)

int lastIndexOf(char * sTheString, char * sSearched, int iUntilIndex, int iMaxLengthToCompare)
{
  int m_iPosition = INVALID_POSITION;
  int m_iStartFromPosition = iUntilIndex-iMaxLengthToCompare+1;
  
  if (m_iStartFromPosition >= 0)
  {
      char * m_sTheStringPointer = &sTheString[m_iStartFromPosition];

      while (m_sTheStringPointer >= sTheString && m_iPosition == INVALID_POSITION)
      {
          int m_iResult = strncmp(m_sTheStringPointer, sSearched, iMaxLengthToCompare);
      
          if (m_iResult == 0)
              m_iPosition = m_sTheStringPointer - sTheString;
    
          else
              m_sTheStringPointer -= 1;
      } // while (m_sTheStringPointer >= sTheString && m_iPosition == INVALID_POSITION)
  } // if (m_iStartFromPosition >= 0)
  
  return m_iPosition;
} // int lastIndexOf(char * sTheString, char * sSearched, int iUntilIndex, int iMaxLengthToCompare)

char * extract_header(char * sFromContents, int * iOK)
{
	*iOK = 0;
	char * m_sHeader = NULL;
	char m_sTagBody[10] = "<body ";
	int m_iPosition = indexOf(sFromContents, m_sTagBody, 0);
	
	if (m_iPosition >= 0)
	{
		char m_sUntilTag[50] = ">";
		m_iPosition = indexOf(sFromContents, m_sUntilTag, m_iPosition + 1);
		
		if (m_iPosition >= 0)
		{
			int m_iPositionTo = m_iPosition + strlen(m_sUntilTag);
			m_sHeader = (char *) malloc(m_iPositionTo + 1);
			strncpy(m_sHeader, sFromContents, m_iPositionTo);
			m_sHeader[m_iPositionTo] = '\0';
			*iOK = 1;
        } // if (m_iPosition >= 0)
    } // if (m_iPosition >= 0)
    else
        fprintf(stderr, "No se encontró la etiqueta '%s' en el archivo por procesar\n", m_sTagBody);
    
    return m_sHeader;
} // char * extract_header(char * sFromContents, int * iOK)

char * my_strcat(char * sIntoString, int iIntoStringLength, char * sFromString, int iFromStringLength)
{
	int m_iCurrentPosition = iIntoStringLength;

	for(int i = 0; i < iFromStringLength; i++)
	{
		sIntoString[m_iCurrentPosition] = sFromString[i];
		m_iCurrentPosition++;
    } // for(int i = 0; i < iFromStringLength; i++)
    
    return sIntoString;
} // char * my_strcat(char * sIntoString, int iIntoStringLength, char * sFromString, int iFromStringLength)

char * str_replace(char * sIntoString, int iFromPosition, int iToPosition, char * sReplacement, int iIntoStringLength, int iReplacementLength)
{
	int m_iResultLength = iIntoStringLength + iReplacementLength - (iToPosition - iFromPosition + 1);
	m_iResultLength += 1; // Se agrega espacio para el caracter de cierre de hilera
	char * m_sResult = (char *) malloc (m_iResultLength);
	
    m_sResult = my_strcat(m_sResult, 0, sIntoString, iFromPosition);
    m_sResult = my_strcat(m_sResult, iFromPosition, sReplacement, iReplacementLength);
	m_sResult = my_strcat(m_sResult, iReplacementLength+iFromPosition, &sIntoString[iToPosition + 1], iIntoStringLength-iToPosition-1);
    m_sResult[m_iResultLength-1] = '\0';
    
	return m_sResult;
} // char * str_replace(char * sIntoString, int iFromPosition, int iToPosition, char * sReplacement, int iIntoStringLength, int iReplacementLength)

char ETIQUETA_SUMARIO[50] = "<div id=\"Sumario1\"";

char * extract_contents_index(char * sFromContents, int * iOK, int * iPositionFrom, int * iPositionTo)
{
	*iOK = 0;
	char * m_sContentsIndex = NULL;
	int m_iPosition = indexOf(sFromContents, ETIQUETA_SUMARIO, 0);

	if (m_iPosition >= 0)
	{
		char m_sFromTag[50] = "<p ";
	    m_iPosition = lastIndexOf(sFromContents, m_sFromTag, m_iPosition, strlen(m_sFromTag));
	    
	    if (m_iPosition >= 0)
	    {
			*iPositionFrom = m_iPosition;
			
			char m_sUntilTag[50] = "</div>";
		    m_iPosition = indexOf(sFromContents, m_sUntilTag, m_iPosition + 1);
		
		    if (m_iPosition >= 0)
		    {				
				*iPositionTo = m_iPosition + strlen(m_sUntilTag)-1;
				int m_iContentsIndexLength = *iPositionTo - *iPositionFrom + 1;
				m_sContentsIndex = (char *) malloc(m_iContentsIndexLength + 1);
  			    
			    strncpy(m_sContentsIndex, &sFromContents[*iPositionFrom], m_iContentsIndexLength);
			    m_sContentsIndex[m_iContentsIndexLength] = '\0';
			    *iOK = 1;
            } // if (m_iPosition >= 0)            
        } // if (m_iPosition >= 0)
    } // if (m_iPosition >= 0)
    
    else
        fprintf(stderr, "No se encontró la etiqueta %s en el índice de contenidos\n", ETIQUETA_SUMARIO);
        
    return m_sContentsIndex;
} // char * extract_contents_index(char * sFromContents, int * iOK, int * iPositionFrom, int * iPositionTo)

char END_OF_FILE_TAGS[50] = "</body></html>";

char * get_contents_of_file(char * sHeader, char * sContents)
{	
	int m_iHeaderLength = strlen(sHeader);
	int m_iContentsIndexLength = strlen(sContents);
	int m_iEndOfFileLength = strlen(END_OF_FILE_TAGS);
	int m_iContentsIndexFileLenght = m_iHeaderLength+m_iContentsIndexLength + m_iEndOfFileLength;
	char * m_sContentsFile = (char *) malloc(m_iContentsIndexFileLenght + 1);
				
	m_sContentsFile = my_strcat(m_sContentsFile, 0, sHeader, m_iHeaderLength);
	m_sContentsFile = my_strcat(m_sContentsFile, m_iHeaderLength, sContents, m_iContentsIndexLength);
	m_sContentsFile = my_strcat(m_sContentsFile, m_iHeaderLength+m_iContentsIndexLength, END_OF_FILE_TAGS, m_iEndOfFileLength);
	m_sContentsFile[m_iContentsIndexFileLenght] = '\0';
	return m_sContentsFile;
} // char * get_contents_of_file(char * sHeader, char * sContents)

char DIGITOS_DECIMALES[11] = "0123456789";

char * GetNextNumber(char * sInContents, int * iIsValid)
{
	*iIsValid = 1;
	int m_iInContensLength = strlen(sInContents);
	char * m_sResult = (char *) malloc(m_iInContensLength+1);
	strcpy(m_sResult, "");
	
	const int ESPERA_DIGITO = 1;
	const int ESPERA_PUNTO = 2;
	
	int m_iEspera = ESPERA_DIGITO;
	
	int m_iNumberOfDigits = 0;
	int m_iNumberOfPoints = 0;
	int m_iNextCharacterIsDigit = 0;
	int m_iNextCharacterIsPoint = 0;
	int m_iNumberOfBlocksOfSpacesAfterFirstDigit = 0;
	
	int m_iSalir = 0;
	
	for(int i = 0; i < m_iInContensLength && !m_iSalir; i++)
	{
		char m_sNextCharacter[2] = "";
		m_sNextCharacter[1] = '\0';
		strncpy(m_sNextCharacter, &sInContents[i], 1);
		
		if (strcmp(m_sNextCharacter, " ") == 0)
		{
			if (m_iNumberOfDigits > 0)
			    m_iNumberOfBlocksOfSpacesAfterFirstDigit += 1;
			
			while (strcmp(m_sNextCharacter, " ") == 0)
			{
				i += 1;
				strncpy(m_sNextCharacter, &sInContents[i], 1);
			} // while (strcmp(m_sNextCharacter, " ") == 0)
		} // if (strcmp(m_sNextCharacter, " ") == 0)
		
		if (i >= m_iInContensLength)
		    continue;

		m_iNextCharacterIsDigit = 0;
		m_iNextCharacterIsPoint = 0;
		
		if (indexOf(DIGITOS_DECIMALES, m_sNextCharacter, 0) >= 0)
		{
			m_iNumberOfDigits += 1;
		    m_iNextCharacterIsDigit = 1;
		} // if (indexOf(DIGITOS_DECIMALES, m_sNextCharacter, 0) >= 0)
		   
		if (strcmp(m_sNextCharacter, ".") == 0)
		{
			m_iNumberOfPoints += 1;
		    m_iNextCharacterIsPoint = 1;
		} // if (strcmp(m_sNextCharacter, ".") == 0)
				
		if ((m_iEspera & ESPERA_DIGITO) > 0 && m_iNextCharacterIsDigit)
		    m_iEspera = ESPERA_DIGITO + ESPERA_PUNTO;
		
		else if ((m_iEspera & ESPERA_PUNTO) > 0 && m_iNextCharacterIsPoint)
		    m_iEspera = ESPERA_DIGITO;
		   		
		else
		    m_iSalir = 1;
		    				
		if (m_iNumberOfBlocksOfSpacesAfterFirstDigit >= 1 && (m_iNextCharacterIsDigit || m_iNextCharacterIsPoint))
		{
		    *iIsValid = 0;
		    m_iSalir = 1;
		} // if (m_iNumberOfBlocksOfSpacesAfterFirstDigit >= 1 && (m_iNextCharacterIsDigit || m_iNextCharacterIsPoint))
		
		if (*iIsValid && (m_iNextCharacterIsDigit || m_iNextCharacterIsPoint))
		    strcat(m_sResult, m_sNextCharacter);
	} // for(int i = 0; i < m_iInContensLength; i++)

	if (m_iNextCharacterIsPoint || m_iNumberOfPoints > 1 || m_iNumberOfDigits == 0)
        *iIsValid = 0;
        	
	return m_sResult;
} // char * GetNextNumber(char * sInContents, int * iIsValid)

struct node
{
    char * sLlave;
    int iNodo;
    int iPadre;
    int iOrden;
    char * sTitulo;
    char * sFileContents;
    struct node * left_child;
    struct node * right_child;
}; // struct node

struct node * change_file_contents(struct node * InNode, char * sFileContents)
{
	if (InNode->sFileContents != NULL)
	    free(InNode->sFileContents);
	    
    if (sFileContents != NULL)
    {
        InNode->sFileContents = (char*) malloc(strlen(sFileContents) + 1);
        strcpy(InNode->sFileContents, sFileContents);
    } // if (sFileContents != NULL)
    else
        InNode->sFileContents = NULL;
    
    return InNode;
} // struct node * change_file_contents(struct node * InNode, char * sFileContents)

struct node * new_node(char * sLlave, int iNodo, int iPadre, int iOrden, char * sTitulo, char * sFileContents)
{
    struct node * Pointer = malloc(sizeof(struct node));
    
    Pointer->sLlave = NULL;
    
    if (sLlave != NULL)
    {
        Pointer->sLlave = (char*) malloc(strlen(sLlave) + 1);
        strcpy(Pointer->sLlave, sLlave);
    } // if (sLlave != NULL)
    
    Pointer->iNodo = iNodo;
    Pointer->iPadre = iPadre; 
    Pointer->iOrden = iOrden;
    
    Pointer->sTitulo = NULL;
    
    if (sTitulo != NULL)
    {
        Pointer->sTitulo = (char*) malloc(strlen(sTitulo) + 1);
        strcpy(Pointer->sTitulo, sTitulo);
    } // if (sTitulo != NULL)
    
    Pointer->sFileContents = NULL;
    Pointer = change_file_contents(Pointer, sFileContents);
        
    Pointer->left_child = NULL;
    Pointer->right_child = NULL;

    return Pointer;
} // struct node * new_node(char * sLlave, int iNodo, int iPadre, int iOrden, char * sTitulo, char * sFileContents)

struct node * delete_contents(struct node * root)
{
	if (root->sLlave != NULL)
	    free(root->sLlave);
	    
	root->sLlave = NULL;
	
	if (root->sTitulo != NULL)
	    free(root->sTitulo);
	    
	root->sTitulo = NULL;

    if (root->sFileContents != NULL)
	    free(root->sFileContents);
	    
	root->sFileContents = NULL;
	
	return root;
} // struct node * delete_contents(struct node * root)

struct node * search(struct node * root, char * sLlave)
{
	int m_iComparisonResult = 0;
	
	if(root != NULL)
	  m_iComparisonResult = strcmp(sLlave, root->sLlave);
		  
    if (root == NULL)
        return root;
    else if (m_iComparisonResult < 0)
		return search(root->left_child, sLlave);
    else if (m_iComparisonResult > 0)
        return search(root->right_child, sLlave);
    else
        return root;
} // struct node * search(struct node * root, char * sLlave)

struct node * insert(struct node * root, struct node * ToAdd)
{	
    if(root != NULL)
    {
		int m_iComparisonResult = strcmp(ToAdd->sLlave, root->sLlave);
		
        if(m_iComparisonResult < 0)
            root->left_child = insert(root->left_child, ToAdd);		
            
        else if(m_iComparisonResult > 0)
		    root->right_child = insert(root->right_child, ToAdd);
    } // if(root != NULL)
    else
        root = ToAdd;
        
    return root;
} // struct node * insert(struct node * root, struct node * ToAdd)

struct node * delete_tree(struct node * root)
{
    if(root != NULL)
    {
		root->left_child = delete_tree(root->left_child);
        root->right_child = delete_tree(root->right_child);
        root = delete_contents(root);
        free(root);
    } // if(root != NULL)
    
    root = NULL;
    return root;
 } // struct node * delete_tree(struct node * root)


void inorder(struct node * root)
{
    if(root != NULL)
    {
        inorder(root->left_child);
        printf("Llave:%s,Título:%s", root->sLlave, root->sTitulo);
        
        if (root->sFileContents != NULL)
            printf(",File Contents:%s", root->sFileContents);
        
        printf(",Id Nodo:%i,Id Padre:%i,Orden:%i", root->iNodo, root->iPadre, root->iOrden);
        printf("\n");
        inorder(root->right_child);
    } // if(root != NULL)
} // void inorder(struct node *root)

struct node * TryToAddNode(struct node * ToRoot, char * sLlave, int iNodo, int iPadre, int iOrden, char * sTitulo, char * sFileContents)
{
	int m_iAdd = 0;
	
	if (ToRoot == NULL)
	    m_iAdd = 1;
	else if (search(ToRoot, sLlave) == NULL)
		m_iAdd = 1;

	if (m_iAdd)
	{
	    struct node * m_NewNode = new_node(sLlave, iNodo, iPadre, iOrden, sTitulo, sFileContents);	    
	    ToRoot = insert(ToRoot, m_NewNode);
	} // if (m_iAdd)
	
	return ToRoot;
} // struct node * TryToAddNode(struct node * ToRoot, char * sLlave, int iNodo, int iPadre, int iOrden, char * sTitulo, char * sFileContents)

const int MAX_NUM_CARACTERES_EN_TITULO = 500;

struct node * build_tree_structure(struct node * Root, char * sContentsIndex, int iIdPadre, int * iSiguienteNodo, double dCurrentLeftMargin, int * iStartFromPosition)
{
	char ENTRY_NODE_REF[50] = "#__RefHeading__";
	char MARGIN_LEFT_TAG[50] = "margin-left:";
	
	int m_iOrden = 1;
	int m_iPosition = indexOf(sContentsIndex, ENTRY_NODE_REF, *iStartFromPosition);
	int m_iEntryNodeTagPosition = *iStartFromPosition;
	int m_iPreviousEntryNodeTagPosition = m_iEntryNodeTagPosition;
    int m_iPadreSiguienteNivel = *iSiguienteNodo;
			
	while (m_iPosition >= 0)
	{
		m_iEntryNodeTagPosition = m_iPosition;
		int m_iFromPosition = m_iPosition+1;
		m_iPosition = indexOf(sContentsIndex, ">", m_iPosition+1);
		
		if (m_iPosition >= 0)
		{
			int m_iToPosition = m_iPosition - 2;
			int m_iNumCaracteresEnLlave = m_iToPosition - m_iFromPosition + 1;
			char * m_sLlave = malloc(m_iNumCaracteresEnLlave + 1);
			strncpy(m_sLlave, &sContentsIndex[m_iFromPosition], m_iNumCaracteresEnLlave);
			m_sLlave[m_iNumCaracteresEnLlave] = '\0';
		
			int m_iInitialTitlePosition = m_iPosition + 1;
			int m_iLastTitlePosition = indexOf(&sContentsIndex[m_iInitialTitlePosition], "</a>", 0);
			char m_sTitulo[MAX_NUM_CARACTERES_EN_TITULO+1];
			strcpy(m_sTitulo, "");
			
			int m_iAddNode = 1;
			
			if (m_iLastTitlePosition >= 0)
			{
				m_iLastTitlePosition += m_iInitialTitlePosition - 1;
			    int m_iNumCaracteresEnTitulo = m_iLastTitlePosition - m_iInitialTitlePosition + 1;
			    
			    if (m_iNumCaracteresEnTitulo > MAX_NUM_CARACTERES_EN_TITULO)
			        m_iNumCaracteresEnTitulo = MAX_NUM_CARACTERES_EN_TITULO;

                if (m_iNumCaracteresEnTitulo > 0)
                {
			        strncpy(m_sTitulo, &sContentsIndex[m_iInitialTitlePosition], m_iNumCaracteresEnTitulo);
			        m_sTitulo[m_iNumCaracteresEnTitulo] = '\0';
			    } // if (m_iNumCaracteresEnTitulo > 0)
			} // if (m_iLastTitlePosition >= 0)
						
			m_iPosition = lastIndexOf(sContentsIndex, MARGIN_LEFT_TAG, m_iFromPosition - 1, strlen(MARGIN_LEFT_TAG));
			double m_dNextMarginLeft = 0;
			
			if (m_iPosition > m_iPreviousEntryNodeTagPosition)
			{
				int m_iPosicionEtiquetaMarginLeft = m_iPosition;
				int m_iIsValid = 0;
				int MAX_LENGTH_NUMBER = 50;
				char m_sSearchLeftMargin[MAX_LENGTH_NUMBER+1];
				strncpy(m_sSearchLeftMargin, &sContentsIndex[m_iPosition + strlen(MARGIN_LEFT_TAG)], MAX_LENGTH_NUMBER);
				m_sSearchLeftMargin[MAX_LENGTH_NUMBER] = '\0';
				
			    char * m_sMarginLeft = GetNextNumber(m_sSearchLeftMargin, &m_iIsValid);
			    
			    if (m_iIsValid)
			    {
			        m_dNextMarginLeft = atof(m_sMarginLeft);
			        
			        if (m_dNextMarginLeft > dCurrentLeftMargin)
			        {
						int m_iSiguienteHijo = *iSiguienteNodo;
						int m_iPosicionEtiquetaSumario = lastIndexOf(sContentsIndex, ETIQUETA_SUMARIO, m_iFromPosition - 1, strlen(ETIQUETA_SUMARIO));
						
						if (m_iPreviousEntryNodeTagPosition <= 0 && m_iPosicionEtiquetaSumario < m_iPosicionEtiquetaMarginLeft)
						{
						    fprintf(stderr, "La etiqueta '%s' no puede contener un valor mayor que 0 en la primera entrada del índice de contenidos\n", MARGIN_LEFT_TAG);
						    return Root;
						} // if (m_iPreviousEntryNodeTagPosition <= 0 && m_iPosicionEtiquetaSumario < m_iPosicionEtiquetaMarginLeft)
						
						*iStartFromPosition = m_iPreviousEntryNodeTagPosition + 1;
						m_iAddNode = 0;

						Root = build_tree_structure(Root, sContentsIndex, m_iPadreSiguienteNivel, &m_iSiguienteHijo, m_dNextMarginLeft, iStartFromPosition);
			            *iSiguienteNodo = m_iSiguienteHijo;
			            m_iEntryNodeTagPosition = *iStartFromPosition;
			        } // if (m_dNextMarginLeft > dCurrentLeftMargin)
			    } // if (m_iIsValid)
			} // if (m_iPosition > m_iPreviousEntryNodeTagPosition)
						
			if (m_dNextMarginLeft < dCurrentLeftMargin)
                return Root;
            
            if (m_iAddNode)
            {
				int m_iAttached = 0;
			    Root = TryToAddNode(Root, m_sLlave, *iSiguienteNodo, iIdPadre, m_iOrden, m_sTitulo, NULL);
			    struct node * m_NewNode = search(Root, m_sLlave);
			    
			    if (m_NewNode != NULL)
			    {
					if (m_NewNode->iNodo == *iSiguienteNodo)
					    m_iAttached = 1;
				} // if (m_NewNode != NULL)
			    
			    // printf("Agregado:\%i, Título:%s, Id Padre:%i, Id Nodo:%i, Orden:%i, Llave:%s\n", m_iAttached, m_sTitulo, iIdPadre, *iSiguienteNodo, m_iOrden, m_sLlave);

                if (m_iAttached)
                {
					m_iPadreSiguienteNivel = *iSiguienteNodo;
        		    *iSiguienteNodo += 1;
		            m_iOrden += 1;
		        } // if (m_iAttached)
			} // if (m_iAddNode)
		} // if (m_iPosition >= 0)
		
		*iStartFromPosition = m_iEntryNodeTagPosition + 1;
		m_iPreviousEntryNodeTagPosition = m_iEntryNodeTagPosition;
		m_iPosition = indexOf(sContentsIndex, ENTRY_NODE_REF, *iStartFromPosition);
	} // while (m_iPosition >= 0)
	
	return Root;
} // struct node * build_tree_structure(struct node * Root, char * sContentsIndex, int iIdPadre, int * iSiguienteNodo, double dCurrentLeftMargin, int * iStartFromPosition)

char * str_replace_all(char * sIntoString, char * sToReplace, char * sReplacement, int iIntoStringLength, int iToReplaceLength, int iReplacementLength, int * iNumberOfReplacements)
{
	*iNumberOfReplacements = 0;
	char * m_sPreviousResult = NULL;
	char * m_sCurrentResult = sIntoString;
	
	int m_iPosition = indexOf(m_sCurrentResult, sToReplace, 0);
	int m_iCurrentResultLength = iIntoStringLength;
	
	while (m_iPosition >= 0)
	{
		m_sCurrentResult = str_replace(m_sCurrentResult, m_iPosition, m_iPosition + iToReplaceLength - 1, sReplacement, m_iCurrentResultLength, iReplacementLength);
		m_iCurrentResultLength = m_iCurrentResultLength - iToReplaceLength + iReplacementLength;
		
		if (m_sPreviousResult != NULL)
		    free(m_sPreviousResult);
		    
		m_sPreviousResult = m_sCurrentResult;
		m_iPosition += iReplacementLength;
		m_iPosition = indexOf(m_sCurrentResult, sToReplace, m_iPosition);
		*iNumberOfReplacements += 1;
	} // while (m_iPosition >= 0)
	
	return m_sCurrentResult;
} // char * str_replace_all(char * sIntoString, char * sToReplace, char * sReplacement, int iIntoStringLength, int iToReplaceLength, int iReplacementLength, int * iNumberOfReplacements)

char * scm(char * sDato)
{
	int m_iNumberOfReplacements = 0;
	char * m_sCopiaDato = str_replace_all(sDato, "'", "\'\'", strlen(sDato), 1, 2, &m_iNumberOfReplacements);
	
	char * m_sResultado = (char *) malloc(strlen(m_sCopiaDato)+3);
	// Se agrega espacio para el caracter de apertura y cierre del string (') más el caracter '\0' de cierre de hilera.
	
	strcpy(m_sResultado, "'");
	strcat(m_sResultado, m_sCopiaDato);	
	strcat(m_sResultado, "'");
	
	if (m_iNumberOfReplacements > 0)
	    free(m_sCopiaDato);
	    
	m_sCopiaDato = NULL;
	
	return m_sResultado;
} // char * scm(char * sDato)

char my_toascci(int iDigit)
{	
	if (iDigit >= 0 && iDigit <= 9)
	    return DIGITOS_DECIMALES[iDigit];
	else
	    return '\0';
} // char my_toascci(int iDigit)

char * my_str_rev(char * sStringToReverse)
{
	int m_iLength = strlen(sStringToReverse);
	char * m_sReverse = (char *) malloc(m_iLength+1);
	
	for(int i=0; i < m_iLength; i++)
	    m_sReverse = my_strcat(m_sReverse, i, &sStringToReverse[m_iLength-i-1], 1);
	    
	m_sReverse[m_iLength] = '\0';
	return m_sReverse;
} // char * my_str_rev(char * sNumber)

const int MAX_NUMBERS_LENGTH = 50;

char * my_itoa(int iNumber)
{
	const int BASE = 10;
	char * m_sNumber = (char *) malloc(MAX_NUMBERS_LENGTH+1);
	int m_iNumberLength = 0;
	int m_iEsMenorQue0 = 0;
	
	if (iNumber < 0)
	{
		iNumber *= -1;
		m_iEsMenorQue0 = 1;
	} // if (iNumber < 0)
	
	do
	{
		int m_iCociente = iNumber / BASE;
		int m_iResiduo = iNumber % BASE;
		char m_cDigito = my_toascci(m_iResiduo);
		m_sNumber = my_strcat(m_sNumber, m_iNumberLength, &m_cDigito, 1);
		m_iNumberLength+= 1;
		iNumber = m_iCociente;
	} while(iNumber > 0);
	
	if (m_iEsMenorQue0)
	{
		char m_cSignoMenos = '-';
		m_sNumber = my_strcat(m_sNumber, m_iNumberLength, &m_cSignoMenos, 1);
		m_iNumberLength+= 1;		
	} // if (m_iEsMenorQue0)

    m_sNumber[m_iNumberLength] = '\0';
	m_sNumber = my_str_rev(m_sNumber);
	return m_sNumber;
} // char * my_itoa(int iNumber)

static int callback(void *NotUsed, int argc, char **argv, char **azColName)
{
  int i;
  
  for(i=0; i<argc; i++){
    printf("%s = %s\n", azColName[i], argv[i] ? argv[i] : "NULL");
  }
  
  printf("\n");
  return 0;
}

int exec_sql_command(int argc, char **argv)
{
  int m_iResult = 1;
  sqlite3 *db;
  char *zErrMsg = 0;
  int rc;
    
  if( argc!=3 )
  {
    fprintf(stderr, "Usage: %s DATABASE SQL-STATEMENT\n", argv[0]);
    m_iResult = 0;
  }
  else
  {
    rc = sqlite3_open(argv[1], &db);

    if( rc )
    {
      fprintf(stderr, "Can't open database: %s\n", sqlite3_errmsg(db));
      m_iResult = 0;
    }
    else
    {
      rc = sqlite3_exec(db, argv[2], callback, 0, &zErrMsg);
  
      if( rc!=SQLITE_OK )
      {
		fprintf(stderr, "Error al ejecutar la instrucción SQL: %s\n", argv[2]);
        fprintf(stderr, "SQL error: %s\n", zErrMsg);
        sqlite3_free(zErrMsg);
        m_iResult = 0;
      }
    }
    
    sqlite3_close(db);
  }
  
  return m_iResult;
} // int exec_sql_command(int argc, char **argv)

char SQLITE3_COMMAND_FILE_NAME[50] = "./sqlite3_shell";

void insert_node_into_database(char * sDatabaseFileName, int iIdNodo, int iIdPadre, char * sTitulo, int iOrden)
{	
	char * m_sIdNodo = my_itoa(iIdNodo);
	char * m_sIdPadre = my_itoa(iIdPadre);
	char * m_sTitulo = scm(sTitulo);
	char * m_sOrden = my_itoa(iOrden);	
	
	char * m_sQuery = (char *) malloc(strlen(m_sIdNodo) + strlen(m_sIdPadre) + strlen(m_sTitulo) + strlen(m_sOrden) + 100);
	
	strcpy(m_sQuery, "insert into Arbol(IdNodo, IdPadre, Titulo, Orden) values(");
	
	strcat(m_sQuery, m_sIdNodo);  free(m_sIdNodo); m_sIdNodo = NULL;
	
	strcat(m_sQuery, ", ");
	strcat(m_sQuery, m_sIdPadre);  free(m_sIdPadre); m_sIdPadre = NULL;
	
	strcat(m_sQuery, ", ");
	strcat(m_sQuery, m_sTitulo); free(m_sTitulo); m_sTitulo = NULL;

	strcat(m_sQuery, ", ");
	strcat(m_sQuery, m_sOrden);  free(m_sOrden); m_sOrden = NULL;
	
	strcat(m_sQuery, ")");
	
    char *argv[] = {SQLITE3_COMMAND_FILE_NAME, sDatabaseFileName, m_sQuery};
	exec_sql_command(3, argv);
	
	free(m_sQuery);
	m_sQuery = NULL;
} // void insert_node_into_database(char * sDatabaseFileName, int iIdNodo, int iIdPadre, char * sTitulo, int iOrden)

void delete_all_nodes_from_database(char * sDatabaseFileName)
{
	char * m_sQuery = (char *) malloc(100);
	strcpy(m_sQuery, "delete from Arbol");
	
    char *argv[] = {SQLITE3_COMMAND_FILE_NAME, sDatabaseFileName, m_sQuery};
	exec_sql_command(3, argv);

	free(m_sQuery);
	m_sQuery = NULL;
} // void delete_all_nodes_from_database(char * sDatabaseFileName)

void insert_tree_into_database(struct node * root, char * sDatabaseFileName)
{
    if(root != NULL)
    {
        insert_tree_into_database(root->left_child, sDatabaseFileName);
        insert_node_into_database(sDatabaseFileName, root->iNodo, root->iPadre, root->sTitulo, root->iOrden);
        insert_tree_into_database(root->right_child, sDatabaseFileName);
    } // if(root != NULL)
} // void insert_tree_into_database(struct node * root, char * sDatabaseFileName)

void create_database(char * sDatabaseFileName, struct node * RootStructure, char * sRootTitle, int iIdNodoRaiz, int iIdPadreRaiz)
{
    delete_all_nodes_from_database(sDatabaseFileName);
    insert_node_into_database(sDatabaseFileName, iIdNodoRaiz, iIdPadreRaiz, sRootTitle, 1);
    insert_tree_into_database(RootStructure, sDatabaseFileName);
} // void create_database(char * sDatabaseFileName, struct node * RootStructure, char * sRootTitle, int iIdNodoRaiz, int iIdPadreRaiz)

void change_file_contents_into_node(char * sFileContents, struct node * StructureToChange, int iCopiarDesde, int iCopiarHasta)
{
    int m_iTamanoContenidoArchivo = iCopiarHasta - iCopiarDesde + 1;
    m_iTamanoContenidoArchivo += 1; // Se agrega espacio para el caracter de cierre de la hilera
				
    char * m_sContenidoArchivo = (char *) malloc(m_iTamanoContenidoArchivo);
    strncpy(m_sContenidoArchivo, &sFileContents[iCopiarDesde], m_iTamanoContenidoArchivo-1);
    m_sContenidoArchivo[m_iTamanoContenidoArchivo-1] = '\0';
    change_file_contents(StructureToChange, m_sContenidoArchivo);
				    				    
    free(m_sContenidoArchivo);
    m_sContenidoArchivo = NULL;
} // void change_file_contents_into_node(char * sFileContents, struct node * StructureToChange, int iCopiarDesde, int iCopiarHasta)

void change_file_contents_into_root(char * sFileContents, struct node * RootStructure, int iFileContentsLength)
{
	char ENTRY_NODE_REF[50] = "__RefHeading__";
	char PAGE_TAG[50] = "<p ";
	char HEADING_TAG[50] = "<h";

	int m_iPosicionInicialLlave = indexOf(sFileContents, ENTRY_NODE_REF, 0);
	
	while(m_iPosicionInicialLlave >= 0)
	{
		int m_iPosicionFinalLlave = indexOf(sFileContents, "\"", m_iPosicionInicialLlave+1);
		m_iPosicionFinalLlave -= 1; // Se excluye el caracter " final.
		
		if (m_iPosicionFinalLlave >= 0)
		{
			int m_iTamanoLlave = m_iPosicionFinalLlave - m_iPosicionInicialLlave + 1;
			m_iTamanoLlave += 1; // Se agrega espacio para el caracter '\0' de cierre de la hilera
			char * m_sLlave = (char *) malloc(m_iTamanoLlave);
			strncpy(m_sLlave, &sFileContents[m_iPosicionInicialLlave], m_iTamanoLlave-1);
			m_sLlave[m_iTamanoLlave-1] = '\0';
			
			struct node * m_StructureToChange = search(RootStructure, m_sLlave);
		        		
			if (m_StructureToChange != NULL)
			{
				int m_iPosicionInicioContenidoArchivo = lastIndexOf(sFileContents, PAGE_TAG, m_iPosicionInicialLlave-1, strlen(PAGE_TAG));
				int m_iPosicionEtiquetaEncabezado = lastIndexOf(sFileContents, HEADING_TAG, m_iPosicionInicialLlave-1, strlen(HEADING_TAG));

				if (m_iPosicionEtiquetaEncabezado > m_iPosicionInicioContenidoArchivo)
					m_iPosicionInicioContenidoArchivo = m_iPosicionEtiquetaEncabezado;
				
			    if (m_iPosicionInicioContenidoArchivo >= 0)
			    {
					int m_iPosicionInicioSiguienteContenidoArchivo = -1;
					int m_iPosicionInicialSiguienteLlave = indexOf(sFileContents, ENTRY_NODE_REF, m_iPosicionFinalLlave+1);
						
					if (m_iPosicionInicialSiguienteLlave >= 0)
					{
                        m_iPosicionInicioSiguienteContenidoArchivo = lastIndexOf(sFileContents, PAGE_TAG, m_iPosicionInicialSiguienteLlave-1, strlen(PAGE_TAG));
						m_iPosicionEtiquetaEncabezado = lastIndexOf(sFileContents, HEADING_TAG, m_iPosicionInicialSiguienteLlave-1, strlen(HEADING_TAG));

						if (m_iPosicionEtiquetaEncabezado > m_iPosicionInicioSiguienteContenidoArchivo)
							m_iPosicionInicioSiguienteContenidoArchivo = m_iPosicionEtiquetaEncabezado;
					} // if (m_iPosicionInicialSiguienteLlave >= 0)

					else
						m_iPosicionInicioSiguienteContenidoArchivo = iFileContentsLength; // Se llegó al final del archivo.
						
					if (m_iPosicionInicioSiguienteContenidoArchivo >= 0)
					{
					    int m_iCopiarDesde = m_iPosicionInicioContenidoArchivo;
				        int m_iCopiarHasta = m_iPosicionInicioSiguienteContenidoArchivo - 1; 
				        // Se excluye la apertura de la etiqueta <p o el caracter '\0' final del archivo.
				    
				        if (m_iCopiarHasta >= m_iCopiarDesde)
					        change_file_contents_into_node(sFileContents, m_StructureToChange, m_iCopiarDesde, m_iCopiarHasta);					    
					} // if (m_iPosicionInicioContenidoArchivoAnterior >= 0)					
			    } // if (m_iPosicionInicioContenidoArchivo >= 0 && m_iPosicionInicioContenidoArchivoAnterior >= 0)
			} // if (m_StructureToChange != NULL)
			
			free(m_sLlave);
			m_sLlave = NULL;
		} // if (m_iPosicionFinalLlave >= 0)
		
		m_iPosicionInicialLlave = indexOf(sFileContents, ENTRY_NODE_REF, m_iPosicionFinalLlave+1);
	} // while(m_iPosicionInicialLlave >= 0)
} // void change_file_contents_into_root(char * sFileContents, struct node * RootStructure)

char * build_path(char * sFolderPath, char * sFileName)
{
    char * m_sResult = (char *) malloc(strlen(sFolderPath) + strlen(sFileName) + 2);
    strcpy(m_sResult, sFolderPath);
    strcat(m_sResult, "/");
    strcat(m_sResult, sFileName);
    return m_sResult;
} // char * build_path(char * sFolderPath, char * sFileName)

const char TARGET_FILE_EXTENSION[10] = ".htm";

char * GetAbsolutePathFileNameOfNode(char * sFolder, int iIdNodo)
{
    char * m_sIdNodo = my_itoa(iIdNodo);
    char * m_sFileName = (char *) malloc(strlen(TARGET_FILE_EXTENSION) + strlen(m_sIdNodo) + 1);
    strcpy(m_sFileName, m_sIdNodo);
    strcat(m_sFileName, TARGET_FILE_EXTENSION);
    char * m_sTargetFile = build_path(sFolder, m_sFileName);
    
    free(m_sIdNodo); m_sIdNodo = NULL;
    free(m_sFileName); m_sFileName = NULL;
    
    return m_sTargetFile;
} // char * GetAbsolutePathFileNameOfNode(char * sFolder, int iIdNodo)

void save_file_contents_of_root(struct node * RootStructure, char * sTargetFolder, char * sHeaderContents)
{
    if(RootStructure != NULL)
    {
        save_file_contents_of_root(RootStructure->left_child, sTargetFolder, sHeaderContents);
        
        if (RootStructure->sFileContents != NULL)
        {
            char * m_sContents = get_contents_of_file(sHeaderContents, RootStructure->sFileContents);
            char * m_sTargetFile = GetAbsolutePathFileNameOfNode(sTargetFolder, RootStructure->iNodo);
            create_text_file(m_sTargetFile, m_sContents);
            free(m_sTargetFile); m_sTargetFile = NULL;
            free(m_sContents); m_sContents = NULL;
        } // if (RootStructure->sFileContents != NULL)
        
        save_file_contents_of_root(RootStructure->right_child, sTargetFolder, sHeaderContents);
    } // if(RootStructure != NULL)
} // void save_file_contents_of_root(struct node * RootStructure, char * sTargetFolder, char * sHeaderContents)

char * get_extension_of_filename(char * sFileName)
{
	char * m_sExtension = (char *) malloc(strlen(sFileName)+1);
	strcpy(m_sExtension, "");
    int m_iPosition = lastIndexOf(sFileName, ".", strlen(sFileName) - 1, 1);
    
    if (m_iPosition >= 0)
        strcpy(m_sExtension, &sFileName[m_iPosition]);
        
    return m_sExtension;
} // char * get_extension_of_filename(char * sFileName)

char SOURCE_EXTENSION_FILENAME[10] = ".html";

int es_archivo_adjunto(char * sFileName)
{
	int m_iEsArchivoAdjunto = 0;
	char * m_sExtension = get_extension_of_filename(sFileName);
	
	if (strcmp(m_sExtension, SOURCE_EXTENSION_FILENAME) != 0)
        m_iEsArchivoAdjunto = 1;
        
    free(m_sExtension);
    m_sExtension = NULL;
    return m_iEsArchivoAdjunto;
} // int es_archivo_adjunto(char * sFileName)

int es_archivo_origen(char * sFileName)
{
	int m_iEsArchivoOrigen = 0;
	char * m_sExtension = get_extension_of_filename(sFileName);
	
	if (strcmp(m_sExtension, SOURCE_EXTENSION_FILENAME) == 0)
        m_iEsArchivoOrigen = 1;
        
    free(m_sExtension);
    m_sExtension = NULL;
    return m_iEsArchivoOrigen;
} // int es_archivo_origen(char * sFileName)

int copy_file(char * sSourceFile, char * sTargetFile)
{    
  int m_iResult = 1;
            
  int m_iFdSource = open(sSourceFile, O_RDONLY);
  struct stat m_SourceStat;
  
  if(m_iFdSource == -1)
  {
    perror(sSourceFile);
    m_iResult = 0;
  } // if(m_iFdSource == -1)
  
  else
  {	  
    int m_iFdTarget = open(sTargetFile, O_CREAT | O_WRONLY | O_TRUNC, 0777);
    
    if (m_iFdTarget == -1)
    {
      perror(sTargetFile);
      m_iResult = 0;
    } // if (m_iFdTarget == -1)
    
    else
    {
      fstat(m_iFdSource, &m_SourceStat);
      
      off64_t m_iNumBytesToWrite = m_SourceStat.st_size;
      off64_t m_iNumBytesWrited = 0;
      
      do
      {
        m_iNumBytesWrited = copy_file_range(m_iFdSource, NULL, m_iFdTarget, NULL, m_iNumBytesToWrite, 0);
           
        if (m_iNumBytesWrited == -1)
        {
          perror("copy_file_range");
          m_iResult = 0;
        } // if (m_iNumBytesWrited == -1)

        else
          m_iNumBytesToWrite -= m_iNumBytesWrited;
      } while (m_iNumBytesToWrite > 0 && m_iNumBytesWrited > 0);
      
      close(m_iFdTarget);
    } // else
    
    close(m_iFdSource);
  } // else
    
  return m_iResult;
} // int copy_file(char * sSourceFile, char * sTargetFile)

void copiar_archivos_adjuntos(char * sInFolderPath, char * sToFolderPath)
{
	struct dirent **namelist;
    int n;
    
    n = scandir(sInFolderPath, &namelist, NULL, alphasort);
    if (n == -1)
        perror("scandir");
    else
    {		
        while (n--)
        {
			struct stat m_FileStats;
		    char * m_sSourceFile = build_path(sInFolderPath, namelist[n]->d_name);
			
            stat(m_sSourceFile, &m_FileStats);
            
            if (S_ISREG(m_FileStats.st_mode))
            {
			    int m_iEsArchivoAdjunto = es_archivo_adjunto(namelist[n]->d_name);
            
                if (m_iEsArchivoAdjunto)
                {
                    char * m_sTargetFile = build_path(sToFolderPath, namelist[n]->d_name);
                    copy_file(m_sSourceFile, m_sTargetFile);
                    free(m_sTargetFile);
                    m_sSourceFile = NULL;
                } // if (m_iEsArchivoAdjunto)
            } // if (S_ISREG(m_FileStats.st_mode))
            
            free(m_sSourceFile);
            m_sSourceFile = NULL;
            free(namelist[n]);
        } // while (n--)
    } // else
} // void copiar_archivos_adjuntos(char * sInFolderPath, char * sToFolderPath)

char * deme_archivo_origen(char * sInFolderPath, int * iNumError)
{
	char * m_sRetorno = NULL;
	int m_iCountOfSourceFileNames = 0;
	*iNumError = 0;
	
	struct dirent **namelist;
    int n;
    
    n = scandir(sInFolderPath, &namelist, NULL, alphasort);
    if (n == -1)
        perror("scandir");
    else
    {		
        while (n--)
        {
			struct stat m_FileStats;
			char * m_sSourceFile = build_path(sInFolderPath, namelist[n]->d_name);
            int m_iFreeSourceFileMemory = 1;
			
            stat(m_sSourceFile, &m_FileStats);
            
            if (S_ISREG(m_FileStats.st_mode))
            {
			    int m_iEsArchivoOrigen = es_archivo_origen(namelist[n]->d_name);
            
                if (m_iEsArchivoOrigen)
                {
					m_iCountOfSourceFileNames += 1;
					
					if (m_iCountOfSourceFileNames > 1)
					    *iNumError = 2;
					    
					if (*iNumError == 0)
					{
					    m_sRetorno = m_sSourceFile;
					    m_iFreeSourceFileMemory = 0;
					} // if (*iNumError == 0)
                } // if (m_iEsArchivoOrigen)
            } // if (S_ISREG(m_FileStats.st_mode))
            
            if (m_iFreeSourceFileMemory)
            {
                free(m_sSourceFile);
                m_sSourceFile = NULL;
            } // if (m_iFreeSourceFileMemory)
            
            free(namelist[n]);
        } // while (n--)
    } // else
    
    if (m_iCountOfSourceFileNames == 0)
        *iNumError = 1;
  
	return m_sRetorno;
} // char * deme_archivo_origen(char * sInFolderPath, int * iNumError)

int FileExistsCheck(const char * sPath)
{
    if (access(sPath, F_OK) == -1)
        return 1;

    struct stat m_FileStats;

    stat(sPath, &m_FileStats);

    if (!S_ISREG(m_FileStats.st_mode))
        return 2;
        
    return 0;
} // int FileExistsCheck(const char * sPath)

char NOMBRE_ARCHIVO_ARBOL[20] = "Arbol.sqlite3";

char * DemeRutaArchivoIdPublicacion(char * sTargetFolder)
{
    char m_sNombreArchivo[20] = "IdPublicacion.txt";
    char * m_sRutaArchivoPublicacion = build_path(sTargetFolder, m_sNombreArchivo);
    return m_sRutaArchivoPublicacion;
} // char * DemeRutaArchivoIdPublicacion(char * sTargetFolder)

void run(char * sSourceFolder, char * sTargetFolder, char * sNombreLibroElectronico, char * sIdPublicacion)
{
	int m_iResult = 0;
	int m_iOK = 0;
	char * m_sContents = NULL;
	int m_iContentsLength = 0;
	
	int m_iNumError = 0;
		
	char * m_sCurrentDirName = get_current_dir_name();
	char * m_sSourceDatabaseFilePath = build_path(m_sCurrentDirName, NOMBRE_ARCHIVO_ARBOL);
	m_iNumError = FileExistsCheck(m_sSourceDatabaseFilePath);
    free(m_sCurrentDirName);
    m_sCurrentDirName = NULL;
    
    char * m_sArchivoXProcesar = NULL;
            
	if (m_iNumError == 0)
	{
	    m_sArchivoXProcesar = deme_archivo_origen(sSourceFolder, &m_iNumError);
	    
	    if (m_iNumError == 1)
	        fprintf(stderr, "No existe ningún archivo con la extensión '%s' en la carpeta %s\n", SOURCE_EXTENSION_FILENAME, sSourceFolder);

	    else if (m_iNumError == 2)
	        fprintf(stderr, "Hay más de un archivo con la extensión '%s' en la carpeta %s\n", SOURCE_EXTENSION_FILENAME, sSourceFolder);
	} // if (m_iNumError == 0)

	else if (m_iNumError == 1)
        fprintf(stderr, "No existe el archivo %s\n", m_sSourceDatabaseFilePath);

	else if (m_iNumError == 2)
        fprintf(stderr, "%s no es un archivo\n", m_sSourceDatabaseFilePath);
		
	if (m_iNumError == 0)
	{		
		printf("Cargando el archivo %s\n", m_sArchivoXProcesar);
	    m_iResult = load_text_file(m_sArchivoXProcesar, &m_sContents, &m_iContentsLength);
				
	    if (m_iResult)
	    {
			printf("Extrayendo el encabezado del archivo %s\n", m_sArchivoXProcesar);
		    char * m_sHeader = extract_header(m_sContents, &m_iOK);
		
		    if (m_iOK)
		    {
			    int m_iPositionFrom = 0;
			    int m_iPositionTo = 0;
			    printf("Extrayendo el índice de contenidos del archivo %s\n", m_sArchivoXProcesar);
		        char * m_sContentsIndex = extract_contents_index(m_sContents, &m_iOK, &m_iPositionFrom, &m_iPositionTo);
		    		    	    
		        if (m_iOK)
		        {
				    int m_iIdPadreRaiz = -1;
				    int m_iIdNodoRaiz = 1;
				    int m_iSiguienteNodo = m_iIdNodoRaiz + 1;
				    int m_iStartFromPosition = 0;
				    struct node * m_Root = NULL;
				    printf("Extrayendo la estructura del índice de contenidos\n");
				    m_Root = build_tree_structure(m_Root, m_sContentsIndex, m_iIdNodoRaiz, &m_iSiguienteNodo, 0, &m_iStartFromPosition);
				    char * m_sTargetDatabaseFilePath = build_path(sTargetFolder, NOMBRE_ARCHIVO_ARBOL);
				    printf("Copiando el archivo de la base de datos en %s\n", m_sTargetDatabaseFilePath);
				    copy_file(m_sSourceDatabaseFilePath, m_sTargetDatabaseFilePath);
				    printf("Creando la base de datos en %s\n", m_sTargetDatabaseFilePath);
				    create_database(m_sTargetDatabaseFilePath, m_Root, sNombreLibroElectronico, m_iIdNodoRaiz, m_iIdPadreRaiz);
		                    free(m_sTargetDatabaseFilePath);
		                    m_sTargetDatabaseFilePath = NULL;
   			            m_sContents = str_replace(m_sContents, m_iPositionFrom, m_iPositionTo, "", m_iContentsLength, 0);
                    		    printf("Obteniendo el código HTML de los archivos de documentación\n");
		                    m_iContentsLength = m_iContentsLength - (m_iPositionTo - m_iPositionFrom + 1);
				    change_file_contents_into_root(m_sContents, m_Root, m_iContentsLength);
				    printf("Guardando los archivos de documentación en %s\n", sTargetFolder);
				    save_file_contents_of_root(m_Root, sTargetFolder, m_sHeader);
				    printf("Guardando los archivos adjuntos en %s\n", sTargetFolder);
				    copiar_archivos_adjuntos(sSourceFolder, sTargetFolder);
				    delete_tree(m_Root);

	  	            char * m_sContentsIndexFile = get_contents_of_file(m_sHeader, m_sContentsIndex);
                    char * m_sTargetFileOfRootNode = GetAbsolutePathFileNameOfNode(sTargetFolder, m_iIdNodoRaiz);
                    printf("Creando el archivo de índice de contenidos en %s\n", m_sTargetFileOfRootNode);
				    m_iResult = create_text_file(m_sTargetFileOfRootNode, m_sContentsIndexFile);
				    free(m_sTargetFileOfRootNode); m_sTargetFileOfRootNode = NULL;

                            char * m_sRutaArchivoPublicacion = DemeRutaArchivoIdPublicacion(sTargetFolder);
                            printf("Creando el archivo de id de publicación en %s\n", m_sRutaArchivoPublicacion);
                            m_iResult =  create_text_file(m_sRutaArchivoPublicacion, sIdPublicacion);
                            free(m_sRutaArchivoPublicacion); m_sRutaArchivoPublicacion = NULL;
                            
		            if (m_sContentsIndexFile != NULL)
		                free(m_sContentsIndexFile);
		                
		            m_sContentsIndexFile = NULL;
		        } // if (m_iOK)
		        
		        if (m_iResult)
		            m_iOK = 1;
		            
                if (m_sContentsIndex != NULL)
		            free(m_sContentsIndex);
		        
		        m_sContentsIndex = NULL;
		    } // if (m_iOK)		    
		    
            if (m_sHeader != NULL)
                free(m_sHeader);
        
            m_sHeader = NULL;		    
        } // if (m_iOK)        
    } // if (m_iResult)

    if (m_sContents != NULL)
        free(m_sContents);
        
    m_sContents = NULL;
    
    if (m_sArchivoXProcesar != NULL)
        free(m_sArchivoXProcesar);
      
    m_sArchivoXProcesar = NULL;
    
    if (m_sSourceDatabaseFilePath != NULL)
        free(m_sSourceDatabaseFilePath);
        
    m_sSourceDatabaseFilePath = NULL;
} // void run(char * sSourceFolder, char * sTargetFolder, char * sNombreLibroElectronico, char * sIdPublicacion)

int FolderExistsCheck(const char * sPath)
{
    if (access(sPath, F_OK) == -1)
        return 1;

    struct stat m_FileStats;

    stat(sPath, &m_FileStats);

    if (!S_ISDIR(m_FileStats.st_mode))
        return 2;
        
    return 0;
} // int FolderExistsCheck(const char * sPath)

int IsFolderEmpty(char * sFolderPath)
{
	int m_iIsFolderEmpty = 1;
	struct dirent **namelist;
    int n;
    
    n = scandir(sFolderPath, &namelist, NULL, alphasort);
    if (n == -1)
        perror("scandir");
    else
    {		
        while (n--)
        {
		    char * m_sFilePath = namelist[n]->d_name;

            if (strcmp(m_sFilePath, ".") == 0);
            
            else if (strcmp(m_sFilePath, "..") == 0);
			
			else
			{
				m_sFilePath = build_path(sFolderPath, namelist[n]->d_name);
				struct stat m_FileStats;
                stat(m_sFilePath, &m_FileStats);
                        
                if (S_ISREG(m_FileStats.st_mode) || S_ISDIR(m_FileStats.st_mode))
                    m_iIsFolderEmpty = 0;

                free(m_sFilePath);
                m_sFilePath = NULL;
            } // else
            
            free(namelist[n]);
        } // while (n--)
    } // else
    
    return m_iIsFolderEmpty;
} // int IsFolderEmpty(char * sFolderPath)

int main(int argc, char **argv)
{    
	int m_iHayError = 0;
	char * m_sFolderOrigen = NULL;
	char * m_sFolderDestino = NULL;
	char * m_sNombreLibroElectronico = NULL;
	char * msIdPublicacion = NULL;
	
	if (argc != 5)
	{
	    fprintf(stderr, "Utilice: %s FOLDER_ORIGEN FOLDER_DESTINO NOMBRE_LIBRO_ELECTRONICO ID_PUBLICACION\n", argv[0]);
	    m_iHayError = 1;
	} // if (argc != 5)
		
	if (!m_iHayError)
	{
		m_sFolderOrigen = argv[1];
		int m_iNumError = FolderExistsCheck(m_sFolderOrigen);
		
		if (m_iNumError == 1)
	        fprintf(stderr, "No existe la carpeta %s\n", m_sFolderOrigen);

		else if (m_iNumError == 2)
	        fprintf(stderr, "%s no es una carpeta\n", m_sFolderOrigen);
	        
	    if (m_iNumError != 0)    
	        m_iHayError = 1;
	} // if (!m_iHayError)
	
	if (!m_iHayError)
	{
		m_sFolderDestino = argv[2];
		int m_iNumError = FolderExistsCheck(m_sFolderDestino);
		
		if (m_iNumError == 1)
	        fprintf(stderr, "No existe la carpeta %s\n", m_sFolderDestino);

		else if (m_iNumError == 2)
	        fprintf(stderr, "%s no es una carpeta\n", m_sFolderDestino);
	        
	    if (m_iNumError != 0)    
	        m_iHayError = 1;
	} // if (!m_iHayError)

    if (!m_iHayError)
    {
		if (!IsFolderEmpty(m_sFolderDestino))
		{
		    fprintf(stderr, "La carpeta %s no está vacía\n", m_sFolderDestino);
		    m_iHayError = 1;
		} // if (!IsFolderEmpty(m_sFolderDestino))
    } // if (!m_iHayError)

    if (!m_iHayError)
        m_sNombreLibroElectronico = argv[3];
    
    if (!m_iHayError)    
        msIdPublicacion = argv[4];
        
    if (!m_iHayError)
        run(m_sFolderOrigen, m_sFolderDestino, m_sNombreLibroElectronico, msIdPublicacion);
    
    exit(1);
} // int main()
