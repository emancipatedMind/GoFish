namespace GoFish {
    using System;
    public interface IManualPlayer : IPlayer {
        CardRequest MakeRequest();
    }
}