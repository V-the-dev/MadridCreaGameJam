#Script
Gestiona la inserción y posicionamiento de imágenes dentro del texto del diálogo. Utiliza una base de datos de [[InlineImageData]] para asociar claves con sprites. Actualiza la posición de las imágenes frame a frame para mantenerlas alineadas con el texto.

**_Funciones principales_**:

- [[Initialize]]: Inicializa el handler con el TMP_Text
- [[ProcessImages]]: Procesa y crea las imágenes basándose en un [[ParsedText]]
- [[GetImageWidth]]: Calcula el ancho de una imagen por su clave
- [[GetSpaceWidth]]: Calcula el ancho de un espacio en la fuente actual
- [[ClearImages]]: Elimina todas las imágenes activas

**_Funciones internas_**:

- `CreateInlineImage`: Crea un GameObject de imagen para un [[TextEffect]] de tipo Image
- `UpdateAllImagePositions`: Actualiza las posiciones de todas las imágenes activas
- `UpdateImagePosition`: Actualiza la posición de una imagen individual
- `GetImageDataByKey`: Busca un [[InlineImageData]] por su clave