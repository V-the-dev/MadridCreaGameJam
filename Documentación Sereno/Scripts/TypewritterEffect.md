#Script
Gestiona el efecto de máquina de escribir para mostrar el texto letra por letra. Aplica efectos visuales al texto como shake, wave, rainbow, cambios de tamaño, color y velocidad. Utiliza [[ParsedText]] procesado por [[TextEffectParser]] para aplicar los efectos correctamente.

**_Funciones principales_**:

- [[Run]]: Inicia el efecto de escritura con un [[ParsedText]]
- [[Stop]]: Detiene el efecto y muestra todo el texto
- [[StopEffects]]: Detiene solo los efectos visuales

**_Efectos aplicables_**:

- `ApplyShakeEffect`: Aplica temblor al texto usando [[TextEffect]] de tipo Shake
- `ApplyWaveEffect`: Aplica movimiento ondulatorio usando [[TextEffect]] de tipo Wave
- `ApplyRainbowEffect`: Aplica colores arcoíris usando [[TextEffect]] de tipo Rainbow
- `ApplySizeEffect`: Cambia el tamaño del texto usando [[TextEffect]] de tipo Size
- `ApplyColorEffect`: Cambia el color del texto usando [[TextEffect]] de tipo Color
- `GetSpeedMultiplier`: Obtiene el multiplicador de velocidad usando [[TextEffect]] de tipo Speed

**_Configuración_**:

- `typewriterSpeed`: Velocidad de escritura en caracteres por segundo
- `shakeIntensity`: Intensidad del efecto de temblor
- `waveIntensity`: Intensidad del efecto de onda
- `waveSpeed`: Velocidad del efecto de onda
- `rainbowSpeed`: Velocidad del efecto arcoíris
- `punctuations`: Lista de puntuaciones que añaden pausas