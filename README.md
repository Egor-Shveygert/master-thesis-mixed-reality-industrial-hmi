# VuforiaApp

Unity aplikacia s Vuforia AR castou, MQTT komunikaciou a Mixed Reality Toolkit/XR komponentmi.

## Verzia Unity

Projekt bol vytvoreny/otvarany v:

```text
Unity 2022.3.55f1
```

Odporucane je otvarat projekt v rovnakej LTS verzii cez Unity Hub.

## Co patri do Gitu

Do repozitara patria hlavne tieto priecinky a subory:

```text
Assets/
Packages/
ProjectSettings/
README.md
.gitignore
```

Tieto priecinky obsahuju zdrojove assety, sceny, skripty, package konfiguraciu a nastavenia projektu. Unity si po otvoreni projektu znova vygeneruje cache a lokalne build subory.

## Co do Gitu nepatri

Do repozitara necommitovat:

```text
Library/
Temp/
Obj/
obj/
Logs/
.vs/
*.csproj
*.sln
Build/
Builds/
Release_build_v2/
QCAR/
```

Tieto subory su bud lokalne generovane Unity/Visual Studiom, alebo su vystup z buildu. Prave tieto casti zvyknu nafuknut projekt na mnoho GB.

## Otvorenie projektu po clone

1. Naklonuj repozitar.
2. Otvor priecinok projektu v Unity Hub.
3. Pouzi Unity `2022.3.55f1`.
4. Pockaj, kym Unity znovu vytvori `Library/` a naimportuje packages.
5. Otvor scenu z `Assets/Scenes/`, napriklad `FullApp.unity` alebo inu pracovnu scenu.

Prve otvorenie po clone moze trvat dlhsie, pretoze Unity importuje assety odznova.

## Velke assety

Ak sa v projekte nachadzaju velke binarne subory, napriklad modely, videa, textury alebo Vuforia databazy nad desiatky MB, je vhodne pouzit Git LFS.

Typicke kandidatky:

```text
*.fbx
*.blend
*.psd
*.png
*.jpg
*.tga
*.wav
*.mp4
*.dat
```

Vuforia databazy, ktore su potrebne pre beh aplikacie a nachadzaju sa v `Assets/StreamingAssets/Vuforia/`, maju ostat sucastou projektu. Ak by boli prilis velke, riesit ich cez Git LFS, nie cez `.gitignore`.

## Poznamky

- `Library/` sa neuploaduje. Unity ho vytvori automaticky.
- `Release_build_v2/` je build vystup, nie zdroj projektu.
- `Assets/StreamingAssets/Vuforia/` obsahuje Vuforia data potrebne pre aplikaciu.
- Po zmene Unity verzie je dobre commitnut aj zmeny v `ProjectSettings/ProjectVersion.txt`.
