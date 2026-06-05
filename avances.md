# Avances del videojuego Nova Break

Documento para explicar el trabajo realizado en el proyecto durante la sesion de desarrollo. La idea es tener una guia por si el profesor pregunta: que hicieron, donde le picaron, que bugs salieron y como se corrigieron.

## 1. Menu principal

### Que se hizo

Se arreglo la escena `Menu` para que los botones funcionaran correctamente:

- El boton `Jugar` carga la escena `Nivel1`.
- El boton `Salir` imprime un mensaje en consola y cierra la aplicacion cuando el juego esta compilado.

El script usado fue:

- `Assets/Scripts/Menu.cs`

La funcion principal es:

```csharp
public void Jugar()
{
    SceneManager.LoadScene("Nivel1");
}
```

### Donde se configuro en Unity

En la escena `Menu`:

1. Se selecciono el objeto que controla el menu, llamado algo como `MenuManager`.
2. Se le agrego el script `Menu.cs`.
3. En el boton `Jugar`, en el componente `Button`, se agrego el evento `On Click`.
4. Se arrastro el objeto `MenuManager` al evento.
5. Se selecciono la funcion `Menu > Jugar`.
6. En el boton `Salir`, se hizo lo mismo pero seleccionando `Menu > Salir`.

### Bug que salio

El boton `Salir` si imprimia en consola, pero `Jugar` no cambiaba de escena. Esto pasaba porque el boton no tenia bien asignada la funcion del script, o porque la escena `Nivel1` no estaba incluida correctamente.

### Como se corrigio

Se conecto el boton `Jugar` con `Menu.Jugar()` y se verifico que `Nivel1` estuviera en Build Settings.

## 2. Fondo del menu con video

### Que se hizo

Se cambio la idea de usar una imagen fija en el menu por un video llamado `fondo_menu.mp4`.

Para esto se uso un objeto con el componente `Video Player`.

### Donde se configuro en Unity

En la escena `Menu`:

1. Se creo o selecciono un objeto para el fondo de video.
2. Se le agrego el componente `Video Player`.
3. En `Source`, se dejo como `Video Clip`.
4. En `Video Clip`, se asigno `fondo_menu.mp4`.
5. Se activo `Play On Awake`.
6. Se activo `Loop` para que el video se repita.
7. Se ajusto el modo de render para que el video se viera como fondo del menu.

### Bug que salio

El video ya estaba asignado, pero no se veia. Tambien aparecia una marca o parte no deseada del video.

### Como se corrigio

Se reviso el objeto del video en la escena y se ajusto su escala/posicion para que quedara como fondo. Para ocultar partes no deseadas del video, se hizo un efecto de recorte visual aumentando el zoom o escala del objeto, de forma que la zona no deseada quedara fuera del area visible de la camara.

## 3. Nave del jugador

### Que se hizo

Se agrego la nave del jugador usando la imagen `nave.png`. La nave se mueve a la izquierda y derecha.

El script usado fue:

- `Assets/Scripts/NaveMovimiento.cs`

La nave usa:

- `SpriteRenderer` para mostrar la imagen.
- `Rigidbody2D` para movimiento con fisicas.
- `BoxCollider2D` para detectar choques con la orbe.
- Tag `Nave` para que la orbe sepa que reboto contra la nave.

### Movimiento implementado

La nave se mueve con:

- Flecha izquierda
- Flecha derecha
- Tecla `A`
- Tecla `D`

El script limita la posicion horizontal para que la nave no se salga de la pantalla.

### Bugs que salieron

1. La nave se veia abajo de la imagen de fondo.
2. La nave no se movia.
3. En `Nivel1`, la nave se movia muy lento.
4. En `Nivel2` y `Nivel3`, al copiar configuraciones, hubo momentos donde no se veia nada.

### Como se corrigio

