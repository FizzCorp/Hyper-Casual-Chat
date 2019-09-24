## Introduction
This repository contains the Fizz UI made in UGUI Unity3d. The UI implements a basic multi-linugal chat system with profanity filtering.

## Instructions
1. Clone the repository.
2. Open the 'SceneSelector' scene under the 'FizzUI/Samples/' directory.
3. Hit run and navigate to hyper casual chat sample.
4. Change userId and Name if you want.
5. Select a language from the language dropdown.
6. Click Connect, then hit Launch after connecting.

## Requirements
The Fizz client requires the .NET 4.x runtime to run. Therefore the sample is not compatible with Unity versions that do not support this new runtime.

## Supported Platforms
* Mac OS X
* Windows
* Android
* iOS

## Concept

### FizzService
FizzService is an intermediate class(MonoBehaviour, DontDestroyOnLoad) designed to work like bridge between FizzClient and FizzUI. It contains an instance of FizzClient which can be opened and closed according to game client need. It's also used to Subscribe and Unsubsribe channels when client is opened.

### FizzChatView
FizzChatView is the core UI compoment which contains channel list, messages and input. Channels are added and removed from it in order to show or hide them from view. But note that a channel should be Subsribed first by using FizzService. It can be configured to show/hide channel list and input. It can be added to any container like, Tabs and Popup etc. 

> Note: FizzChatView is designed with reference resolution of 750x1334.

Goto Wiki for more details.