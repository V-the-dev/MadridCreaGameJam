#Script
Singleton que gestiona el sistema de mensajes y diálogos en el juego. Mantiene una referencia al [[DialogueUI]] y al [[DialogueActivator]] activo. Actúa como punto de acceso global para interactuar con el sistema de diálogos.

**_Propiedades_**:

- `Instance`: Instancia singleton del MessageManager
- `DialogueUI`: Referencia al [[DialogueUI]]
- `dialogueActivator`: Referencia al [[DialogueActivator]] actualmente activo

**_Funciones_**:

- [[Interactuar]]: Inicia una interacción con el [[DialogueActivator]] actual