# Siege Optimization Simulator

This project simulates siege strategies using Evolutionary Algorithms within the Unity engine.

## Downloads & Execution Modes

You can download the full project source code and pre-built executable files from GitHub:

**[Download Project Files Here](https://github.com/VladP1skovy1/siege-simulator)**  
There are two ways to run this simulator:

### Option 1: Quick Start (Pre-built Executables)
If you want to view the results immediately without installing Unity, simply run the included `.exe` files. Each executable comes with **presets** for specific algorithms:
* **Random Army**: Demonstrates a baseline random strategy.
* **Local Search**: Shows the result of the Local Search algorithm.
* **Evolutionary Algorithm**: Runs the standard Evolutionary Algorithm.
* **Memetic Algorithm**: Runs the advanced Memetic Algorithm.

### Option 2: Full Customization (Unity Project)
If you want to **design your own castles** or tweak specific parameters (warrior stats, eoa parameters), you must open the source code in the Unity Editor. Follow the instructions below.

---

## Prerequisites (For Full Customization)

Before opening the source code, ensure you have the following installed:

1.  **Unity Hub**: Download and install it from the [official Unity website](https://unity.com/download).
2.  **Unity Editor**: Inside Unity Hub, download and install the required Unity version for **Windows**: **6000.3.1f1**.

## Installation

1.  **Unzip the Project**: Extract the downloaded archive (from the GitHub link above) to a folder on your computer.
2.  **Add to Unity Hub**:
    * Open Unity Hub.
    * Click on the **"Projects"** tab.
    * Click the **"Add"** button.
    * Navigate to the unzipped folder containing the source code and select it.
3.  **Open the Project**: Click on the project name in Unity Hub to launch the Unity Editor.

## Configuration & Usage

Once the project is open in Unity, follow these steps to set up your custom simulation:

### 1. Castle Setup
* Navigate to the **Scene View**.
* You can manually modify the castle layout by moving, adding, or removing building objects.
* **Important:** Please ensure all buildings are aligned perfectly with the **grid cells** (tiles) for the logic to work correctly.

### 2. Register Buildings
* After modifying the castle, you must update the system.
* Select the **`BuildingSystem`** GameObject in the Hierarchy.
* In the **Inspector** window, update the list of used buildings to match your current scene layout.

### 3. Algorithm & Warrior Settings
* Select the **`GameManager`** GameObject in the Hierarchy.
* In the **Inspector**, you can:
    * Choose which **Evolutionary Algorithm** to use for the simulation.
    * Adjust the **Warrior Parameters** (Speed, Health, Damage, etc.) to test different scenarios.

## Running the Simulation

1.  Click the **Play** button at the top of the Unity Editor.
2.  In the **Game View** (simulation UI), click the **"Start"** button.
3.  Wait for the algorithm to compute the best strategy. The visual simulation will begin automatically once the calculation is complete.

---
*Created by Vladyslav Piskovyi*
