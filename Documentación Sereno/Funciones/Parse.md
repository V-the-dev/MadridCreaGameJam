#Función
Procesa texto con etiquetas de efectos y devuelve un [[ParsedText]]. Reconoce etiquetas como `<shake>`, `<wave>`, `<color=#FF0000>`, etc. Para imágenes (`<img=key>`), calcula espacios necesarios usando [[InlineImageHandler]]. Elimina las etiquetas del texto y crea [[TextEffect]] con sus posiciones.

**_Parámetros_**:

- `text`: El texto con etiquetas a procesar
- `imageHandler`: (Opcional) [[InlineImageHandler]] para calcular espacios de imágenes

**_Retorna_**: [[ParsedText]] con el texto limpio y sus efectos

**_Ubicación_**: [[TextEffectParser]]