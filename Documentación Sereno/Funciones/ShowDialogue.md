#Función
Muestra un diálogo en pantalla. Configura los personajes, inicia el [[TypewritterEffect]] y gestiona las respuestas si las hay. Pausa el juego usando [[GameManager]].

**_Parámetros_**:

- `dialogueObject`: El [[DialogueObject]] a mostrar
- `linker`: (Opcional) [[InventoryDialogueLinker]] para verificar condiciones
- `endEvent`: (Opcional) [[ResponseEvent]] a ejecutar al finalizar

**_Ubicación_**: [[DialogueUI]]