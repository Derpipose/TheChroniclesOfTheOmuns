namespace PlayerApp.Models;

public class SyncResult {
    public bool Success { get; set; }
    public int Inserted { get; set; }
    public int Updated { get; set; }
    public string Message { get; set; } = "";
}