#Función
Verifica si el npc debe mostrar el indicador de interacción (la exclamación) cuando está obligado por la entrada del player en la trigger zone de [[InteractableObject]].

Un npc solo debe mostrar la exclamación si tiene al menos un diálogo disponible, es decir cuando se pueda interactuar.

**_Parámetros_**:

- `dialogueObject`: El [[DialogueObject]] actual
- `dialogueIndex`: Índice de la línea de diálogo actual

**_Ubicación_**: [[NPCController]]