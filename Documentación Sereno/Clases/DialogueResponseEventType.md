#Clase
Clase serializable que asocia un [[DialogueObject]] con sus [[ResponseEvent]]. Contiene un array de eventos donde cada índice corresponde a una respuesta, más un evento final para END_CONVERSATION.

**_Propiedades_**:

- `DialogueObject`: El diálogo asociado
- `Events`: Array de [[ResponseEvent]], uno por cada respuesta más uno final