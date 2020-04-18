namespace Thandizo.WebPortal.Services
{
    public interface ICookieService
    {
        void Add(string friendlyName, string value);
        void Delete(string friendlyName);
        string Get(string friendlyName);
    }
}