#Clase
Clase serializable que asocia un [[DialogueObject]] con condiciones de eventos y objetos. Permite configurar condiciones tanto para el diálogo principal como para cada respuesta individual.

**_Propiedades_**:

- `dialogueObject`: El [[DialogueObject]] asociado
- `hasAssociatedEvent`: Indica si el diálogo tiene eventos asociados
- `associatedEvents`: Lista de [[EventData]] para el diálogo principal
- `hasAssociatedObject`: Indica si el diálogo tiene objetos asociados
- `associatedObjects`: Lista de [[ObjectData]] para el diálogo principal
- `eventResponseAssociated`: Lista que indica qué respuestas tienen eventos asociados
- `eventsAssociatedPerResponse`: Lista de [[EventDataList]] para cada respuesta
- `objectResponseAssociated`: Lista que indica qué respuestas tienen objetos asociados
- `objectsAssociatedPerResponse`: Lista de [[ObjectDataList]] para cada respuesta