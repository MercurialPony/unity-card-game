![](https://i.imgur.com/ySs7i0E.mp4)

# Task
Create a game based on the battle of two "stacks" of units.

Units:
- Footman (light unit, low attack and defense, low cost, cloneable, healable). Special action - they can put on additional upgrades on knights (helmet, shield, pike, horse) if the knight is near.
- Archer (light unit, small attack and defense, average price, cloned, healed). Special action - they are able to shoot from a bow (medium strength) at a certain range.
- Mage (light unit, medium attack and defense, high cost, cannot be cloned, cannot be healed). Special action - they can clone units next to them.
- Healer (light unit, low attack and defense, high price, not cloneable, healable). Special action - they can heal nearby units.
- Knight (heavy unit, great attack and defense, high price, not cloneable, healable). They have no special actions. Can equip upgrades that increase its strength or defense. These upgrades can be destroyed with a certain chance with enemy attacks. Upgrades can only be worn once (for example, you cannot equip two shields). But if an upgrade is destroyed, it can be worn again.
- Tumbleweed (heavy unit, zero attack and very high defense, price is very high, can't be cloned, can't be healed). Has no specials.

The game proceeds as follows: first, armies are created for a certain amount of gold. One or both can be created automatically. Next it should be possible to advance the game by playing out turns. Turns can be undone and redone.

A turn consists of a sequence of actions:
- First, a side is randomly selected. The first unit of the selected side attacks the first unit on the opposite side.
- If the opposing unit doesn't die, it responds with another attack.
- After that, starting from the second unit, each unit uses its special action with a chance.
- After all special actions have been completed, all destroyed units are removed from the field and the turn is considered finished.

The game should support the following unit formations:
- 1 on 1
- 3 on 3
- wall on wall

All units must inherit from the IUnit interface, which should define common properties such as Id, Name, HP, Attack, Defense, Cost.

The project must include at least 8 software design patterns and their usage must be justified.

# Tech stack
A game on the Unity engine using C# as the language

# How to play
You can download the latest compiled binary from the [releases page](https://github.com/Melonslise/unity-card-game/releases)

# How to set up
1. Clone the repo
2. Open the downloaded folder in Unity (File -> Open Project)