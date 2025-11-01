#Script
Es el controlador principal de la interfaz de usuario del sistema de diálogos. Gestiona la visualización de los personajes, el texto con efecto de máquina de escribir ([[TypewritterEffect]]), las respuestas del jugador ([[ResponseHandler]]) y las imágenes en línea ([[InlineImageHandler]]). Controla el posicionamiento y enfoque de los personajes durante la conversación.

**_Funciones principales_**:

- [[ShowDialogue]]: Inicia la visualización de un diálogo
- [[CloseDialogueBox]]: Cierra el cuadro de diálogo y limpia todos los recursos
- [[SetCharacterSpritesInScene]]: Gestiona la aparición y desaparición de personajes
- [[FocusOnCharacter]]: Enfoca visualmente en el personaje que está hablando
- [[SetTalkingOrders]]: Actualiza qué personaje está hablando en cada línea
- [[SetNameAndTitle]]: Actualiza el nombre y título del personaje que habla
- [[AddResponseEvents]]: Añade eventos de respuesta al [[ResponseHandler]]
- [[ClearResponseEvents]]: Limpia los eventos de respuesta

**_Referencias_**:

- Utiliza [[TypewritterEffect]] para el efecto de escritura
- Utiliza [[ResponseHandler]] para gestionar las respuestas
- Utiliza [[InlineImageHandler]] para mostrar imágenes en el texto
- Utiliza [[TextEffectParser]] para procesar efectos de texto
- Interactúa con [[GameManager]] para pausar/reanudar el juego
- Interactúa con [[InventoryManager]] para verificar condiciones de objetos y eventos