public interface ILayer
{
    void CreatePlatforms();
    void DestroyLayer();
    void NextTurn();
    Platform[] currentPlatforms { get;}
    
}

