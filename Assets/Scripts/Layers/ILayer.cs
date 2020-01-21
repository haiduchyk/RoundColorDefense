public interface ILayer
{
    void CreatePlatforms();
    void DestroyLayer();
    void NextTurn();
    void ChangeState();
    Platform[] currentPlatforms { get;}
    
}

