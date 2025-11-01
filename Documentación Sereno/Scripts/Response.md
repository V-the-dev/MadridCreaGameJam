#Script
Clase serializable que representa una opción de respuesta para el jugador. Contiene el texto de la respuesta y opcionalmente un [[DialogueObject]] que se ejecutará si se selecciona esta respuesta.

**_Propiedades_**:

- `ResponseText`: El texto que se muestra al jugador
- `DialogueObject`: El [[DialogueObject]] que se ejecuta al seleccionar esta respuesta (puede ser null si termina la conversación)