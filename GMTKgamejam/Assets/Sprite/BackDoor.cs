public class BackDoor : Door
{
    public static BackDoor Instance;

    void Awake()
    {
        Instance = this;
    }
}