- Se ajusto el orden visual del `SpriteRenderer` para que la nave quedara encima del fondo.
- Se agrego o reviso el `Rigidbody2D`.
- Se conecto el script `NaveMovimiento.cs`.
- Se ajusto la velocidad de la nave.
- Se copio la misma configuracion a `Nivel2` y `Nivel3`.

## 4. Orbe / pelota

### Que se hizo

Se agrego la orbe que cae desde el centro y rebota contra:

- La nave.
- La pared izquierda.
- La pared derecha.
- El techo.
- Los asteroides.

La parte de abajo queda libre. Si la orbe cae por abajo, se pierde una vida.

El script usado fue:

- `Assets/Scripts/OrbeMovimiento.cs`

### Comportamiento

La orbe:

- Inicia desde una posicion central.
- Cae hacia abajo al empezar.
- Rebota en la nave con angulos distintos dependiendo de donde pegue.
- Si pega al centro de la nave, sale mas recta.
- Si pega a los lados de la nave, sale mas inclinada, como en Pong o Breakout.
- Cuando cae en la zona de muerte, se reinicia desde el centro si todavia quedan vidas.

### Bug que salio

Al principio la orbe no se veia, no caia o no rebotaba.

### Como se corrigio

Se reviso que tuviera:

- `SpriteRenderer`.
- `Rigidbody2D`.
- `CircleCollider2D`.
- Tag `Orbe`.
- El script `OrbeMovimiento.cs`.

Tambien se agregaron paredes y techo con colliders para que la orbe pudiera rebotar.

## 5. Paredes, techo y zona de muerte

### Que se hizo

Se configuraron los limites del nivel:

- Pared izquierda.
- Pared derecha.
- Techo.
- Zona de muerte abajo.

La zona de muerte detecta cuando cae la orbe.

El script usado fue:

- `Assets/Scripts/ZonaMuerte.cs`

### Comportamiento

Cuando la orbe entra a la zona de muerte:

1. Se imprime `Perdiste una vida` en la consola.
2. Se resta una vida en `GameSession`.
3. Si aun quedan vidas, la orbe se reinicia.
4. Si ya no quedan vidas, la orbe deja de reiniciarse y se muestra `Game Over`.

### Donde se configuro en Unity

En cada nivel:

1. Se crearon objetos invisibles para paredes y techo.
2. Se agregaron `BoxCollider2D`.
3. Para la zona de muerte, se activo `Is Trigger`.
4. Se agrego el script `ZonaMuerte.cs` a la zona de muerte.

## 6. Asteroides

### Que se hizo

Se agregaron asteroides en la parte superior del nivel. La orbe choca contra ellos, rebota y los destruye.

El script usado fue:

- `Assets/Scripts/Asteroide.cs`

### Como funcionan

El generador de asteroides crea una formacion automaticamente:

- Genera varios asteroides.
- Los coloca en posiciones aleatorias dentro de un area.
- Les da escalas diferentes para que no se vean tan alineados.
- Cuando la orbe choca con un asteroide, se destruye.
- Al destruirse, avisa a `GameSession` para contar cuantos quedan.

### Bug que salio

Los asteroides si chocaban, pero no se veia la imagen.

### Como se corrigio

Se reviso el `SpriteRenderer` del asteroide y se asigno correctamente la imagen del sprite. Tambien se ajusto la escala porque algunos sprites quedaban demasiado pequenos o fuera de vista.

### Asteroides aleatorios

Despues se cambio la formacion para que no estuvieran todos alineados. Se agrego generacion aleatoria con limites:

- `areaMinima`
- `areaMaxima`
- `escalaAleatoria`
- `distanciaMinima`

Esto hace que aparezcan mas naturales en la parte superior.

## 7. Sonido de explosion

### Que se hizo

Se agrego el sonido `explosion` cuando la orbe choca contra un asteroide.

### Donde se configuro

En el objeto generador de asteroides:

