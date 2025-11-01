#Script
Es un ScriptableObject que contiene toda la información necesaria para una conversación completa. Almacena los sprites de los personajes, sus nombres, títulos, posiciones (izquierda/derecha), las líneas de diálogo y las posibles respuestas. Utiliza el enum [[SpriteSide]] para determinar en qué lado de la pantalla aparece cada personaje. Cada línea de diálogo es un [[ConversationLine]] que contiene el texto y el índice del sprite del personaje que habla.

**_Propiedades_**:

- `characters`: Array de sprites de los personajes
- `charactersName`: Array de nombres de los personajes
- `charactersTitle`: Array de títulos de los personajes
- `charactersSide`: Array de [[SpriteSide]] que determina la posición de cada personaje
- `mainCharacterWhenResponses`: Booleano que indica si se debe enfocar al personaje principal durante las respuestas
- `mainCharacterIndex`: Índice del personaje principal
- `conversationLine`: Array de [[ConversationLine]] con las líneas del diálogo
- `responses`: Array de [[Response]] con las posibles respuestas del jugador

**_Propiedades de Acceso_**:

- `Dialogue`: Devuelve solo los textos de las líneas de conversación
- `SpriteIndexes`: Devuelve los índices de sprites de cada línea
- `HasResponses`: Indica si el diálogo tiene respuestas disponibles