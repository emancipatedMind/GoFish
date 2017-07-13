namespace GoFish {
    using System.Collections.Generic;
    public interface IAutomatedPlayer : IPlayer {
        CardRequest MakeRequest(IEnumerable<IPlayer> players);
        void CommitRoundToMemory(IEnumerable<CardRequestResult> results);
        void ClearMemory();
    }
}