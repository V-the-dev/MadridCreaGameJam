#Función
Muestra los botones de respuesta en pantalla. Crea dinámicamente un botón por cada [[Response]] usando el template. Configura el EventSystem para seleccionar el primer botón.

**_Parámetros_**:

- `responses`: Array de [[Response]] a mostrar
- `originalIndices`: (Opcional) Índices originales de las respuestas (para respuestas filtradas)

**_Ubicación_**: [[ResponseHandler]]