#Función
Guarda la función que se vaya a activar desde el [[PlayerMovement]] en el objeto interactuable. En este caso, de [[NPCController]], guarda la activación de un diálogo mediante la clase estática [[MessageManager]] y sus funciones [[UpdateDialogueObject]]  e  [[Interactuar]] .

SIempre se puede activar pero solo funciona si hay un [[InventoryDialogueLinker]] y diálogos disponibles.

**_Ubicación_**: [[NPCController]] , [[InteractableObject]]