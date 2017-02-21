
using System;
using System.Collections.Generic;

using JuloUtil;

namespace TurtleIsland {
	public class TeamManager {
		public Character currentCharacter = null;
		private int currentTeamIndex;
		
		private List<Team> teams;
		private Dictionary<Character, float> characterLastTurn;
		
		private int numTeams;
		private int numCharacters;
		
		public TeamManager(int numTeams, int numCharaters) {
			this.numTeams = numTeams;
			this.numCharacters = numCharacters;
			this.teams = new List<Team>();
			characterLastTurn = new Dictionary<Character, float>();
			currentTeamIndex = -1;
			//Debug.Log("new team manager: 0");
		}
		
		public void startsTeam(int teamId) {
			for(int i = 0; i < teams.Count; i++) {
				Team t = teams[i];
				if(t.id == teamId) {
					currentTeamIndex = i - 1;
					break;
				}
			}
		}
		public void addTeam(Team team) {
			this.teams.Add(team);
		}
		
		public void addCharacter(Character c) {
			Team t = getTeamFor(c);
			t.addCharacter(c);
			characterLastTurn.Add(c, JuloTime.gameTime());
		}
		
		public void removeCharacter(Character c) {
			Team t = getTeamFor(c);
			t.removeCharacter(c);
			characterLastTurn.Remove(c);
		}
		
		public Team getTeamFor(Character c) {
			foreach(Team t in teams)
				if(t.id == c.getTeamId())
					return t;
			throw new ApplicationException("No team found");
		}
		
		public Team currentTeam() {
			return teams[currentTeamIndex];
		}
		
		public IEnumerable<Character> allActiveCharacters() {
			foreach(Team team in teams) {
				foreach(Character c in team) {
					if(c.isActive())
						yield return c;
				}
			}
		}
		
		public Character nextCharacter() {
			currentTeamIndex = (currentTeamIndex + 1) % teams.Count;
			
			float now = JuloTime.gameTime();
			float oldestTimestamp = now;
			
			List<Character> candidates = new List<Character>();
			
			foreach(Character c in currentTeam()) {
				if(c.isReady()) {
					float lastTurn = characterLastTurn[c];
					
					if(lastTurn == oldestTimestamp) {
						candidates.Add(c);
					} else if(lastTurn < oldestTimestamp) {
						candidates.Clear();
						candidates.Add(c);
						oldestTimestamp = lastTurn;
					}
				}
			}
			
			int numCandidates = candidates.Count;
			if(numCandidates == 0) {
				throw new ApplicationException("No candidate found");
			} else if(numCandidates == 1) {
				currentCharacter = candidates[0];
			} else {
				currentCharacter = candidates[JuloMath.randomInt(0, numCandidates - 1)];
			}
			
			characterLastTurn[currentCharacter] = now;
			
			return currentCharacter;
		}
		
		public TurtleIslandStatus getStatus() {
			TurtleIslandStatus ret = new TurtleIslandStatus(numTeams, numCharacters);
			
			// TODO remove this!
			int readyTeams = 0;
			int readyChars = 0;
			foreach(Team t in teams) {
				bool teamIsReady = false;
				foreach(Character c in t) {
					if(c.isReady()) {
						readyChars++;
						teamIsReady = true;
					}
				}
				if(teamIsReady)
					readyTeams++;
			}
			ret.readyTeams = readyTeams;
			ret.readyChars = readyChars;
			
			return ret;
		}
		
		public Team getSomeReadyTeam() {
			foreach(Team t in teams)
				if(t.isReady())
					return t;
			return null;
		}
		
		public List<Character> getReadyRivals(Character c) {
			List<Character> ret = new List<Character>();
			
			foreach(Team t in teams)
				if(t.id != c.getTeamId())
					foreach(Character rival in t)
						if(rival.isReady())
							ret.Add(rival);
			
			return ret;
		}
		
		public Character getLastCharacterOf(Team team) {
			Character ret = null;
			float lastTime = 0f;
			
			foreach(Character c in team) {
				if(c.isReady()) {
					float thisTime = characterLastTurn[c];
					if(ret == null || thisTime > lastTime) {
						ret = c;
						lastTime = thisTime;
					}
				}
			}
			
			return ret;
		}
	}
}