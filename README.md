# CustomRetoolFilter

**CustomRetoolFilter** é unha pequena ferramenta de consola pensada para filtrar ficheiros `.dat` de **No-Intro** e **Redump** con Retool (https://unexpectedpanda.github.io/retool/).

O obxectivo é facer algo menos restritivo que o filtrado **1G1R** ("one game, one rom").
A idea naceu de que eu quería conservar **dúas versións** de cada xogo cando están dispoñibles —normalmente a **USA** e a **Europe** (ou **Spain**, se existe)— en vez de quedar cunha soa.

---

## Que fai o programa

- Mostra un **menú de texto** no terminal con varias opcións.
- A función principal permite **filtrar un ficheiro `.dat`** usando Retool.
- O proceso consiste en:
  1. Realizar un primeiro filtrado estilo *1G1R*.
  2. Revisar o dat de descartes e **recuperar certas versións adicionais**.
  3. Engadir esas versións á selección do primeiro filtrado, acadando así un resultado estilo **"1G2R"** (*one game, two roms*).

O resultado é un ficheiro `.dat` filtrado de maneira lixeiramente menos restritiva que co estilo *1G1R*.

---

## Estado actual

Este proxecto está **en desenvolvemento (WIP)** e pode non funcionar correctamente.
Actualmente pide un `.dat` concreto para filtralo, pero quero engadir algunhas opcións máis no futuro, como:

- Escoller entre modo **1G1R** e **1G2R**.
- Indicar **un cartafol completo** con varios `.dat` para procesalos todos dunha tacada.

---

## Tecnoloxías empregadas

- **C# (.NET)**
- **Retool CLI**

---

## Licenza

Este proxecto publícase baixo a licenza **MIT**.
Podes usalo, modificalo e adaptalo libremente baixo a túa propia responsabilidade.

---

# CustomRetoolFilter (English)

**CustomRetoolFilter** is a small **C#** console tool designed to filter **No-Intro** and **Redump** `.dat` files using **Retool** (https://unexpectedpanda.github.io/retool/).

The goal is to provide a less restrictive alternative to the standard **1G1R** (“one game, one rom”) filtering.
The idea came from wanting to keep **two versions** of each game when available — usually **USA** and **Europe** (or **Spain**, if it exists) — instead of just one.

---

## What the program does

- Displays a **text-based menu** in the terminal with several options.
- The main feature allows you to **filter a `.dat` file** using Retool.
- The process works as follows:
  1. Perform an initial *1G1R*-style filtering pass.
  2. Review the discard `.dat` and **recover specific additional versions**.
  3. Add those recovered entries back into the filtered result, achieving a **"1G2R"** (*one game, two roms*) style output.

The result is a `.dat` file filtered in a slightly less restrictive way than standard *1G1R* mode.

---

## Current status

This project is **a work in progress (WIP)** and may not work perfectly yet.
At the moment, it takes a single `.dat` file to process, but future plans include:

- Allow switching between **1G1R** and **1G2R** modes.
- Allow selecting **a folder of `.dat` files** to process in batch.

---

## Technologies used

- **C# (.NET)**
- **Retool CLI**

---

## License

This project is released under the **MIT License**.
You’re free to use, modify, and adapt it at your own responsibility.