1. Se selecciono el objeto con `Asteroide.cs`.
2. En el Inspector, se asigno el audio `explosion` al campo `Sonido Explosion`.
3. Se ajusto el volumen con `Volumen Explosion`.

### Como funciona en codigo

Cuando la orbe choca contra el asteroide, se ejecuta:

```csharp
AudioSource.PlayClipAtPoint(sonidoExplosion, transform.position, volumenExplosion);
```

## 8. Nivel 2 con mas dificultad

### Que se hizo

Se hizo que `Nivel2` fuera similar a `Nivel1`, pero mas dificil:

- Mas asteroides.
- Asteroides especiales usando la imagen `asteroide_B_grande`.
- La orbe aumenta su velocidad al chocar contra asteroides.
- Los asteroides B aumentan mas la velocidad que los asteroides normales.

### Asteroides B

En `Nivel2` se agregaron 3 asteroides especiales. Estos usan:

- Sprite `asteroide_B_grande`.
- Mayor escala.
- Mayor incremento de velocidad.

### Bugs que salieron

1. Los asteroides B no se veian.
2. La velocidad de la orbe no cambiaba.
3. Despues la orbe iba demasiado rapido.
4. Al perder vida, la orbe mantenia la velocidad aumentada.

### Como se corrigio

- Se corrigio el sprite asignado de los asteroides B.
- Se hizo que `Asteroide.cs` regresara un incremento de velocidad distinto si era asteroide normal o especial.
- Se conecto `OrbeMovimiento.cs` con `Asteroide.cs` para que al chocar aumentara la velocidad.
- Se agrego velocidad maxima para que no se descontrolara.
- Se guardo la velocidad inicial de la orbe, y al reiniciarse por perder vida vuelve a esa velocidad base.

## 9. Tiempo, vidas y cambio de nivel

### Que se hizo

Se agrego un sistema global de partida con:

- Tiempo de partida arriba a la izquierda.
- Texto de vidas arriba a la izquierda.
- Vidas iniciales: `5`.
- Cambio automatico de `Nivel1` a `Nivel2`.
- Conservacion de vidas y tiempo entre niveles.

El script usado fue:

- `Assets/Scripts/GameSession.cs`

### Como funciona

`GameSession` se crea automaticamente cuando inicia el juego y no se destruye al cambiar de escena gracias a:

```csharp
DontDestroyOnLoad(gameObject);
```

Esto permite que:

- El tiempo no se reinicie.
- Las vidas no se reinicien.
- El contador de asteroides pueda decidir cuando pasar de nivel.

### UI agregada

Se crea un `Canvas` por codigo con dos textos:

- Estado arriba a la izquierda:

```text
Tiempo: 00:00
Vidas: 5
```

- Mensaje central:

```text
Has pasado al nivel 2
```

o

```text
Game Over
```

### Cambio de Nivel1 a Nivel2

Cuando todos los asteroides de `Nivel1` son destruidos:

1. `Asteroide.cs` avisa a `GameSession`.
2. `GameSession` baja el contador de asteroides activos.
3. Si ya no quedan asteroides en `Nivel1`, muestra `Has pasado al nivel 2`.
4. Espera 2 segundos.
5. Carga `Nivel2`.

### Bug que salio

Al darle `Jugar`, entraba a `Nivel1`, pero inmediatamente salia `Has pasado al nivel 2` y cambiaba de nivel.

### Por que pasaba

El contador de asteroides todavia estaba en `0` cuando el sistema revisaba si ya no quedaban asteroides. Como `0` parecia significar "ya no hay asteroides", el juego pensaba que el nivel ya estaba terminado.

### Como se corrigio

Se agrego una variable para saber si ya se registraron asteroides:

```csharp
private int asteroidesRegistrados;
```

Ahora el cambio de nivel solo ocurre si:

- Ya se registro al menos un asteroide.
- Los asteroides activos llegaron a `0`.
- La escena actual es `Nivel1`.

Tambien se cambio el registro para que los asteroides se cuenten desde que el generador los instancia, no hasta despues.

