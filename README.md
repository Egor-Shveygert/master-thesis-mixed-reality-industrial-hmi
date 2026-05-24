<!-- # VuforiaApp

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
- Po zmene Unity verzie je dobre commitnut aj zmeny v `ProjectSettings/ProjectVersion.txt`. -->


# Master Thesis — Mixed Reality Industrial HMI

AR aplikácia pre Microsoft HoloLens 2 určená na rozšírené ovládanie a monitorovanie laboratórnej výrobnej linky fischertechnik. Aplikácia rozpoznáva 3D identifikátory (kocky) umiestnené na linke pomocou Vuforia Engine, a po úspešnej detekcii zobrazí používateľovi v zornom poli stavové a ovládacie panely výrobnej linky. Komunikácia s PLC RevPi Connect+ (CODESYS) je riešená cez Node-RED a MQTT broker HiveMQ.

Repozitár obsahuje zdrojové súbory Unity projektu vytvoreného v Unity 2022.3.55f1 s MRTK3 a Vuforia Engine. Kompletný používateľský návod na inštaláciu prostredia, build a deploy na HoloLens 2 sa nachádza v Dodatku C diplomovej práce. Tento dokument popisuje skriptovú časť aplikácie a slúži ako rýchla orientácia v repozitári.

---

## Štruktúra repozitára
├── Assets/              # Unity assety - skripty, scény, prefaby, materiály, Vuforia databázy
├── Packages/            # Unity Package Manager manifest a lokálne MRTK3/Graphics Tools balíky
├── ProjectSettings/     # Konfigurácia Unity projektu (XR plugin, OpenXR, MRTK3, Player)
├── UserSettings/        # Lokálne nastavenia editora
├── .gitignore
└── README.md

Priečinky `Library/`, `Temp/`, `Obj/`, `Logs/` a `Build/` sa do repozitára necommit-ujú — Unity ich pri prvom otvorení projektu vygeneruje automaticky.

---

## Použité technológie a verzie

- **Unity 2022.3.55f1** (LTS, ARM64 build pre UWP, posledná podporovaná vetva s plnou podporou HoloLens 2)
- **MRTK3** (Mixed Reality Toolkit 3) — interakčné prvky, MRTK XR Rig, Hand Menu, Slate panely
- **Mixed Reality OpenXR Plugin** + Unity OpenXR Plugin
- **Vuforia Engine** — detekcia 3D identifikátorov (Advanced Model Targets)
- **M2MQTT for Unity** — MQTT klient
- **TextMeshPro** — texty v UI

Cieľová platforma: Universal Windows Platform, architektúra ARM64, Microsoft HoloLens 2.

---

## Skriptová časť aplikácie

Všetky vlastné C# skripty sa nachádzajú v priečinku `Assets/Scripts/`. Skripty sú navrhnuté tak, aby boli funkčne oddelené — detekcia 3D identifikátorov, riadenie aplikačného stavu, diagnostika a MQTT komunikácia tvoria samostatné celky, ktoré sa medzi sebou prepájajú cez referencie v Unity Inspectore a cez C# eventy.

### Prehľad skriptov

| Skript | Úloha |
|---|---|
| `AMTargetAppStateController.cs` | Centrálny manažér aplikácie. Riadi stavový automat (Detection OFF → Detection ON → Detection Successful → UI Mode), prepína viditeľnosť vrstiev UI a prijíma informácie o detekcii od loggerov. |
| `DetectedModelLogger.cs` | Pripája sa na Vuforia event `OnTargetStatusChanged` jedného Model Targetu. Filtruje udalosti príznakom `detectionEnabled` a podstatné zmeny forwarduje centrálnemu manažérovi. |
| `VuforiaStatusDisplay.cs` | Diagnostický skript. Zobrazuje aktuálny `Status`, `StatusInfo` a platnosť pózy daného Model Targetu — využíva sa pri vývoji a ladení rozpoznávania. |
| `PlcMqttClient.cs` | Rozšírenie `M2MqttUnityClient`. Pripája sa k MQTT brokeru, odoberá zoznam topicov, ukladá poslednú prijatú hodnotu pre každý topic a vystavuje event `MessageArrived` ostatným skriptom. Poskytuje aj metódu na publikovanie príkazov späť do brokera. |
| `MqttToDialog.cs` | Pripájač jedného UI panelu na jeden MQTT topic. Pri prijatí správy aktualizuje text aj farbu hodnoty (zelená pre `true`, červená pre `false`). |

Zdrojové výpisy všetkých piatich skriptov sú zaradené aj v Dodatku B diplomovej práce.

### Tok dát a interakcie medzi skriptmi

**Smer PLC → AR (čítanie stavov):**
1. Node-RED publikuje hodnoty PLC premenných do MQTT topicov v tvare `hololens/status/<premenna>`.
2. `PlcMqttClient` na týchto topicoch prijme správu a vyvolá event `MessageArrived(topic, payload)`.
3. Každý UI panel má pripojený skript `MqttToDialog`, ktorý počúva tento event, porovná topic s vlastnou konfiguráciou a ak sa zhoduje, aktualizuje text a farbu hodnoty.

**Smer AR → PLC (odoslanie príkazu):**
1. Používateľ v AR rozhraní zmení hodnotu ovládacieho prvku (napr. AutoModeActive).
2. UI element zavolá publikovaciu metódu na `PlcMqttClient`, ktorá odošle správu do topicu `hololens/cmd/<premenna>/set`.
3. Node-RED príkaz prijme a cez OPC UA ho zapíše do PLC programu v CODESYS.

