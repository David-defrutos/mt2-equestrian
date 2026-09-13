# Como compilar el DLL de este clan

> Recordatorio de la regla que cuesta una tarde: **el DLL lleva dentro la lista de
> ficheros JSON que carga**. Un JSON nuevo no existe para el juego hasta que se anade a
> `src\Plugin.cs` y se recompila. Detalle en `docs\64-anadir-json-nuevos-y-recompilar.md`.

## Por GitHub Actions (lo normal)

1. Anadir la ruta del JSON nuevo a la lista de `AddMergedJsonFile` en `src\Plugin.cs`.
2. Commit y push a `main`. El workflow salta solo cuando se toca `src\`.
3. En la pestana Actions, descargar el artefacto `mt2_equestrian.Plugin`.
4. Copiar `mt2_equestrian.Plugin.dll` a la **raiz de esta carpeta**, sustituyendo el viejo.

## En local (si hace falta)

```powershell
cd "C:\Users\david\AppData\Roaming\Thunderstore Mod Manager\DataFolder\MonsterTrain2\profiles\Default\BepInEx\plugins\David-Equestrian_Custom"
dotnet build .\src -c Release --output D:\Juegos\MT2_mod\_dll-build\out
Copy-Item D:\Juegos\MT2_mod\_dll-build\out\mt2_equestrian.Plugin.dll .\ -Force
```

**Nunca** compilar con la salida dentro de esta carpeta: BepInEx escanea `plugins\` en
profundidad y cargaria el plugin dos veces.

## Desactivar este clan sin desinstalarlo

Renombrar el DLL, no la carpeta (BepInEx rastrea recursivo):

```powershell
Rename-Item .\mt2_equestrian.Plugin.dll mt2_equestrian.Plugin.dll.off
```

## Antes de arrancar el juego

```powershell
D:\Juegos\MT2_mod\scripts\validate-mt2-mods.ps1
```