## 10. Build Settings

### Que se hizo

Se agregaron escenas al Build Settings para que `SceneManager.LoadScene` pueda cargarlas.

Escenas importantes:

- `Assets/Scenes/Menu.unity`
- `Assets/Scenes/Nivel1.unity`
- `Assets/Scenes/Nivel2.unity`

### Por que es importante

Si una escena no esta en Build Settings, Unity puede fallar al intentar cargarla con:

```csharp
SceneManager.LoadScene("NombreEscena");
```

## 11. Archivos principales modificados

### Scripts

- `Assets/Scripts/Menu.cs`
- `Assets/Scripts/NaveMovimiento.cs`
- `Assets/Scripts/OrbeMovimiento.cs`
- `Assets/Scripts/Asteroide.cs`
- `Assets/Scripts/ZonaMuerte.cs`
- `Assets/Scripts/GameSession.cs`
- `Assets/Scripts/FondoGalaxia.cs`
- `Assets/Scripts/PowerUp.cs`

### Escenas

- `Assets/Scenes/Menu.unity`
- `Assets/Scenes/Nivel1.unity`
- `Assets/Scenes/Nivel2.unity`
- `Assets/Scenes/Nivel3.unity`

### Configuracion del proyecto

- `ProjectSettings/EditorBuildSettings.asset`

## 12. Nivel 3 y mejoras generales

### Que se hizo

Se agrego `Nivel3` como continuacion de la dificultad del juego. Este nivel usa fondo morado, mas asteroides y nuevos tipos de obstaculos.

Cambios principales:

- `Nivel3` se agrego al Build Settings.
- El cambio de nivel ahora puede avanzar de `Nivel1` a `Nivel2` y de `Nivel2` a `Nivel3`.
- Al terminar `Nivel3`, se muestra una victoria final.
- Se agrego musica diferente para `Nivel3`.
- Se agregaron asteroides resistentes que necesitan mas de un golpe.

### Asteroides resistentes

Los asteroides resistentes usan el mismo script `Asteroide.cs`, pero cambian su tipo interno a `Resistente`.

La idea principal es:

```csharp
if (tipoAsteroide == TipoAsteroide.Resistente)
{
    golpesRestantes--;

    if (golpesRestantes > 0)
    {
        return;
    }
}
```

Esto significa que el asteroide no se destruye en el primer golpe. Primero baja su contador de golpes y solo desaparece cuando ya no le quedan golpes.

### Sistema de puntos

Se agrego un contador de puntos al `GameSession`. Cada asteroide da puntos cuando se destruye:

- Asteroide normal: `100` puntos.
- Asteroide especial: `250` puntos.
- Asteroide resistente: `400` puntos.

El metodo principal es:

```csharp
public void SumarPuntos(int cantidad)
{
    puntos += Mathf.Max(0, cantidad);
    ActualizarUI();
}
```

### Dash de la nave

Se agrego un dash simple para ayudar al jugador cuando esta lejos de la orbe.

Controles:

- `Shift izquierdo`
- `Espacio`

El dash solo funciona si el jugador se esta moviendo a la izquierda o derecha. Esto evita que se gaste cuando la nave esta quieta.

La funcion principal es:

```csharp
private void AplicarDash()
{
    if (Mathf.Approximately(movimiento, 0f))
    {
        return;
    }

    inicioDash = rb != null ? rb.position : (Vector2)transform.position;
    destinoDash = inicioDash;
    destinoDash.x += Mathf.Sign(movimiento) * distanciaDash;
    destinoDash.x = Mathf.Clamp(destinoDash.x, -limiteX, limiteX);
}
```

Tambien se agrego una estela visual con `TrailRenderer` para que no parezca que la nave se teletransporta.

### Fondo con particulas

Se agrego `FondoGalaxia.cs` para que los niveles tengan particulas de estrellas. La idea es dar la sensacion de que el jugador se esta adentrando en una galaxia.

