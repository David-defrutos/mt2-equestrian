# mt2-equestrian

Traspaso del clan **Equestrian** de Monster Train 1 a **Monster Train 2**, sobre
Trainworks Reloaded. Fork personal, en JSON, **sin codigo C# propio**.

Obra original: [Equestrian Clan](https://github.com/ThreeFishies/Equestrian-Clan) de
ThreeFishies. Ver `NOTICE.md`.

## Estado

Esqueleto. La clase carga, los dos campeones existen con **sendas de relleno** y hay
**dos cartas de prueba** para cerrar el ciclo entero antes de escribir las otras 44.

| bloque | hecho | total previsto |
|---|---|---|
| clase, subtipos, pools | si | — |
| campeones | 2 (sendas de relleno) | 2 |
| unidades | 1 (Trash Panda) | 18 |
| hechizos | 1 (Apple Cider) | 27 |
| reliquias | 0 | 13 |
| potenciadores | 0 | 3 |

El mapeo carta a carta esta en `D:\Juegos\MT2_mod\docs\77-port-equestrian.md`.

## Estructura

```
David-Equestrian_Custom\
  src\           Plugin.cs (la lista de rutas JSON), csproj, nuget.config
  json\          los datos del clan
  textures\      el arte, reutilizado del mod de MT1
  manifest.json  para el gestor de mods
```

## Decisiones de traspaso

1. Sin C#. Los 26 efectos de carta, 13 traits y 11 target modes que usa el mod original
   existen ya en MT2; no hace falta escribir ninguno.
2. `social` **no** puede ser un estado propio (un estado nuevo exige una clase C#), asi
   que pasa a ser el **subtipo** `Sub_Social`. El escalado "cuantos mas aliados, mejor"
   lo hace el tracked value `NumUnitsInTargetRoom` de Conductor.
3. El trait `Herd` (requisito de N aliados para poder jugar) no tiene equivalente: la
   puerta se convierte en escalado.
4. Fuera: el evento de caverna en Ink, la UI propia, la compatibilidad con Arcadian y los
   parches Harmony de MT1.
