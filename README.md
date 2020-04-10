# AdvancedTools
This project is set up for school course named Advanced Tools

In this project I will compare simple boardgame AI's, for example MiniMax vs Monte Carlo with connect 4.
I will be implementing the AI's myself, in a provided engine and provided boardgame setup and rules. Code wise
there is an abstract class GameBoard, which has basic functions like CheckWinner(): int, MakeMove(int): int,
GetMoves(): List<int> and GetActivePlayer(): int. These functions are implemented on different gameboards, for example
ConnectFourBoard. Using this information it is possible to write non-game specific AI's. For better results however
game specific AI's may be implemented in the future.

The provided engine is a custom C# engine without interface, complete code-based. It relies on a copyrighted sound library
so that will not be uploaded. 

In the C# project there is an option to compare two AI's, which can be set to 'Normal Games' or 'Compare AI'. 
This will automatically enable a tracker which will write the gamedata away to a file (not recommended, but no option to turn it off yet,
except by code) and will print this information into the console.

Current implemented Agents
=======================================
Human         // The human player, not really an agent but can play quite well.

Random        // Gets all possible moves and picks randomly.

Random+       // Gets all possible moves and picks randomly. However if there is a move which wins directly, it 
              // will pick that move.

MonteCarlo    // Plays an amount of games on every possible move on the current board. Then it will play the move
              // with the highest score (determined on wins).

MiniMax       // Recursively loops through all possible board states from the current state and will pick the move
              // with the best future. This is a heavy AI on bigger games than Tic-Tac-Toe, that's why there is depth
              // implemented which will keep the AI from searching deeper than said depth.

MiniCarlo     // Combination of MonteCarlo and Minimax (Custom name). Will use minimax until a certain depth is
		 reached, than it will use MonteCarlo, this will save perfermonce on bigger games like connect 4.

Alpha	      // Alpha Beta pruning. gets the best move the same way as minimax. However, if a node is unnecessary to
		 'calculate' it will skip that node, making it faster 

Processing Checkers
=======================================
The CheckersGame folder has a Processing project with a checkers game. Now it is important to notice that there can actually be a draw,
though the game does not recognize this yet. In the checkersGame various different AI's will be implemented as well.

Current implemented Agents
=======================================
Human         // The human player, not really an agent but can play quite well.

Random        // Gets all possible moves and picks randomly.

MonteCarlo    // Plays an amount of games on every possible move on the current board. Then it will play the move
              // with the highest score (determined on wins).