Cada nivel conserva su color principal:

- `Nivel1`: azul.
- `Nivel2`: rojo.
- `Nivel3`: morado.

La velocidad aumenta por nivel para reforzar la sensacion de dificultad.

### Sonido y dano al perder vida

Cuando la orbe cae en la zona de muerte:

- Se reproduce un sonido de perdida.
- Se resta una vida.
- La nave se recentra.
- Aparece un flash rojo en pantalla como efecto de dano.

Esto esta en `ZonaMuerte.cs`.

## 13. Power-ups

### Que se hizo

Se agregaron power-ups que pueden caer cuando se destruye un asteroide. No salen siempre, porque si salieran demasiado seguido el juego perderia dificultad.

La probabilidad configurada es:

```csharp
[SerializeField] private float probabilidadPowerUp = 0.12f;
```

Esto quiere decir que hay aproximadamente 12% de probabilidad de que salga un power-up al destruir un asteroide.

### Power-up de nave grande

Este power-up agranda temporalmente la nave.

Sirve para que sea mas facil golpear la orbe cuando el jugador esta en una situacion dificil.

La funcion que se ejecuta en la nave es:

```csharp
public void ActivarNaveGrande(float duracion, float multiplicador)
{
    if (rutinaNaveGrande != null)
    {
        StopCoroutine(rutinaNaveGrande);
    }

    rutinaNaveGrande = StartCoroutine(NaveGrandeTemporal(duracion, multiplicador));
}
```

La nave vuelve a su tamano original despues de unos segundos.

### Power-up de explosion

Este power-up destruye asteroides cercanos a la orbe. Se hizo asi porque los asteroides normalmente estan cerca de la pelota, no cerca de la nave.

La parte importante es:

```csharp
GameObject orbe = GameObject.FindGameObjectWithTag("Orbe");

if (orbe != null)
{
    centroExplosion = orbe.transform.position;
}
```

Despues usa `Physics2D.OverlapCircleAll` para encontrar asteroides cercanos y destruirlos.

### Bug que salio

Al principio parecia que los power-ups no hacian nada.

### Por que pasaba

El power-up de explosion explotaba alrededor de la nave. Como la nave esta abajo y los asteroides estan arriba, casi nunca habia asteroides dentro del radio.

### Como se corrigio

Se cambio la explosion para que tome como centro la posicion de la orbe. Tambien se aumento el radio de explosion y se hizo mas evidente el efecto de nave grande.

## 14. Bugs importantes y soluciones

| Problema                                 | Causa                                                                            | Solucion                                                                   |
| ---------------------------------------- | -------------------------------------------------------------------------------- | -------------------------------------------------------------------------- |
| El boton Jugar no servia                 | No estaba bien conectado al metodo del script o faltaba escena en Build Settings | Se conecto `Menu.Jugar()` y se reviso `Nivel1`                             |
| El video del menu no se veia             | Configuracion del `Video Player` o escala/posicion incorrecta                    | Se asigno el clip y se ajusto el objeto de fondo                           |
| Se veia una parte no deseada del video   | El encuadre mostraba una zona del video que no queriamos                         | Se hizo zoom/escala para recortar visualmente                              |
| La nave salia detras del fondo           | Orden visual incorrecto                                                          | Se ajusto el `SpriteRenderer`                                              |
| La nave no se movia                      | Faltaba script, Rigidbody2D o configuracion de entrada                           | Se agrego `NaveMovimiento.cs` y componentes                                |
| La orbe no aparecia o no caia            | Faltaban componentes o tag                                                       | Se reviso `SpriteRenderer`, `Rigidbody2D`, `CircleCollider2D` y tag `Orbe` |
| Los asteroides chocaban pero no se veian | Sprite mal asignado o escala incorrecta                                          | Se asigno bien el sprite y se ajusto escala                                |
| La velocidad no aumentaba                | La orbe no estaba leyendo el incremento del asteroide                            | Se conecto `OrbeMovimiento.cs` con `Asteroide.cs`                          |
| La orbe iba demasiado rapido             | Incrementos muy altos                                                            | Se redujeron incrementos y se puso velocidad maxima                        |
| Al perder vida la orbe seguia rapida     | No se reiniciaba la velocidad base                                               | Se guardo velocidad inicial y se restauro al reiniciar                     |
| Nivel1 pasaba inmediatamente a Nivel2    | El contador de asteroides estaba en 0 antes de registrarlos                      | Se agrego `asteroidesRegistrados` y registro inmediato                     |
| Al llegar a 0 vidas seguia reiniciando   | La zona de muerte siempre reiniciaba la orbe                                     | `PerderVida()` ahora avisa si quedan vidas                                 |
| La orbe se quedaba horizontal            | La velocidad vertical podia quedar demasiado baja                                | Se agrego un minimo vertical en `OrbeMovimiento.cs`                        |
| El dash se gastaba parado                | El dash no revisaba si habia direccion                                           | Se agrego validacion de movimiento antes de usarlo                         |
| Los power-ups parecian no funcionar      | La explosion ocurria abajo, lejos de los asteroides                              | La explosion ahora se centra cerca de la orbe                              |

