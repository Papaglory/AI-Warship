## Project Status
**Note:** This project is currently unfinished and may never be completed. It is being shared publicly for educational and reference purposes. With this in mind, please feel free to explore the available codebase and the most up-to-date Unity build.

## Description

This project features a battleship that autonomously navigates the map in search of enemy vessels. When an enemy vessel is encountered, the battleship engages in combat and collects the remains of the ship.

The decision-making process of the battleship is driven by utility AI, a system that uses mathematical functions and scoring systems to evaluate and prioritize actions based on their utility. This approach allows the battleship to make intelligent decisions in various scenarios, including combat and resource management.

## Preview

<img src="assets/preview-fire.gif" alt="Alt Text" width="600" height="350" />

The battleship fires using physics-based calculations to determine the correct firing angle to hit its targets.

<img src="assets/preview-search.gif" alt="Alt Text" width="600" height="350" />

Displayed is the search area of the battleship, highlighted as a green sphere. While patrolling, a small vessel enters this area and the battleship decides to engage it.

<img src="assets/preview-refuel.gif" alt="Alt Text" width="600" height="350" />

When the battleship's fuel gets low, the utility functions determine that it should seek a refueling station.

## Caution
Please note that the code in this project is poorly written and lacks sufficient comments. It has not been rewritten or refactored to adhere to standard coding practices. Therefore, we recommend using the concepts and ideas presented rather than relying on the code itself.

## Dependencies
Uses the Unity Engine (version 2022.3.29f1) along with the following assets (for more information, please refer to the acknowledgements file):
* Foam Textures by A Dog's Life Software.
* Fire Sound from Freesound.org (the audioclip seems to have been taken down).

## Author
Marius H. Naasen, originally created August 2018.
