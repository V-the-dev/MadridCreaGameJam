#Script
Gestiona la visualización y selección de respuestas del jugador. Crea botones dinámicamente basados en las [[Response]] disponibles y ejecuta los [[ResponseEvent]] correspondientes. Filtra las respuestas según condiciones de inventario usando [[InventoryDialogueLinker]].

**_Funciones principales_**:

- [[AddResponseEventos]]: Asigna los eventos de respuesta
- [[ShowResponses]]: Muestra los botones de respuesta en pantalla
- [[OnPickedResponse]]: Ejecuta la lógica cuando se selecciona una respuesta

**_Referencias_**:

- Utiliza [[DialogueUI]] para mostrar el siguiente diálogo
- Utiliza [[TypewritterEffect]] para detener efectos antes de cambiar de diálogo
- Utiliza [[MessageManager]] para acceder al sistema de diálogos
- Utiliza [[DialogueActivator]] para obtener información de inventario
- Ejecuta [[ResponseEvent]] según la respuesta seleccionada