## 15. Estado actual

Actualmente el juego tiene:

- Menu funcional.
- Fondo de menu con video.
- Nave moviendose horizontalmente.
- Orbe con rebotes estilo Pong/Breakout.
- Paredes, techo y zona de muerte.
- Sistema de vidas.
- Sistema de tiempo.
- Asteroides normales.
- Asteroides especiales en Nivel2.
- Asteroides resistentes en Nivel3.
- Sonido de explosion al destruir asteroides.
- Cambio automatico de `Nivel1` a `Nivel2`.
- Cambio automatico de `Nivel2` a `Nivel3`.
- Victoria final al completar `Nivel3`.
- Conservacion de vidas y tiempo entre escenas.
- Sistema de puntos.
- Dash con recarga.
- Fondo con particulas de galaxia.
- Efecto de dano al perder vida.
- Power-up de nave grande.
- Power-up de explosion.

## 16. Pendientes posibles

Cosas que podrian agregarse despues:

- Balancear la probabilidad de power-ups.
- Agregar iconos o texto temporal para explicar que power-up se recogio.
- Agregar mas sonidos para rebote y pasar nivel.
- Mejorar el diseno visual del contador de tiempo y vidas.
- Pausa del juego.
- Nivel infinito con dificultad progresiva.

## 17. Pulido visual y mejoras recientes

### Que se hizo

Se agregaron mejoras de presentacion y jugabilidad para que el juego se sienta mas completo:

- Game Over y Victoria muestran puntos y tiempo final.
- Las vidas ahora se ven con iconos en lugar de solo un numero.
- El dash tiene una barra de recarga visual.
- Los power-ups muestran texto temporal en el centro cuando se recogen.
- El power-up de explosion muestra un aro visual donde ocurre la explosion.
- La orbe reproduce sonido de rebote.
- La hitbox de la orbe se ajusta al sprite para que no rebote antes de tocar el borde.
- Los asteroides tienen rotacion y pulso suave para que no se vean estaticos.
- Los asteroides especiales y resistentes se ajustaron de tamano y hitbox.

### Dash con barra

El indicador de dash sigue mostrando texto, pero ahora tambien tiene una barra que se llena mientras se recarga.

La idea es que el jugador pueda saber visualmente cuando el dash vuelve a estar listo.

### Vidas con iconos

Antes se mostraba:

```text
Vidas: 5
```

Ahora se dibujan iconos en la esquina superior izquierda. Esto hace que la interfaz se vea mas parecida a un videojuego.

### Hitbox de la orbe

La orbe rebotaba antes de tocar visualmente el borde porque su `CircleCollider2D` era mas grande que el sprite.

