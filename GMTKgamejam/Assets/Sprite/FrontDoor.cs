public class FrontDoor : Door
{
    public static FrontDoor Instance;

    void Awake()
    {
        Instance = this;
    }
}