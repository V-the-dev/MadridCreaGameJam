#Función
Ejecuta la lógica cuando se selecciona una respuesta. Oculta los botones, ejecuta el [[ResponseEvent]] correspondiente. Si la respuesta tiene un [[DialogueObject]], llama a [[ShowDialogue]]. Si no, llama a [[CloseDialogueBox]]. Detiene efectos del [[TypewritterEffect]] antes de cambiar de diálogo.

**_Parámetros_**:

- `response`: La [[Response]] seleccionada
- `responseIndex`: Índice de la respuesta seleccionada

**_Ubicación_**: [[ResponseHandler]]