# **Kingmaker — Game Design Document (GDD)**

## **1\. Game Overview**

**Working Title:** Kingmaker  
 **Genre:** Strategy / Kingdom Management / Exploration  
 **Platform:** PC (Unity Engine)  
 **Perspective:**

* Global Map View: top-down / isometric-like strategy view

* Tile View: close-up, single-region playable area

**Core Fantasy:**  
 The player builds, governs, and expands a kingdom across a hex-based world. Each hex represents a region that can be explored, developed, and customized, while higher-level strategic decisions are made on the Global Map.

---

## **2\. High-Level Game Structure**

### **2.1 Dual-Scale World Model (“Hex Interface”)**

The game uses **two distinct but connected scales**:

### **A. Global Map View (Current Focus)**

* A **rectangular grid of flat-top hex tiles**

* Each hex represents a **region**

* Player movement is **tile-to-tile only**

* Physical distances (meters/feet) are abstract at this level

* Emphasis: visibility, clarity, and strategic overview

### **B. Tile View (Planned)**

* Entered by selecting a tile on the Global Map

* A **single hex-shaped playable area**

* All gameplay occurs *inside* that hex boundary

* Tile View has:

  * terrain generation

  * buildings

  * units

  * interactions

* All Tile Views share:

  * the same hex shape

  * the same boundary rules

  * the same base size (currently envisioned as **\~60 m per side**)

* Terrain and contents vary per tile

This separation allows:

* Simple math and rendering at the Global level

* Rich, spatial gameplay at the Tile level

---

## **3\. Global Map Design (Primary Active Scope)**

### **3.1 Hex Grid Specification**

**Orientation:** Flat-top  
 **Layout:** Rectangular map  
 **Coordinate Plane:** XZ plane (Y \= height)

**Hex Metrics (Global Map):**

* Outer Radius (center → corner): **1 unit**

* Inner Radius: `outerRadius * √3 / 2`

* Height/thickness: visual only (e.g., 0.2–0.5)

At the Global level, 1 unit is a *map unit*, not a real-world meter.

All systems that reference hex size must derive from a **single source of truth** (`HexMetrics`).

---

### **3.2 Global Map Responsibilities**

The Global Map:

* Displays the world as a grid of hex tiles

* Allows the player to:

  * see owned vs unowned regions

  * select tiles (click-only)

  * move one tile per turn/action

* Acts as the gateway into Tile View

The Global Map **does not**:

* simulate real-world movement distances

* contain buildings or combat

* support free-form movement

---

## **4\. Core Systems (Current \+ Near-Term)**

### **4.1 Grid Generation System**

**Component:** `FlatTopGridManager`  
 **Responsibilities:**

* Generate a rectangular flat-top hex grid

* Parent all tiles under a single `HexGridRoot`

* Prevent duplicate generation at runtime

* Place tiles using consistent spacing math

* Store tile references in a structured collection

**Design Principle:**  
 Grid placement, tile mesh generation, and selection math must all reference the same hex metrics.

---

### **4.2 Hex Tile**

Each Global Map tile:

* Is a **GameObject** spawned at runtime

* Uses:

  * a **hexagonal prism mesh** (not a cylinder)

  * a collider matching its footprint (BoxCollider or MeshCollider)

* Represents one region

Tiles may later store:

* ownership

* terrain type

* resource data

* seed for Tile View generation

---

### **4.3 Selection & Highlighting**

**Interaction Model:** Click-only selection

* Player clicks a tile → it becomes selected

* Only one tile is selected at a time

* Selection is visualized via an outline

**Selection Outline:**

* Implemented as a single `TerritoryBoundary` object

* Uses a `LineRenderer`

* Draws a thin outline aligned exactly to the tile’s edges

* Corner math must match the hex mesh (flat-top, 30° offset)

There is **no hover logic** at this stage.

---

### **4.4 Camera (Global Map)**

* Top-down / angled view suitable for strategy

* Supports:

  * RMB rotation

  * mouse wheel zoom (planned/partially implemented)

* Camera behavior is independent from Tile View camera behavior

---

## **5\. Tile View (Planned, Not Active Yet)**

### **5.1 Tile View Concept**

When entering a tile:

* The player transitions from Global Map → Tile View

* Tile View is:

  * one hex-shaped playable area

  * bounded by a fixed hex border

* Size is conceptually **\~60 m per side** (subject to tuning)

### **5.2 Tile View Responsibilities**

Tile View will handle:

* terrain generation

* buildings and infrastructure

* units and interactions

* local movement and combat (e.g., 30 ft movement)

Tile View is **not** responsible for:

* world-scale navigation

* global economics

* multi-tile pathing

---

## **6\. Scaling Philosophy**

### **Global Map**

* Uses **abstract units**

* Optimized for:

  * readability

  * performance

  * clear adjacency

* Hex radius \= 1 keeps math and visuals simple

### **Tile View**

* Uses **real-feeling spatial scale**

* Hex side length \~60 m is a starting hypothesis

* Chosen to allow:

  * meaningful movement

  * visible buildings

  * internal layout variety

**Key Principle:**  
 Global scale and Tile scale are intentionally decoupled.

---

## **7\. Unity Architecture & Terminology**

### **Key Unity Concepts Used**

* **GameObject:** entities in the Hierarchy (tiles, grid root, boundary)

* **Component:** behavior attached to GameObjects (scripts, renderers, colliders)

* **Prefab:** reusable templates for tiles, units, buildings

* **Scene:** Global Map scene and future Tile View scene(s)

### **Tooling & Workflow**

* Runtime-generated content is expected

* Visual changes require:

  * correct prefab setup

  * regeneration of runtime objects

* Editor tooling (prefabs, ProBuilder, colliders) is part of the intended workflow, not avoided

---

## **8\. Current Development Priorities**

### **Phase 1 (Active)**

* Correct Global Map hex rendering

* Replace cylinders with hex prisms

* Ensure grid spacing and orientation are correct

* Reliable click selection and aligned outline

### **Phase 2 (Next)**

* Robust camera zoom

* Tile ownership and state data

* Transition mechanics: Global → Tile View

### **Phase 3 (Later)**

* Tile View scene

* Terrain generation

* Buildings and units

* Player customization

---

## **9\. Design Principles to Preserve**

1. **Single Source of Truth**  
    Hex size, orientation, and corners are defined once and reused everywhere.

2. **Separation of Scales**  
    Global Map ≠ Tile View. Never mix assumptions between them.

3. **Editor \+ Code Cooperation**  
    Some visual work belongs in prefabs and editor tools, not just scripts.

4. **Incremental Stability**  
    Each system should be correct and testable before layering on the next.

---

## **10\. Open Design Questions (Intentionally Deferred)**

* How many tiles can the player own?

* Is Tile View persistent or regenerated from a seed?

* Are Tile Views streamed or scene-loaded?

* How does combat integrate with Tile View scale?

These are deferred **by design** to avoid premature complexity.

