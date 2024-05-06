namespace GDCUI
{
    public class SessionHandler
    {
        public string GenerateID()
        {
            return (Guid.NewGuid().ToString("N").Substring(0, 4) + "-" +
                   Guid.NewGuid().ToString("N").Substring(4, 4) + "-" +
                   Guid.NewGuid().ToString("N").Substring(8, 4)).ToUpperInvariant();
        }
    }
}
