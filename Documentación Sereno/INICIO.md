# Resumen

### _Documentación del sistema de dialogo_:

En esta documentación se definen las utilidades de las funciones de cada script relacionado con el sistema de conversaciones del juego.

Las funciones privadas no se definen, ya que normalmente solo se utilizarán de manera interna en el propio script donde están definidas, pero se pueden revisar.
# Sistemas
### Sistema de Filtrado de Respuestas

El sistema permite mostrar u ocultar respuestas dinámicamente basándose en:

- Estado de eventos del inventario (verificado mediante [[EventData]])
- Cantidad de objetos del inventario (verificado mediante [[ObjectData]])
- Las condiciones se evalúan en [[DialogueUI]].StepThroughDialogue
- Solo las respuestas que cumplen todas las condiciones se muestran

### Sistema de Efectos de Texto

Soporta múltiples efectos visuales que pueden combinarse:

- **Shake**: Temblor aleatorio del texto
- **Wave**: Movimiento ondulatorio sinusoidal
- **Rainbow**: Colores que cambian con el tiempo
- **Size**: Escala del texto con reposicionamiento automático
- **Color**: Color personalizado con formato hexadecimal
- **Speed**: Control de velocidad de escritura por segmentos
- **Image**: Inserción de sprites en línea con el texto

### Sistema de Imágenes en Línea

- Las imágenes se insertan en el texto como espacios
- Se calculan dinámicamente según el tamaño de la fuente
- Se posicionan frame a frame para seguir el texto
- Se muestran/ocultan según el progreso del [[TypewritterEffect]]
- Utilizan [[InlineImageData]] para mapear claves a sprites

### Sistema de Eventos de Respuesta

Cada respuesta puede ejecutar múltiples tipos de eventos:

- Eventos Unity sin parámetros
- Eventos con un parámetro string
- Eventos con string e int
- Eventos con string y bool
- Evento especial "END_CONVERSATION" añadido automáticamente

### Sistema de Posicionamiento de Personajes

- Personajes se posicionan en dos grupos: izquierda y derecha
- Espaciado automático entre personajes del mismo lado
- Sistema de enfoque que oscurece personajes no activos
- Reposicionamiento dinámico al añadir/eliminar personajes
- Los personajes persisten entre líneas del mismo diálogo
# Scripts
[[DialogueActivator]]
[[DialogueObject]]
[[DialogueResponseEvents]]
[[DialogueUI]]
[[InlineImageHandler]]
[[InventoryDialogueLinker]]
[[Response]]
[[ResponseEvent]]
[[ResponseHandler]]
[[TextEffectParser]]
[[TypewritterEffect]]
[[MessageManager]]

# Flujo del sistema
### Inicio de Diálogo

1. El jugador interactúa con un objeto que tiene [[DialogueActivator]]
2. Se llama a [[MessageManager]].[[Interactuar]]
3. [[DialogueActivator]].[[Interact]] se ejecuta
4. Se buscan [[DialogueResponseEvents]] asociados al [[DialogueObject]]
5. [[DialogueUI]].[[ShowDialogue]] inicia la visualización

### Procesamiento de Texto

1. [[DialogueUI]] obtiene cada línea de diálogo del [[DialogueObject]]
2. [[TextEffectParser]].[[Parse]] procesa el texto y genera un [[ParsedText]]
3. [[InlineImageHandler]].[[ProcessImages]] crea las imágenes en línea
4. [[TypewritterEffect]].[[Run]] inicia el efecto de escritura
5. Los efectos visuales se aplican frame a frame

### Selección de Respuestas

1. Al terminar el diálogo, [[DialogueUI]] filtra respuestas según [[InventoryDialogueLinker]]
2. [[ResponseHandler]].[[ShowResponses]] muestra los botones
3. El jugador selecciona una respuesta
4. [[ResponseHandler]].[[OnPickedResponse]] ejecuta el [[ResponseEvent]]
5. Si hay un [[DialogueObject]] asociado, se muestra el siguiente diálogo
6. Si no, se llama a [[CloseDialogueBox]]

### Cierre de Diálogo

1. [[DialogueUI]].[[CloseDialogueBox]] limpia todos los recursos
2. Se destruyen los GameObjects de personajes
3. [[InlineImageHandler]].[[ClearImages]] elimina las imágenes
4. [[TypewritterEffect]].[[StopEffects]] detiene los efectos
5. [[GameManager]].ResumeGame() reanuda el juego
6. PlayerMovement.CheckIfMoreDialogues() verifica diálogos pendientes
# Dependencias
MessageManager (Singleton)
    └── DialogueUI
        ├── TypewritterEffect
        │   └── ParsedText (de TextEffectParser)
        ├── ResponseHandler
        │   └── Response
        │       └── DialogueObject
        ├── InlineImageHandler
        │   └── InlineImageData
        └── InventoryDialogueLinker
            └── InventoryDialogueEntry
                ├── EventData
                └── ObjectData

DialogueActivator
    ├── DialogueObject
    │   ├── ConversationLine
    │   └── Response
    ├── DialogueResponseEvents
    │   └── DialogueResponseEventType
    │       └── ResponseEvent
    │           ├── CustomStringEvent
    │           ├── CustomStringIntEvent
    │           └── CustomStringBoolEvent
    └── InventoryDialogueLinker

TextEffectParser (Estático)
    └── ParsedText
        └── TextEffect
            └── TextEffectType (Enum)
# Anotaciones
### Sincronización en Editor

- [[DialogueResponseEvents]].OnValidate sincroniza automáticamente los eventos con las respuestas
- Crea un evento por cada respuesta más uno para END_CONVERSATION
- Se ejecuta automáticamente en el Editor de Unity al modificar valores

### Gestión de Memoria

- [[DialogueUI]].[[CloseDialogueBox]] destruye explícitamente todos los GameObjects creados
- [[InlineImageHandler]].[[ClearImages]] limpia las imágenes al cambiar de diálogo
- Los tempResponseButtons se limpian después de cada selección
- Las corrutinas se detienen explícitamente antes de iniciar nuevas

### Tiempo Real

- El [[TypewritterEffect]] usa `WaitForSecondsRealtime` para funcionar durante pausas
- Los efectos visuales usan `Time.unscaledTime` para continuar durante pausas
- Esto permite que los diálogos funcionen mientras el juego está en pausa

### Espacios para Imágenes

- [[TextEffectParser]] calcula el número de espacios necesarios para cada imagen
- Usa [[InlineImageHandler]].[[GetImageWidth]] y [[GetSpaceWidth]]
- Los espacios se insertan en el `cleanText` del [[ParsedText]]
- Las imágenes se posicionan sobre estos espacios

### Índices de Respuestas Filtradas

- [[ResponseHandler]] mantiene `originalResponseIndices` para respuestas filtradas
- Esto asegura que los [[ResponseEvent]] correctos se ejecuten
- Necesario porque el filtrado puede cambiar los índices de las respuestas mostradas