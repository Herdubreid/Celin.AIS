using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Celin.AIS;

/* Helpers */
public enum ControlType
{
    Menu,
    Row,
    Form,
    Other
}
public record ControlField(ControlType type, string title);
public record FormField(string Alias, string Title, object Value);
public record FormFields(Dictionary<int, FormField> data);
/*
 * AIS Response definitions
 */
public record PoOption(
    int type,
    string value);
public record PoValue(
    int id,
    string value);

public record FormField<T>(
    string title,
    T value);
public record Summary(
    int records,
    bool moreRecords);
public record GridData<R>(
    int id,
    IDictionary<string, string> titles,
    IDictionary<string, string> columns,
    IEnumerable<R> rowset,
    Summary summary);
public record FormData<R>
{
    public GridData<R> gridData { get; init; }
}
public record Output<R>(
    IEnumerable<R> output,
    ErrorResponse error);
public record ErrorWarning(
    string CODE,
    string TITLE,
    string ERRORCONTROL,
    string DESC,
    string MOBILE);
public record Response()
{
    public string message { get; init; }
    public string exception { get; init; }
    public string timeStamp { get; init; }
    public Response(Response rs)
    {
        message = rs.message;
        exception = rs.exception;
        timeStamp = rs.timeStamp;
    }
}
public record PoResponse<T> : Response
{
    public string application { get; init; }
    public string version { get; init; }
    public T processingOptions { get; init; }
}
public record Form<F>
{
    public F data { get; init; }
    public string title { get; init; }
    public IEnumerable<ErrorWarning> errors { get; init; }
    public IEnumerable<ErrorWarning> warnings { get; init; }
}
public record DemoForm<F, C>(
    F data,
    string title,
    IEnumerable<ErrorWarning> errors,
    IEnumerable<ErrorWarning> warnings,
    C actionControls);
public record Link(
    string rel,
    string href,
    string context);
public record FormResponse() : Response
{
    public int stackId { get; init; }
    public int stateId { get; init; }
    public string rid { get; init; }
    public string currentApp { get; init; }
    public IEnumerable<ErrorWarning> sysErrors { get; init; }
    public IEnumerable<Link> links { get; init; }
    public FormResponse(FormResponse rs) : base(rs)
    {
        currentApp = rs.currentApp;
        stackId = rs.stackId;
        stateId = rs.stateId;
        rid = rs.rid;
    }
}
public record UBEResponse
{
    public string reportName { get; init; }
    public string reportVersion { get; init; }
    public int jobNumber { get; init; }
    public string executionServer { get; init; }
    public string port { get; init; }
    public string jobStatus { get; init; }
    public string objectType { get; init; }
    public string user { get; init; }
    public string environment { get; init; }
    public DateTime submitDate { get; init; }
    public DateTime lastDate { get; init; }
    public int submitTime { get; init; }
    public int lastTime { get; init; }
    public string oid { get; init; }
    public string queueName { get; init; }
}
public record FileAttachmentResponse
{
    public string uniquefilename { get; init; }
    public string itemName { get; init; }
    public int sequence { get; init; }
}
public record AttachmentItem(
    string downloadUrl,
    string data,
    string file,
    string itemName,
    string link,
    int moType,
    string queue,
    int sequence,
    DateTime updateDate,
    int updateHourOfDay,
    int updateMinuteOfHour,
    int updateSecondOfMinute,
    int updateTimeStamp,
    string updateUserID,
    bool hasValidTimestamp,
    bool isDefaultImage,
    bool isImage,
    bool isMisc,
    bool isOLE,
    bool isShortCut,
    bool isText,
    bool isUpdated,
    bool isURL);
public record AttachmentListResponse
{
    public IEnumerable<AttachmentItem> mediaObjects { get; init; }
}
public record AttachmentResponse
{
    public string ssoEnabled { get; init; }
    public string text { get; init; }
    public bool isRTF { get; init; }
    public string itemName { get; init; }
    public string addTextStatus { get; init; }
    public string updateTextStatus { get; init; }
    public string deleteStatus { get; init; }
    public string saveURL { get; init; }
    public string urlText { get; init; }
    public string error { get; init; }
    public int sequence { get; init; }
    public IEnumerable<AttachmentResponse> textAttachments { get; init; }
}
public record PreferenceResponse
{
    public string objectName { get; init; }
    public string type { get; init; }
    public string preferenceData { get; init; }
}
public record Rowcount(
    int records,
    bool modifier,
    int gridrows);
