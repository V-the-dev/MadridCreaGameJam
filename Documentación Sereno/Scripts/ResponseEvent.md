#Script
Clase serializable que contiene eventos Unity que se ejecutan cuando se selecciona una respuesta o termina una conversación. Soporta múltiples tipos de eventos: sin parámetros, con string, con string e int, y con string y bool.

**_Funciones_**:

- [[Invoke]]: Ejecuta todos los eventos configurados

**_Tipos de eventos_**:

- `onPickedResponse`: UnityEvent sin parámetros
- `customStringEvents`: Lista de [[CustomStringEvent]]
- `customStringIntEvents`: Lista de [[CustomStringIntEvent]]
- `customStringBoolEvents`: Lista de [[CustomStringBoolEvent]]