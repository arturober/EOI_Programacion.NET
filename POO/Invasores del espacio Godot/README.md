# Invasores del espacio Godot

Versión simplificada del juego de consola adaptado a Godot 4.5 .NET.

## Objetivo de esta versión

El proyecto mantiene la lógica original por turnos, pero utiliza una representación gráfica deliberadamente sencilla para que `Juego.cs` sea fácil de leer y modificar a mano.

- La nave es un triángulo azul.
- Cada alien es un rectángulo verde con dos ojos.
- Cada bala es un rectángulo amarillo.
- El escenario es una cuadrícula de 12 x 12.
- No hay estrellas, paneles laterales, efectos ni dibujos complejos.
- No se utilizan imágenes, sonidos, fuentes ni otros recursos externos.

## Requisitos

- Godot 4.5 con soporte para .NET.
- .NET SDK 8.0 o posterior.

## Ejecución

1. Descomprime la carpeta.
2. Abre `project.godot` con la edición .NET de Godot.
3. Espera a que Godot restaure y compile el proyecto C#.
4. Pulsa F6 o F5.

## Controles

- Flecha izquierda: mover la nave una casilla a la izquierda.
- Flecha derecha: mover la nave una casilla a la derecha.
- Espacio: disparar.
- X o Escape: salir.
- R o Intro tras perder: comenzar una partida nueva.

## Archivos principales

- `Entidades.cs`: clases `Entidad`, `Nave`, `Alien` y `Bala`.
- `EntradaGodot.cs`: convierte las teclas en acciones del juego.
- `GestorRecord.cs`: carga y guarda el récord.
- `Juego.cs`: lógica de la partida y representación gráfica simplificada.
- `Juego.tscn`: escena mínima con un único nodo `Node2D`.

## Comentarios del código

Todos los archivos C# incluyen comentarios formativos y detallados. Los comentarios explican:

- La responsabilidad de cada clase.
- El motivo de las constantes, campos y colecciones.
- El ciclo de vida de la escena de Godot.
- La lectura no bloqueante del teclado.
- El procesamiento de turnos y la dificultad progresiva.
- La detección de impactos y la eliminación segura de elementos.
- La conversión de coordenadas lógicas a píxeles.
- El dibujo de la interfaz y de cada entidad mediante primitivas gráficas.
- El almacenamiento persistente del récord mediante `user://record.txt`.

Los comentarios utilizan `//` normales y no documentación XML, con el objetivo de que resulten fáciles de seguir para alumnado que está aprendiendo C# y programación orientada a objetos.