Se agrego un ajuste automatico:

```csharp
collider.radius = Mathf.Min(bounds.extents.x, bounds.extents.y) * margenHitbox;
```

Esto hace que el collider coincida mejor con la imagen visible.

### Animacion de asteroides

Los asteroides ahora rotan lentamente y tienen un pulso muy pequeno de escala. No se mueven de lugar para evitar que la dificultad cambie de forma injusta.

La animacion usa:

```csharp
transform.localRotation = Quaternion.Euler(0f, 0f, rotacionActual);
transform.localScale = escalaBase * pulso;
```

### Power-ups mas claros

Cuando se recoge un power-up aparece texto temporal:

- `Power-up: nave grande`
- `Power-up: explosion`

Ademas, la nave grande tiene una animacion de brillo y crecimiento, y la explosion muestra un aro visual.

### Backup recomendado

Antes de empezar cambios grandes, como un nivel infinito, se recomienda guardar una version estable en GitHub con:

- Una rama de backup.
- Un tag estable.

Asi se puede volver a esta version si los cambios nuevos no convencen.

## 18. Avances del dia: victoria final y ajustes de Nivel3

### Que se hizo

Se corrigio y completo el final del juego en `Nivel3`.

Antes, al terminar el Nivel3 podia pasar que no saliera la victoria, o que saliera la victoria pero la partida siguiera activa y la orbe siguiera cayendo, restando vidas aunque el jugador ya habia ganado.

Hoy se hicieron estos cambios:

- Se hizo mas robusta la deteccion de fin de nivel.
- Se confirmo por que a veces no salia victoria.
- Se ajusto el area de generacion de asteroides en `Nivel3`.
- Se corrigio la posicion de los asteroides especiales.
- Se detuvo la partida cuando aparece `Victoria final`.
- Se evito que la zona de muerte siga restando vidas despues de ganar.
- Se agrego sonido de victoria con `win.wav`.

### Bug: terminaba Nivel3 y no salia victoria

El problema era que el sistema dependia de un contador interno de asteroides. Si ese contador se desacomodaba, el jugador podia destruir visualmente todos los asteroides pero el juego podia pensar que todavia quedaba alguno.

Tambien vimos en Unity que en el `Hierarchy` todavia aparecian objetos como:

- `Asteroide_B_5`
- `Asteroide_B_6`

Eso significaba que no se habia terminado realmente el nivel, aunque pareciera que ya no habia asteroides visibles.

### Como se corrigio

En `GameSession.cs`, despues de destruir un asteroide, ahora el juego espera un frame y revisa directamente si queda algun objeto `Asteroide` activo en la escena.

Se agrego una revision con:

```csharp
Asteroide[] asteroides = FindObjectsByType<Asteroide>(FindObjectsSortMode.None);
```

La idea es:

1. Se destruye un asteroide.
2. El juego espera un frame para dejar que Unity termine de destruir el objeto.
3. Busca todos los `Asteroide` activos.
4. Si ya no queda ninguno:
   - En `Nivel1`, pasa a `Nivel2`.
   - En `Nivel2`, pasa a `Nivel3`.
   - En `Nivel3`, muestra `Victoria final`.

### Bug: asteroides se generaban en la esquina superior derecha

En `Nivel3`, algunos asteroides especiales aparecian arriba a la derecha, justo donde esta el HUD del dash.

Esto pasaba por dos razones:

- El area de generacion estaba muy grande hacia arriba y hacia la derecha.
- La formula de asteroides especiales estaba pensada para pocos asteroides especiales, pero en `Nivel3` hay mas.

### Donde se le pico en Unity

En la escena `Nivel3`:

1. Se selecciono `GeneradorAsteroides`.
2. En el Inspector se busco el componente `Asteroide`.
3. Se ajustaron los valores del area de generacion.

Antes estaba aproximadamente asi:

