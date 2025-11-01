#Script
Vincula diálogos con condiciones de inventario (eventos y objetos). Permite mostrar u ocultar diálogos y respuestas basándose en el estado del inventario del jugador. Utiliza [[InventoryDialogueEntry]] para asociar cada [[DialogueObject]] con sus condiciones.

**_Funciones principales_**:

- [[GetEntryWithDialogueObject]]: Obtiene la entrada asociada a un diálogo
- [[TryGetEventos]]: Obtiene los eventos asociados al diálogo principal
- [[TryGetObjetos]]: Obtiene los objetos asociados al diálogo principal
- [[TryGetEventData]]: Obtiene los datos de eventos del diálogo principal
- [[TryGetObjectData]]: Obtiene los datos de objetos del diálogo principal
- [[TryGetEventosFromResponse]]: Obtiene los eventos de una respuesta específica
- [[TryGetObjetosFromResponse]]: Obtiene los objetos de una respuesta específica
- [[TryGetEventDataFromResponse]]: Obtiene los datos de eventos de una respuesta
- [[TryGetObjectDataFromResponse]]: Obtiene los datos de objetos de una respuesta
- [[CanShowDialogue]]: Verifica si se puede mostrar el diálogo según condiciones

**_Referencias_**:

- Utiliza [[InventarioObject]] para acceder a la base de datos de eventos y objetos
- Utiliza [[InventoryManager]] para verificar el estado actual del inventario
- Trabaja con [[EventData]] y [[ObjectData]] para las condiciones