#Clase
Clase hija de [[InteractableObject]], enfocada a representar los npcs con opciones a diálogo , que generen eventos o den objetos.

**_Propiedades_**:

- - `interactionDialogues` : Lista de diálogos que mostrará al interactuar mediante [[Trigger]]
- - `proximityDialogue` : Lista de diálogos que mostrará al acercarse mediante [[AutoTrigger]]
- -  `inventoryDialogueLinker` : objeto [[InventoryDialogueLinker]] para gestionar disponibilidad de diálogos y respuestas