**Detekcia 3D identifikátora:**
1. Vuforia oznámi zmenu stavu cez event `OnTargetStatusChanged` na príslušnom `ObserverBehaviour`.
2. `DetectedModelLogger` event prijme, skontroluje príznak povoľujúci reagovanie, a forwarduje informáciu centrálnemu manažérovi.
3. `AMTargetAppStateController` vyhodnotí, či ide o úspešnú detekciu (stav TRACKED) a podľa stavového automatu prepne UI vrstvy.

### MQTT topiky

Konvencia topicov je opísaná v kapitole 10.5 DP:

- `hololens/status/<premenna>` — stavové hodnoty z PLC publikované pre AR aplikáciu (napr. `LED1_start`, `ConvLeft`, `PunchUp`, `PartAtPunch`, `CycleActive`, `AutoModeActive`).
- `hololens/cmd/<premenna>/set` — ovládacie príkazy z AR aplikácie smerujúce späť do PLC (napr. `hololens/cmd/AutoModeActive/set`).

Zoznam odoberaných topicov sa konfiguruje v Inspectore na objekte `MQTT_Manager` v poli `Topics` skriptu `PlcMqttClient`. Pre pridanie novej hodnoty do AR rozhrania stačí pridať topic do tohto zoznamu a vytvoriť UI panel s pripojeným skriptom `MqttToDialog`, ktorý daný topic odoberá — centrálnu logiku meniť netreba.

### Použitý MQTT broker

Pre demonštračné účely je použitý verejný broker `broker.hivemq.com` na porte `1883` bez autentifikácie. Adresa a port sa nastavujú priamo v Inspectore na komponente `PlcMqttClient`. Pre reálne nasadenie je potrebné použiť privátny broker so zabezpečeným spojením a riadením prístupových práv.

---

## Spustenie projektu

Tento dokument obsahuje len rýchly prehľad. Detailný postup s obrázkami je v Dodatku C diplomovej práce (kapitoly C.3 až C.14).

### Predpoklady

- Visual Studio 2022 s workloadmi: .NET desktop development, Desktop development with C++, Universal Windows Platform development, Game development with Unity (vrátane Windows 10/11 SDK a USB Device Connectivity).
- Unity Hub a Unity 2022.3.55f1 s modulmi Universal Windows Platform Build Support a Windows Build Support (IL2CPP).
- Microsoft HoloLens 2 (pre Holographic Remoting aj samotná aplikácia Holographic Remoting Player).

### Klonovanie a otvorenie

```bash
git clone https://github.com/Egor-Shveygert/master-thesis-mixed-reality-industrial-hmi.git
```

V Unity Hub-e cez **Add → Add project from disk** vybrať koreňový priečinok klonu. Pri prvom otvorení Unity dogeneruje priečinok `Library/`, čo môže trvať niekoľko minút.

### Doplnenie balíkov, ktoré nie sú v repozitári

Niektoré balíky sa pre svoju veľkosť alebo licenčné podmienky do repozitára necommit-ujú a je potrebné ich doplniť ručne podľa postupu v DP:

- **Mixed Reality OpenXR Plugin** — cez nástroj Mixed Reality Feature Tool (DP, C.6).
- **MRTK3 balíky** + **Graphics Tools for Unity** — stiahnuť `.tgz` súbory z oficiálnych GitHub releases a importovať cez Package Manager → *Add package from tarball* v poradí: graphicstools → core → uxcore → standardassets → spatialmanipulation → uxcomponents → input (DP, C.7).
- **Vuforia Engine** — z oficiálneho zdroja Vuforia. Licenčný kľúč sa nastavuje vo Vuforia Configuration (DP, podkapitola 8.4.1).
- **M2MQTT for Unity** — knižnica pre MQTT klienta.

Po importe balíkov skontrolovať Project Settings → MRTK3 a XR Plug-in Management podľa DP kapitol C.8 a C.9.

### Build a deploy

Pre Holographic Remoting (rýchle testovanie bez buildu) postupovať podľa kapitoly C.11 DP. Pre plný build a deploy na HoloLens 2 cez Visual Studio postupovať podľa kapitol C.12 až C.14 DP.

---

## Konfigurácia v Unity Inspectore

Hlavné objekty scény, na ktorých sú pripojené vlastné skripty:

- **MRTK XR Rig → Main Camera** — pripojený komponent Vuforia Behaviour s licenčným kľúčom a databázami.
- **AppStateManager** — pripojený `AMTargetAppStateController`. V Inspectore sú priradené tri ModelTarget objekty, ich `DetectedModelLogger` komponenty, koreň Symbolic Guide View, UI panely pre jednotlivé identifikátory a textové polia hlavného stavového panela.
- **ModelTarget_02 / ModelTarget_04 / ModelTarget_05** — každý má pripojený `DetectedModelLogger` a `VuforiaStatusDisplay`. V `DetectedModelLogger` je priradená referencia na `AMTargetAppStateController`.
- **MQTT_Manager** — pripojený `PlcMqttClient`. V Inspectore je adresa brokera, port a zoznam odoberaných topicov.
- **UI panely premenných** — každý má pripojený `MqttToDialog` s referenciou na `MQTT_Manager`, na textové pole hodnoty a s vyplneným MQTT topicom.

---

## Odkazy

- Diplomová práca a kompletný používateľský návod: Dodatok C
- Zdrojové výpisy skriptov v texte práce: Dodatok B
- Komunikačná pipeline (CODESYS → OPC UA → Node-RED → MQTT → Unity): kapitola 10 DP

---

## Autor

Egor Shveygert  
Fakulta elektrotechniky a informatiky STU v Bratislave