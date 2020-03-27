abstract class Agent {
	public readonly string name;

	public Agent(string pName) {
		name = pName;
	}

	abstract public int ChooseMove (GameBoard current, int timeLeftMS); 
}
