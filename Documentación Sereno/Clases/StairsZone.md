#Clase
Componente del objeto tipo stairs que obtiene su inclinación en la escena al iniciarse el juego para calcular los nuevos multiplicadores de movimiento del player.

Estos multiplicadores son actualizados en [[PlayerMovement]] al entrar o salir del collider correspondiente.

**_Propiedades_**:

- `anglez`: ángulo en la escena, capado a ser igual o menor de 180 y en valor positivo
- `newVec`: nuevos multiplicadores para el player al estar en la zona