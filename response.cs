using System.Collections;
using System.Collections.Generic;

namespace Celin.AIS
{
    /*
     * AIS Response definitions
     */
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
    public class Form<F>
    {
        public F data { get; set; }
        public string title { get; set; }
        public ErrorWarning[] errors { get; set; }
        public ErrorWarning[] warnings { get; set; }
    }
    public class FormResponse : Response
    {
        public int stackId { get; set; }
        public int stateId { get; set; }
        public string rid { get; set; }
        public string currentApp { get; set; }
        public string [] sysErrors { get; set; }
    }
    public class FileAttachmentResponse
    {
        public string uniquefilename { get; set; }
        public string itemName { get; set; }
        public int sequence { get; set; }
    }
    public class AttachmentItem
    {
        public string file { get; set; }
        public string itemName { get; set; }
        public string link { get; set; }
        public int moType { get; set; }
        public string queue { get; set; }
        public int sequence { get; set; }
        public string updateDate { get; set; }
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
}
