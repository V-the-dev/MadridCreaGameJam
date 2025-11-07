#Clase 
Clase de control de movimiento de cámara para cambiar su posición de manera fluida en base al movimiento del jugador y las condiciones del mapa.

Usa un objeto con rigidbody y collider para usar el movimiento de rigidbody y chocarse contra los limites de cámara. La cámara en sí es un objeto hijo ligado al "follower", el cual sigue al target.

**_Propiedades_**:

- `dampTime`: velocidad de movimiento del follower
- `target`: objetivo al que va a seguir el follower