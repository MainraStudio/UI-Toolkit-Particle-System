# UIToolkit ParticleSystem

[![GitHub all releases](https://img.shields.io/github/downloads/MainraStudio/UI-Toolkit-Particle-System/total)](https://github.com/SeaeeesSan/SimpleFolderIcon/releases)
[![GitHub license](https://img.shields.io/github/license/MainraStudio/UI-Toolkit-Particle-System)](https://github.com/SeaeeesSan/SimpleFolderIcon/blob/master/LICENSE)
[![openupm](https://img.shields.io/npm/v/com.mainragames.uitoolkitparticlesystem?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.mainragames.uitoolkitparticlesystem/)

## Features

**UIToolkit ParticleSystem** enables you to create ParticleSystem in the Unity UIBuilder.

## Requirements
| **Name** | **Version** |
| --- | --- |
| Unity | `>= 2021.2` |
| [Alchemy](https://github.com/annulusgames/Alchemy) | `>= 2.1.0` |

## Installation
- **Using Open UPM**
  <details>
  <summary>Install via Package Manager</summary>
    
    - open **`Edit/Project Settings/Package Manager`**
    - add a new Scoped Registry (or edit the existing OpenUPM entry)

      | | |
      | --- | --- |
      | **Name** | `package.openupm.com` |
      | **URL** | `https://package.openupm.com` |
      | **Scope(s)** | `com.annulusgames.alchemy` |
      | | `com.mainragames.uitoolkit-particlesystem` |
    - click **`Apply`**
    - open **`Window/Package Management/Package Manager`**
    - click **`+`**
    - select **`Install package by name...`**
    - paste **`com.mainragames.uitoolkit-particlesystem`** into **Name**
    - click **`Install`**
      ## <sub>                                                                               OR</sub>
    - Alternatively, merge the snippet to [Packages/manifest.json](https://docs.unity3d.com/Manual/upm-manifestPrj.html)
      
      ```json
      {
          "scopedRegistries": [
              {
                  "name": "package.openupm.com",
                  "url": "https://package.openupm.com",
                  "scopes": [
                      "com.annulusgames.alchemy",
                      "com.mainragames.uitoolkit-particlesystem"
                  ]
              }
          ],
          "dependencies": {
              "com.mainragames.uitoolkit-particlesystem": "1.0.0"
          }
      }
      ```
  </details>
  <details>
  <summary>Install via command-line</summary>
    
    ```console
    $ openupm add com.mainragames.uitoolkit-particlesystem
    ```
- **Using Open Git URL**
