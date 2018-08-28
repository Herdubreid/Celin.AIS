using Newtonsoft.Json.Linq;
namespace Celin.AIS
{
    /*
 * AIS Response definitions
 */
    public class Cell
    {
        public string title { get; set; }
        public string longName { get; set; }
        public string assocDesc { get; set; }
        public string value { get; set; }
    }
    public class String : Cell
    {
        public string internalValue { get; set; }
    }
    public class Number : Cell
    {
        public decimal internalValue { get; set; }
    }
    public class Int : Cell
    {
        public int internalValue { get; set; }
    }
    public class Row
    {
        public int rowIndex { get; set; }
    }
    public class Summary
    {
        public int records { get; set; }
        public bool moreRecords { get; set; }
    }
    public class GridData<R>
    {
        public int id { get; set; }
        public object titles { get; set; }
        public R[] rowset { get; set; }
        public Summary summary { get; set; }
    }
    public class FormData<R>
    {
        public GridData<R> gridData { get; set; }
    }
    public class ErrorWarning
    {
        public string CODE { get; set; }
        public string TITLE { get; set; }
        public string ERRORCONTROL { get; set; }
        public string DESC { get; set; }
        public string MOBILE { get; set; }
    }
    public class Response
    {
        public string message { get; set; }
        public string exception { get; set; }
        public string timeStamp { get; set; }
    }
    public class Form<F>
    {
        public F data { get; set; }
        public string title { get; set; }
        public ErrorWarning[] errors { get; set; }
        public ErrorWarning[] warnings { get; set; }
    }
    public class FormResponse : Response
    {
        public string stackId { get; set; }
        public string rid { get; set; }
        public string currentApp { get; set; }
        public JArray sysErrors { get; set; }
    }
    public class UserInfo
    {
        public string token { get; set; }
        public string langPref { get; set; }
        public string locale { get; set; }
        public string dateFormat { get; set; }
        public string dateSeperator { get; set; }
        public string simpleDateFormat { get; set; }
        public string decimalFormat { get; set; }
        public string addressNumber { get; set; }
        public string alphaName { get; set; }
        public string appsRelease { get; set; }
        public string country { get; set; }
    }
    public class AuthResponse : Response
    {
        public string username { get; set; }
        public string environment { get; set; }
        public string role { get; set; }
        public string jasserver { get; set; }
        public UserInfo userInfo { get; set; }
        public string userAuthorized { get; set; }
        public string version { get; set; }
        public string aisSessionCookie { get; set; }
        public string adminAuthorized { get; set; }
    }
}
