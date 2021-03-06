using System;
using System.Collections.Generic;

namespace Celin.AIS
{
    /*
     * AIS Response definitions
     */
    public class PoOption
    {
        public int type { get; set; }
        public string value { get; set; }
    }
    public class PoValue
    {
        public int id { get; set; }
        public string value { get; set; }
    }
    public class FormField<T>
    {
        public string title { get; set; }
        public T value { get; set; }
    }
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
        public IDictionary<string, string> titles { get; set; }
        public IDictionary<string, string> columns { get; set; }
        public R[] rowset { get; set; }
        public Summary summary { get; set; }
    }
    public class FormData<R>
    {
        public GridData<R> gridData { get; set; }
    }
    public class Output<R>
    {
        public R[] output { get; set; }
        public ErrorResponse error { get; set; }
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
    public class PoResponse<T> : Response
    {
        public string application { get; set; }
        public string version { get; set; }
        public T processingOptions { get; set; }
    }
    public class Form<F>
    {
        public F data { get; set; }
        public string title { get; set; }
        public IEnumerable<ErrorWarning> errors { get; set; }
        public IEnumerable<ErrorWarning> warnings { get; set; }
    }
    public class Link
    {
        public string rel { get; set; }
        public string href { get; set; }
        public string context { get; set; }
    }
    public class FormResponse : Response
    {
        public int stackId { get; set; }
        public int stateId { get; set; }
        public string rid { get; set; }
        public string currentApp { get; set; }
        public IEnumerable<ErrorWarning> sysErrors { get; set; }
        public IEnumerable<Link> links { get; set; }
    }
    public class UBEResponse
    {
        public string reportName { get; set; }
        public string reportVersion { get; set; }
        public int jobNumber { get; set; }
        public string executionServer { get; set; }
        public string port { get; set; }
        public string jobStatus { get; set; }
        public string objectType { get; set; }
        public string user { get; set; }
        public string environment { get; set; }
        public DateTime submitDate { get; set; }
        public DateTime lastDate { get; set; }
        public int submitTime { get; set; }
        public int lastTime { get; set; }
        public string oid { get; set; }
        public string queueName { get; set; }
    }
    public class FileAttachmentResponse
    {
        public string uniquefilename { get; set; }
        public string itemName { get; set; }
        public int sequence { get; set; }
    }
    public class AttachmentItem
    {
        public string downloadUrl { get; set; }
        public string data { get; set; }
        public string file { get; set; }
        public string itemName { get; set; }
        public string link { get; set; }
        public int moType { get; set; }
        public string queue { get; set; }
        public int sequence { get; set; }
        public DateTime updateDate { get; set; }
        public int updateHourOfDay { get; set; }
        public int updateMinuteOfHour { get; set; }
        public int updateSecondOfMinute { get; set; }
        public int updateTimeStamp { get; set; }
        public string updateUserID { get; set; }
        public bool hasValidTimestamp { get; set; }
        public bool isDefaultImage { get; set; }
        public bool isImage { get; set; }
        public bool isMisc { get; set; }
        public bool isOLE { get; set; }
        public bool isShortCut { get; set; }
        public bool isText { get; set; }
        public bool isUpdated { get; set; }
        public bool isURL { get; set; }
    }
    public class AttachmentListResponse
    {
        public AttachmentItem[] mediaObjects { get; set; }
    }
    public class AttachmentResponse
    {
        public string ssoEnabled { get; set; }
        public string text { get; set; }
        public bool isRTF { get; set; }
        public string itemName { get; set; }
        public string addTextStatus { get; set; }
        public string updateTextStatus { get; set; }
        public string deleteStatus { get; set; }
        public string saveURL { get; set; }
        public string urlText { get; set; }
        public string error { get; set; }
        public int sequence { get; set; }
        public AttachmentResponse[] textAttachments { get; set; }
    }
    public class PreferenceResponse
    {
        public string objectName { get; set; }
        public string type { get; set; }
        public string preferenceData { get; set; }
    }
    public class Rowcount
    {
        public int records { get; set; }
        public bool modifier { get; set; }
        public int gridrows { get; set; }
    }
    public class WatchListResponse
    {
        public string watchListOBNM { get; set; }
        public string name { get; set; }
        public string formtitle { get; set; }
        public string queryname { get; set; }
        public Rowcount rowcount { get; set; }
        public int lastRunTime { get; set; }
        public string Description { get; set; }
        public int warningThreshold { get; set; }
        public int criticalThreshold { get; set; }
        public string queryObjectName { get; set; }
        public int maxRecords { get; set; }
        public string formOID { get; set; }
        public bool isCritical { get; set; }
    }
    public class ErrorResponse
    {
        public string message { get; set; }
        public string exception { get; set; }
        public string timeStamp { get; set; }
        public string type { get; set; }
        public string userDefinedError { get; set; }
    }
    public class AggregationOutput<T>
    {
        public T groupBy { get; set; }
    }
    public class AggregationResponse<T>
    {
        public T[] output { get; set; }
    }
    public class DdInfo
    {
        public string ddict { get; set; }
        public string alias { get; set; }
        public string longName { get; set; }
        public string systemCode { get; set; }
        public int type { get; set; }
        public int length { get; set; }
        public int decimals { get; set; }
        public int dispDecimals { get; set; }
        public string searchFormName { get; set; }
        public string searchAppName { get; set; }
        public int nextNumberIndex { get; set; }
        public string nextNumberSystem { get; set; }
        public int style { get; set; }
        public string as400EditRule { get; set; }
        public string as400EditParm1 { get; set; }
        public string as400EditParm2 { get; set; }
        public string editRuleBFName { get; set; }
        public string as400DisplayRule { get; set; }
        public string as400DisplayParm { get; set; }
        public string dispRuleBFName { get; set; }
        public int currency { get; set; }
        public string dfltValue { get; set; }
    }
    public class Control
    {
        public int type { get; set; }
        public int idControl { get; set; }
        public int pageNumber { get; set; }
        public int idObject { get; set; }
        public string title { get; set; }
        public string memberName { get; set; }
        public IEnumerable<DdInfo> ddInfo { get; set; }
    }
    public class TabPage
    {
        public int type { get; set; }
        public int idControl { get; set; }
        public int pageNumber { get; set; }
        public int idObject { get; set; }
        public string title { get; set; }
        public IEnumerable<Control> controls { get; set; }
        public string poglossaryOverride { get; set; }
    }
    public class PoPrompt
    {
        public IEnumerable<TabPage> tabPages { get; set; }
    }   
    public class DataSelectionColumn
    {
        public int idEVDT { get; set; }
        public string displayString { get; set; }
        public int @type { get; set; }
        public string view { get; set; }
        public string dictItem { get; set; }
        public string table { get; set; }
    }
    public class ReportSecurity
    {
        public bool canChangePO { get; set; }
        public bool canRunPO { get; set; }
        public bool canRunDataSelection { get; set; }
        public bool hasFullDataSelectionAccess { get; set; }
        public bool canAddDataSelection { get; set; }
        public bool canModifyDataSelection { get; set; }
        public int versionSecurityFlag { get; set; }
    }
    public class PrintOptions
    {
        public string printerName { get; set; }
        public string paperType { get; set; }
        public int orientation { get; set; }
        public int printStyleSDT { get; set; }
        public int numberCopies { get; set; }
        public int paperSource { get; set; }
        public bool printImmediate { get; set; }
        public bool savePDLFile { get; set; }
        public bool saveCSVFile { get; set; }
        public string osaInterfaceName { get; set; }
    }
    public class DiscoveryUBEResponse
    {
        public string reportName { get; set; }
        public string reportVersion { get; set; }
        public DataSelection dataSelection { get; set; }
        public DataSequence dataSequence { get; set; }
        public IEnumerable<PoValue> poValues { get; set; }
        public PoPrompt poPrompt { get; set; }
        public IEnumerable<DataSelectionColumn> dataSelectionColumns { get; set; }
        public ReportSecurity reportSecurity { get; set; }
        public PrintOptions printOptions { get; set; }
        public string lastModifiedUser { get; set; }
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
        public int addressNumber { get; set; }
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
        public bool userAuthorized { get; set; }
        public string version { get; set; }
        public string aisSessionCookie { get; set; }
        public bool adminAuthorized { get; set; }
    }
    public class TokenValidationResponse : Response
    {
        public bool isValidSession { get; set; }
    }
}
