#Script
Gestiona los eventos asociados a cada respuesta de múltiples [[DialogueObject]]. Automáticamente sincroniza los eventos con las respuestas disponibles en cada diálogo durante OnValidate. Crea un evento adicional "END_CONVERSATION" para cada diálogo.

**_Propiedades_**:

- `DREvents`: Lista de [[DialogueResponseEventType]]

**_Funciones_**:

- `OnValidate`: Sincroniza automáticamente los eventos con las respuestas de los diálogos