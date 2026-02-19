using System;
using System.IO;
using Microsoft.AspNetCore.DataProtection;

namespace SnowrunnerMerger.Desktop.Services.Auth;

public class SecureTokenStore
{
    private readonly IDataProtector _protector;
    private readonly string _tokenPath;

    public SecureTokenStore(IDataProtectionProvider protectorProvider, string appDataPath)
    {
        _protector = protectorProvider.CreateProtector("RefreshTokenProtector");
        
        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }
        
        _tokenPath = Path.Combine(appDataPath, "refresh.token");
    }

    public void Save(string refreshToken)
    {
        var protectedToken  = _protector.Protect(refreshToken);
        File.WriteAllText(_tokenPath, protectedToken);
    }

    public string? Load()
    {
        if (!File.Exists(_tokenPath))
        {
            return null;
        }
        
        var protectedToken = File.ReadAllText(_tokenPath);
        
        return _protector.Unprotect(protectedToken);
    }
    
        public void Clear()
        {
            if (File.Exists(_tokenPath))
            {
                File.Delete(_tokenPath);
            }
        }
}