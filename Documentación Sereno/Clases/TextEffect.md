#Clase
Clase serializable que representa un efecto visual aplicado a una porción de texto. Contiene el tipo de efecto ([[TextEffectType]]), el rango de caracteres afectados y parámetros opcionales.

**_Propiedades_**:

- `type`: El tipo de efecto ([[TextEffectType]])
- `startIndex`: Índice inicial del efecto en el texto limpio
- `endIndex`: Índice final del efecto en el texto limpio
- `parameter`: Parámetro opcional (ej: color, tamaño, velocidad, clave de imagen)