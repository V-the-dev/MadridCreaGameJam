#Script
Clase estática que parsea el texto con etiquetas de efectos y genera un [[ParsedText]]. Reconoce etiquetas como `<shake>`, `<wave>`, `<rainbow>`, `<size=1.5>`, `<color=#FF0000>`, `<speed=2>` y `<img=key>`. Calcula los espacios necesarios para las imágenes usando [[InlineImageHandler]].

**_Función principal_**:

- [[Parse]]: Procesa el texto con etiquetas y devuelve un [[ParsedText]]

**_Tipos de efectos_**: Utiliza [[TextEffectType]] para identificar el tipo de efecto.