public record WatchListResponse
{
    public string watchListOBNM { get; init; }
    public string name { get; init; }
    public string formtitle { get; init; }
    public string queryname { get; init; }
    public Rowcount rowcount { get; init; }
    public int lastRunTime { get; init; }
    public string Description { get; init; }
    public int warningThreshold { get; init; }
    public int criticalThreshold { get; init; }
    public string queryObjectName { get; init; }
    public int maxRecords { get; init; }
    public string formOID { get; init; }
    public bool isCritical { get; init; }
}
public record ErrorDetails(
    IEnumerable<ErrorWarning> errors);
public record ErrorResponse
{
    public string message { get; init; }
    public string exception { get; init; }
    public string timeStamp { get; init; }
    public string type { get; init; }
    public string userDefinedError { get; init; }
    public ErrorDetails errorDetails { get; init; }
}
public record AggregationOutput<T>(
    T groupBy);
public record AggregationResponse<T>(
    IEnumerable<T> output);
public record DdInfo(
    string ddict,
    string alias,
    string longName,
    string systemCode,
    int type,
    int length,
    int decimals,
    int dispDecimals,
    string searchFormName,
    string searchAppName,
    int nextNumberIndex,
    string nextNumberSystem,
    int style,
    string as400EditRule,
    string as400EditParm1,
    string as400EditParm2,
    string editRuleBFName,
    string as400DisplayRule,
    string as400DisplayParm,
    string dispRuleBFName,
    int currency,
    string dfltValue);
public record Control(
    int type,
    int idControl,
    int pageNumber,
    int idObject,
    string title,
    string memberName,
    DdInfo ddInfo);
public record RiItem(
    int idItem,
    string szDict,
    string szDesc,
    int length,
    int dataType,
    string szDataItem);
public record TabPage(
    int type,
    int idControl,
    int pageNumber,
    int idObject,
    string title,
    IEnumerable<Control> controls,
    string poglossaryOverride);
public record PoPrompt(
    IEnumerable<TabPage> tabPages);
public record DataSelectionColumn(
    int idEVDT,
    string displayString,
    int @type,
    string view,
    string dictItem,
    string table);
public record ReportField(
    [property: JsonPropertyName("@type")]
    string stype,
    int idEVDT,
    string displayString,
    int type,
    int sectionID,
    int variableID);
public record ReportSecurity(
    bool canChangePO,
    bool canRunPO,
    bool canRunDataSelection,
    bool hasFullDataSelectionAccess,
    bool canAddDataSelection,
    bool canModifyDataSelection,
    int versionSecurityFlag);
public record PrintOptions(
    string printerName,
    string paperType,
    int orientation,
    int printStyleSDT,
    int numberCopies,
    int paperSource,
    bool printImmediate,
    bool savePDLFile,
    bool saveCSVFile,
    string osaInterfaceName);
public record DiscoveryUBEResponse
{
    public string reportName { get; init; }
    public string reportVersion { get; init; }
    public DataSelection dataSelection { get; init; }
    public DataSequence dataSequence { get; init; }
    public IEnumerable<PoValue> poValues { get; init; }
    public PoPrompt poPrompt { get; init; }
    public IEnumerable<DataSelectionColumn> dataSelectionColumns { get; init; }
    public IEnumerable<ReportField> dataSelectionReportFields { get; init; }
    public ReportSecurity reportSecurity { get; init; }
    public IEnumerable<RiItem> reportInterconnects { get; init; }
    public PrintOptions printOptions { get; init; }
    string lastModifiedUser { get; init; }
}
public record UserInfo(
    string token,
    string langPref,
    string locale,
    string dateFormat,
    string dateSeperator,
    string simpleDateFormat,
    string decimalFormat,
    int addressNumber,
    string alphaName,
    string appsRelease,
    string country);
public record AuthResponse : Response
{
    public string username { get; init; }
    public string environment { get; init; }
    public string role { get; init; }
    public string jasserver { get; init; }
    public UserInfo userInfo { get; init; }
    public bool userAuthorized { get; init; }
    public string version { get; init; }
    public string aisSessionCookie { get; init; }
    public bool adminAuthorized { get; init; }
}
public record TokenValidationResponse : Response
{
    public bool isValidSession { get; init; }
}
