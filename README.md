# Siege Optimization Simulator

This project simulates siege strategies using Evolutionary Algorithms within the Unity engine.

## Downloads & Execution Modes

**To get started, please visit the GitHub repository:**

👉 **[Go to GitHub Repository](https://github.com/VladP1skovy1/siege-simulator)** 👈

From there, you have two options depending on your needs:

1.  **For Quick Start**: Download only the **`Ready`** folder. This folder contains the pre-built executable files that run without installing Unity.
2.  **For Full Customization**: Download the **entire repository** (or clone it). This allows you to add the project to Unity Hub and modify the source code.

---

### Option 1: Quick Start (Pre-built Executables)
If you downloaded the **`Ready`** folder, you can view the results immediately. Inside, you will find separate folders for each algorithm.

To run a specific simulation:
1.  Open the folder corresponding to the algorithm you want to test (e.g., **Random**, **Local Search**, **Evolutionary Algorithm**, or **Memetic Algorithm**).
2.  Inside that folder, launch the **`My project.exe`** file.

Each folder contains a version of the simulator pre-configured with that specific algorithm. The default configurations for these presets are:

* **Global Constraint**: All algorithms have a **Max Army Cost** of **1010 gold**.
* **Local Search**: Configured for **5000 epochs**.
* **Evolutionary Algorithm**: Configured for **100 epochs** with a **population size of 50**.
* **Memetic Algorithm**: Configured for **50 epochs** with a **population size of 50**. It utilizes Local Search (5 epochs) with a **20% probability** and improves the initial population.

---

### Option 2: Full Customization (Unity Project)
If you downloaded the **full project**, you can **design your own castles** or tweak specific parameters (warrior stats, EOA parameters). You must open the source code in the Unity Editor.

## Prerequisites (For Full Customization)

Before opening the source code, ensure you have the following installed:

1.  **Unity Hub**: Download and install it from the [official Unity website](https://unity.com/download).
2.  **Unity Editor**: Inside Unity Hub, download and install the required Unity version for **Windows**: **6000.3.1f1**.

## Installation

1.  **Unzip the Project**: Extract the downloaded archive to a folder on your computer.
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