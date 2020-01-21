public class PlatformProvider
{
    public ILayer[] layers;
    private static PlatformProvider instance;
    public static PlatformProvider Instance => instance ?? (instance = new PlatformProvider());
    
}
