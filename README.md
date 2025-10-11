# Characteristics

## Core Features

### Main Mechanic
All mechanics are implemented as required, although to create a different asthaetic I used runes and did implement a variation of the "special candy" (see [Tablet Game Mechanic](#tablet-game-mechanic)).

### User Interface
There are 3 scenes, *Main Menu*, *Level 1*, and *Level 2*. Additionally, there are multiple HUD overlays for the level elements (score, timer, pause button, etc.), pause menu, game over screen, and level complete screen. Buttons that switch between scenes (such as "Return to Main Menu" or "Restart Level") use an enum to track the build indexes of the scenes for better scalability.

### Level, Tile, and Rune Design
In keeping with the rustic theme, the grid is represented as a wooden board. The initial grid generation uses a uniform distribution for each cell that doesn't include a color if the cell would make a run of 3 such colors. To balance with the chaotic *tablet* mechanic (see [Tablet Game Mechanic](#tablet-game-mechanic)), I did change the required number of matches for each color from 3 to 8. The number of moves (15) and timer (90 seconds) remained the same.

### Level 1 + 2
The distribution for each level works as outlined by the assignment. To maintain modularity, I created an interface ```ICascadeRefiller``` which implements ```GenerateColor()```. Each level creates its own derivation of the interface and overrides the method, which implements each distribution. This way, more levels with distinct distributions can be easily added. Also, for the edge case where a tile is part of a horizontal and vertical match, it will act as just a vertical match with no special impact on the distribution, although it could activate a *tablet* (see [Tablet Game Mechanic](#tablet-game-mechanic)).

### Tile-matching Mechanics
All runes are swappable with adjacent runes, and I intentionally made it so that swapping runes that do not result in a match does *not* waste a move. This is because it might be an accident, especially on mobile, which I didn't want to publish the player for, especially with the low number of moves available. It *does* however do a little cancelling animation to indicate to the player that nothing happened.

### Cascading Mechanics
Cascading was implemented by first felling existing runes into the missing tiles, and then spawning new runes at the top. The grid is then searched in constant time for rows of 3+ same-coloured runes. Consecutive rows (like 4 runes of the same color in a row) are merged, and batch cleared - which is scored using an increasing multiplier for each new cascade.

### Scoring
Each match is scored using the following formula: 10 * (# of runes in row) * (cascade level). The cascade level starts at 1, but with each new *consecutive* cascade, the multiplier increases by 1. The initial clearage of a *tablet* (see [Tablet Game Mechanic](#tablet-game-mechanic)) is scored by 10 * (# of runes cleared), and subsequent cascades are score like normal.

### End-Game and Reward
The game over screen follows the outlined requirements, as well as includes a description for why the player failed (out of time, or out of moves). The level complete screen has the following thresholds: 3 stars (12000+), 2 stars (5000-12000), 1 star (1500-5000), and 0 stars (0-1500). All these scores were tested to find the optimal thresholds for the game's scoring mechanics.

### Other Rules
The following additional features were also implemented:
* Music
* SFX
* Animations
* Reshuffling: after every match/cascade/grid-fill, the grid will search for possible matches - if none exist, the grid is refilled.
* As previously mentioned, a rune theme is used instead of a candy theme.
* The board is always filled before matches and cascades can be made. Rune swapping input is disabled when cascading.
* There is an additional button in the main menu for quitting the game.
* Non-default fonts are used for a unique asthaetic.

## Special Features
There are a few additional features I added that go beyond the requirements of the assignment, which I'd like to be taken into consideration during evaluation:

### Full desktop + mobile support
There are desktop and mobile versions of all the game and UI layouts, as well as support for both mouse and touch input. I've tested both a Windows build and an Android build (through Android Studio's emulator).

### Scrolling backgrounds
To add to the asthaetics of the game, I implemented a scrolling background feature to make the game feel more dynamic.

### Animations
Another method of making the game less static and more dynamic is the use of animations when matching and cascading runes, which are applied to the runes and different parts of the UI. For example, the score label will animate when updating, as well as the number of required runes, and the number of moves left. Also, the timer will animate when counting down the final 10 seconds.

### Dynamic BKG music
The background music is constant throughout the game, but it will lower in volume when in the pause menu. These changes in volume are also smooth, not abrupt.

### Tablet game mechanic
Instead of special runes, I added *tablets*, which are unlocked when certain combo patterns are matched - specifically, an L shape, cross shape, five-in-a-row, and 2x2 area. The player can then activate an unlocked rune, which will clear a particular pattern of runes in the grid - leading to a huge bonus score and lots of potentital cascades, although the initial clearage does not contribute to the required number of rune matches. Because this adds a lot to the score, and cascades are frequent from it, I increased the stars' score thresholds and the required number of matches from 3 to 8, in order to balance the difficulty.

# Attribution

All sprites (except background textures) were created by me using Libresprite. All remaining assets are CC0-licensed, save for those below, which are CC3-licensed.

---

Font: Romulus<br>
Author: Pix3M<br>
From: OpenGameArt<br>
https://creativecommons.org/licenses/by/3.0/

---

Font: Alagard<br>
Author: Pix3M<br>
From: OpenGameArt<br>
https://creativecommons.org/licenses/by/3.0/

---

Sound effect pack: (in Assets/Audio/Tablets)<br>
Author: Michel Baradari apollo-music.de<br>
From: OpenGameArt<br>
https://creativecommons.org/licenses/by/3.0/

