#Clase
Clase que gestiona el movimiento, animaciones e interacciones del jugador, incluyendo el manejo de objetos equipados como la lámpara o el chuzo, los sonidos de pasos y la detección de objetos interactuables cercanos.

**_Propiedades_**:
- `Componentes de Unity`: Rigidbody2D, Animator, PlayerInput  
- `messageManager`: Referencia al gestor de mensajes del juego.  
- `speed`: Multiplicador de velocidad del jugador (por defecto 5).  
- `movementMults`: Multiplicadores de movimiento en ejes X/Y.  
- `currentMults`: Multiplicadores de movimiento activos (pueden variar por escaleras).  
- `animatorSpeed`: Magnitud del vector de movimiento, usada para animar.  
- `withLamp`, `withChuzo`: Booleanos que indican si el jugador lleva la lámpara y/o el chuzo.  
- `animWithNOTHING`, `animWithLamp`, `animWithChuzo`, `animWithLampAndChuzo`: Controladores de animación según los objetos equipados.  
- `lampObject`: GameObject que representa la lámpara del jugador.  
- `lampPositions`: Array de posiciones predefinidas de la lámpara según la dirección del jugador.  
- `stepSoundDelay`: Tiempo entre sonidos de pasos.  
- `stepSoundMinPitch`, `stepSoundMaxPitch`: Rango de tono aleatorio para el sonido de pasos.  
- `playerInput`: Sistema de entrada del jugador.  
- `interactuables`: Diccionario que guarda los objetos interactuables dentro del área de interacción.  
- `nearest`: Referencia al objeto interactuable más cercano.  

**_Métodos de Unity_**:
- `Awake()`: Inicializa referencias a Rigidbody2D, Animator y PlayerInput. Guarda el controlador de animación por defecto.  

- `Update()`:  
  - Lee la entrada de movimiento.  
  - Actualiza la animación y dirección del jugador.  
  - Reproduce sonidos de pasos a intervalos.  
  - Detecta y activa interacciones (`Interact`) si hay objetos cercanos.  
  - Actualiza la posición de la lámpara si está equipada.  

- `FixedUpdate()`: Aplica el movimiento físico al jugador usando `MovePosition` del rigidbody.  

- `OnTriggerEnter2D(Collider2D)`:  
  - Añade objetos con tag `"Interactuable"` al diccionario.  
  - Activa interacciones automáticas con objetos `"Proximity"`.  
  - Ajusta los multiplicadores de movimiento al entrar en zonas `"Stairs"`.  

- `OnTriggerExit2D(Collider2D)`:  
  - Elimina objetos interactuables al salir del área.  
  - Restaura los multiplicadores de movimiento al salir de escaleras.  
