## Crash-Ball-Clone

A physics-based multiplayer ball crash prototype built in Unity 2022.3.62f2.  
This project demonstrates real-time ball physics, player interactions, and obstacle mechanics with visual and audio feedback. Designed as a rapid prototype for gameplay testing and iteration.

---

## 🛠️ How to Run

1. Clone the repository:  

```bash git clone https://github.com/ahsan015015/Crash-Ball-Clone.git ```

## 🎮 Gameplay Overview

Crash-Ball-Clone is an arena where balls are launched and interact with players and obstacles. The main objective is eliminate opponents and survive.

## 📹 Interpretation of the video

I have tried to implement all the mechanisms from the video.. I have tried to search on youtube about the game and have proper understanding of the game mechanism and tried to implement as much as possible and add some additional elements.

## 🛠️ Key features implemented:  
- Real-time ball physics with velocity boost on collisions  
- Player-controlled movement (keyboard for pc, swipe input for mobile)  
- Interactive obstacles: ball launcher platforms, barriers, and central pillar.
- Win conditions: Last player alive
- UI for indicating points
- Some indications like some blink effect when ball hit obstacle , goal indication , ball spawn indication..
- On player death a barrier on the player line.
- A simple menu system.
- On winning player's interaction (simple)
- On death interaction
- Simple AI that defend ball and try to survive

---

## ➕ Additional Additions
- A central pillar that will give player a defense barrier for aa while when the player hit it with the ball.
- 2 type of winning condition (changeable form unity editor). In the video i saw the mechanism is even if its the last man standing still he/she needs to play till the end. So I implemented that and additionally added a way to end it as soon as its the last man standing.

## 🗓️ Known limitations or areas for improvement
- Sometimes the ball can glitch because of some collider issue. It can be improved by making custom 3D model for the hoverboard.
- The ai's can be more intelligent and I could have added some difficulty variation settings.
- The graphics can be improved :)

## ⚙️ Core Mechanics (Scripts Breakdown)

### Game Management
- **GameManager.cs**: Handles player elimination, win conditions, and game state.  
- Supports **LastAlive** and **AllDead** win modes.  
- Plays audio feedback on wins, ball hits, and health loss.  
- Moves the winner to a central spot and displays a win panel.  

### Player Controls
- **PlayerController.cs**: Enables smooth horizontal movement.  
- Supports swipe input on mobile and keyboard input in the editor.  
- Movement is clamped within arena bounds.  

### Ball Physics
- **BallController.cs**: Manages launching, velocity clamping, and collision boosting.  
- Tracks which player last hit the ball.  
- Plays hit sounds on collisions.  
- Keeps movement constrained to the XZ plane.  

### Obstacles and Interactive Elements
- **MidBouncer.cs**: Platforms that bounce vertically with randomized timing, change materials on hit, and activate temporary deadlines.  
- **Rim.cs**: Rims that activate or blink when balls collide for visual feedback.  
- **Barrier.cs**: Player-specific barriers that deal damage when hit by a ball and play a hit sound.  
- **HitVisualSwitcher.cs**: Switches visuals when objects are hit for immediate feedback.  

### Player Types
- **PlayerType.cs**: Defines multiple players (player1–player4) to support multiplayer gameplay.  

### Health System
- **HealthManager.cs**: Stores player max health and can be extended for health UI and damage tracking.  

---
