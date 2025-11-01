#Función
Verifica si un diálogo puede mostrarse basándose en las condiciones de eventos y objetos del inventario. Consulta [[InventoryManager]] para verificar el estado actual. Devuelve true solo si todas las condiciones se cumplen.

**_Parámetros_**:

- `dialogueObject`: El [[DialogueObject]] a verificar

**_Retorna_**: bool indicando si se puede mostrar, o null si el diálogo es null

**_Ubicación_**: [[InventoryDialogueLinker]]