```text
Area Minima: X = -7.4, Y = 1.45
Area Maxima: X = 7.4, Y = 4.65
```

Ahora quedo asi:

```text
Area Minima: X = -7.1, Y = 1.35
Area Maxima: X = 6.1, Y = 4
```

Con esto los asteroides ya no se generan tan pegados al borde superior derecho.

### Cambio en Asteroide.cs

Tambien se corrigio la funcion que coloca asteroides especiales.

Antes, la posicion especial podia mandar asteroides demasiado a la derecha cuando habia mas de 3 especiales.

Ahora se distribuyen dentro del area segura usando un progreso de 0 a 1:

```csharp
float margenHorizontal = 0.8f;
float cantidad = Mathf.Max(1, cantidadEspeciales);
float progreso = (indice + 0.5f) / cantidad;
float x = Mathf.Lerp(areaMinima.x + margenHorizontal, areaMaxima.x - margenHorizontal, progreso);
float y = areaMaxima.y - 0.75f;
```

Esto hace que los asteroides especiales se repartan de forma ordenada, pero sin invadir la esquina del HUD.

### Bug: despues de ganar seguia perdiendo vidas

Cuando salia `Victoria final`, la orbe podia seguir moviendose. Si despues caia a la zona de muerte, el juego seguia restando vidas.

Eso se sentia mal porque el jugador ya habia ganado.

### Como se corrigio

En `GameSession.cs` se agrego una variable:

```csharp
private bool partidaTerminada;
```

Cuando aparece `Victoria final`:

```csharp
partidaTerminada = true;
```

Y en `PerderVida()` se revisa:

```csharp
if (partidaTerminada)
{
    return false;
}
```

Asi, si la partida ya termino, la zona de muerte ya no puede restar vidas.

### Cambio en ZonaMuerte.cs

En `ZonaMuerte.cs`, ahora si `PerderVida()` regresa `false`, ya no se hace nada mas.

Esto evita que despues de victoria o game over se siga reproduciendo:

- Sonido de perder vida.
- Flash de dano.
- Recentrar nave.
- Reiniciar orbe.

La logica quedo asi:

```csharp
bool quedanVidas = GameSession.Instancia.PerderVida();
OrbeMovimiento orbe = collision.GetComponent<OrbeMovimiento>();

if (!quedanVidas)
{
    return;
}
```

### Sonido de victoria

Se agrego sonido cuando el jugador gana.

El sonido usado fue:

```text
Assets/Sounds/win.wav
```

Como `GameSession` se crea por codigo, se copio tambien a:

```text
Assets/Resources/Sounds/win.wav
```

Esto permite cargarlo con:

```csharp
Resources.Load<AudioClip>("Sounds/win");
```

Y reproducirlo cuando aparece `Victoria final`:

```csharp
ReproducirSonidoFinal("Sounds/win");
```

### Archivos modificados hoy

- `Assets/Scripts/GameSession.cs`
- `Assets/Scripts/Asteroide.cs`
- `Assets/Scripts/ZonaMuerte.cs`
- `Assets/Scenes/Nivel3.unity`
- `Assets/Resources/Sounds/win.wav`

### Como probar que ya quedo

1. Abrir `Nivel3`.
2. Dar Play.
3. Destruir todos los asteroides.
4. Verificar que aparece `Victoria final`.
5. Verificar que suena `win.wav`.
6. Dejar que la orbe caiga despues de ganar.
7. Confirmar que ya no se restan vidas.
8. Confirmar que el tiempo final queda fijo.
9. Confirmar que aparecen los botones finales.

### Estado despues de estos cambios

Ahora el final del juego ya esta mas completo:

- Si se termina `Nivel3`, sale victoria.
- La victoria muestra resumen de puntos y tiempo.
- La victoria reproduce sonido.
- La partida se detiene al ganar.
- La zona de muerte ya no sigue afectando al jugador despues de ganar.
- Los asteroides de `Nivel3` ya no se generan encima del HUD.
