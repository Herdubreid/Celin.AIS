using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Celin.AIS
{
    /*
     * AIS Request definitions
     */
    public class Input
    {
        public string id { get; set; }
        public string value { get; set; }
    }
    public class ColumnEvent
    {
        public string command { get; set; }
        public string value { get; set; }
        public string columnID { get; set; }
    }
    public class RowEvent
    {
        public int rowNumber { get; set; }
        public IEnumerable<ColumnEvent> gridColumnEvents { get; set; }
    }
    public class Grid
    {
        public string gridID { get; set; }
    }
    public class GridInsert : Grid
    {
        public IEnumerable<RowEvent> gridRowInsertEvents { get; set; }
    }
    public class GridUpdate : Grid
    {
        public IEnumerable<RowEvent> gridRowUpdateEvents { get; set; }
    }
    public abstract class Action { }
    public class GridAction : Action
    {
        public static readonly string SetGridCellValue = "SetGridCellValue";
        public static readonly string SetGridComboValue = "SetGridComboValue";
        public Grid gridAction { get; set; }
    }
    public class FormAction : Action
    {
        public static readonly string ClickGridCell = "ClickGridCell";
        public static readonly string ClickGridColumnAggregate = "ClickGridColumnAggregate";
        public static readonly string DoAction = "DoAction";
        public static readonly string SelectAllRows = "SelectAllRows";
        public static readonly string SelectRow = "SelectRow";
        public static readonly string SetCheckBoxValue = "SetCheckBoxValue";
        public static readonly string SetComboValue = "SetComboValue";
        public static readonly string SetControlValue = "SetControlValue";
        public static readonly string SetRadioButton = "SetRadioButton";
        public static readonly string SetQBEValue = "SetQBEValue";
        public static readonly string UnSelectAllRows = "UnSelectAllRows";
        public static readonly string UnSelectRow = "UnSelectRow";
        public string controlID { get; set; }
        public string command { get; set; }
        public string value { get; set; }
    }
    public class Value
    {
        public static string LITERAL = "LITERAL";
        public static string LOGIN_USER = "LOGIN_USER";
        public static string TODAY = "TODAY";
        public static string TODAY_MINUS_DAY = "TODAY_MINUS_DAY";
        public static string TODAY_MINUS_MONTH = "TODAY_MINUS_MONTH";
        public static string TODAY_MINUS_YEAR = "TODAY_MINUS_YEAR";
        public static string TODAY_PLUS_DAY = "TODAY_PLUS_DAY";
        public static string TODAY_PLUS_MONTH = "TODAY_PLUS_MONTH";
        public static string TODAY_PLUS_YEAR = "TODAY_PLUS_YEAR";
        public string content { get; set; }
        public string specialValueId { get; set; }
    }
    public class ComplexQuery
    {
        public static readonly string AND = "AND";
        public static readonly string OR = "OR";
        public Query query { get; set; }
        public string andOr { get; set; }
    }
    public class Condition
    {
        public static readonly string BETWEEN = "BETWEEN";
        public static readonly string EQUAL = "EQUAL";
        public static readonly string GREATER = "GREATER";
        public static readonly string GREATER_EQUAL = "GREATER_EQUAL";
        public static readonly string LESS = "LESS";
        public static readonly string LESS_EQUAL = "LESS_EQUAL";
        public static readonly string LIST = "LIST";
        public static readonly string NOT_EQUAL = "NOT_EQUAL";
        public static readonly string STR_BLANK = "STR_BLANK"; 
        public static readonly string STR_CONTAIN = "STR_CONTAIN";
        public static readonly string STR_END_WITH = "STR_END_WITH";
        public static readonly string STR_NOT_BLANK = "STR_NOT_BLANK";
        public static readonly string STR_START_WITH = "STR_START_WITH";
        public IEnumerable<Value> value { get; set; }
        public string controlId { get; set; }
        public string @operator { get; set; }
        public string aggregation { get; set; }
    }
    public class Query
    {
        public static readonly string MATCH_ALL = "MATCH_ALL";
        public static readonly string MATCH_ANY = "MATCH_ANY";
        public IEnumerable<ComplexQuery> complexQuery { get; set; }
        public IEnumerable<Condition> condition { get; set; }
        public bool? autoFind { get; set; }
        public string matchType { get; set; }
        public bool? autoClear { get; set; }
    }
    public class AggregationItem
    {
        public static readonly string SUM = "SUM";
        public static readonly string MIN = "MIN";
        public static readonly string MAX = "MAX";
        public static readonly string AVG = "AVG";
        public static readonly string COUNT = "COUNT";
        public string aggregation { get; set; }
        public string column { get; set; }
        public string direction { get; set; }
        public string specialHandling { get; set; }
    }
    public class Aggregation
    {
        public static readonly string COUNT_DISTINCT = "COUNT_DISTINCT";
        public static readonly string AVG_DISTINCT = "AVG_DISTINCT";
        public static readonly string SUM_DISTINCT = "SUM_DISTINCT";
        public IEnumerable<AggregationItem> aggregations { get; set; }
        public IEnumerable<AggregationItem> groupBy { get; set; }
        public IEnumerable<AggregationItem> orderBy { get; set; }
    }
    public class ActionRequest
    {
        public string returnControlIDs { get; set; }
        public IEnumerable<Action> formActions { get; set; }
        public string formOID { get; set; }
        public string stopOnWarning { get; set; }
    }
    public abstract class Service
    {
        public abstract string SERVICE { get; }
        public string token { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string deviceName { get; set; }
        public string formName { get; set; }
        public string version { get; set; }
    }
    public abstract class Request : Service
    {
        public static readonly string VERSION1 = "VERSION1";
        public static readonly string GRID_DATA = "GRID_DATA";
        public static readonly string VERSION2 = "VERSION2";
        public static readonly string ORACLE = "ORACLE";
        public static readonly string XML = "XML";
        public static readonly string XMLSIMPLE = "XMLSIMPLE";
        public static readonly string TRUE = "TRUE";
        public static readonly string FALSE = "FALSE";
        public string findOnEntry { get; set; }
        public string returnControlIDs { get; set; }
        public string maxPageSize { get; set; }
        public bool? aliasNaming { get; set; }
        public Query query { get; set; }
        public Aggregation aggregation { get; set; }
        public string outputType { get; set; }
        public string formServiceDemo { get; set; }
        public bool? bypassFormServiceEREvent { get; set; }
    }
    public class PoRequest : Service
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "poservice";
        public string applicationName { get; set; }
    }
    public class PreferenceRequest : Service
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "preference";
        public static readonly string GET = "GET";
        public static readonly string PUT = "PUT";
        public string action { get; set; }
        public string idList { get; set; }
        public string objectName { get; set; }
        public int sequence { get; set; }
        public string preferenceData { get; set; }
    }
    public class WatchListRequest : Service
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "watchlist";
        public bool? forceUpdate { get; set; }
        public bool? setDirtyOnly { get; set; }
        public string watchlistId { get; set; }
        public string watchlistObjectName { get; set; }
    }
    public class FormRequest : Request
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "formservice";
        public static readonly string Create = "C";
        public static readonly string Read = "R";
        public static readonly string Update = "U";
        public static readonly string Delete = "D";
        public string formServiceAction { get; set; }
        public string stopOnWarning { get; set; }
        public string queryObjectName { get; set; }
        public IEnumerable<Input> formInputs { get; set; }
        public IEnumerable<Action> formActions { get; set; }
    }
    public class StackFormRequest : Request
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "appstack";
        public static readonly string open = "open";
        public static readonly string execute = "execute";
        public static readonly string close = "close";
        public string action { get; set; }
        public FormRequest formRequest { get; set; }
        public ActionRequest actionRequest { get; set; }
        public int stackId { get; set; }
        public int stateId { get; set; }
        public string rid { get; set; }
    }
    public class DatabrowserRequest : Request
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "dataservice";
        public static readonly string table = "table";
        public static readonly string view = "view";
        public static readonly string BROWSE = "BROWSE";
        public static readonly string AGGREGATION = "AGGREGATION";
        public string targetName { get; set; }
        public string targetType { get; set; }
        public string dataServiceType { get; set; }
        public bool? enableNextPageProcessing { get; set; }
        public string nextPageTimeInterval { get; set; }
        public bool? batchDataRequest { get; set; }
        public IEnumerable<Condition> having { get; set; }
        public IEnumerable<DatabrowserRequest> dataRequests { get; set; }
    }
    public class BatchformRequest : Request
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "batchformservice";
        public IEnumerable<FormRequest> formRequests { get; set; }
    }
    public class Predicate
    {
        public static readonly string SINGLE = "SINGLE";
        public static readonly string LIST = "LIST";
        public static readonly string RANGE = "RANGE";
        public string literalType { get; set; }
        public string[] values { get; set; }
    }
    public class Subject
    {
        public string view { get; set; }
        public string dictItem { get; set; }
        public string table { get; set; }
    }
    public class Criteria
    {
        public Subject subject { get; set; }
        public Predicate predicate { get; set; }
        public string comparisonType { get; set; }
    }
    public class DataSelection
    {
        public Criteria[] criteria { get; set; }
    }
    public class OrderByColumn
    {
        public string dictItem { get; set; }
        public string table { get; set; }
        public string direction { get; set; }
        public bool levelBreak { get; set; }
        public bool pageBreak { get; set; }
        public int seq { get; set; }
    }
    public class DataSequence
    {
        public OrderByColumn[] orderByColumns { get; set; }
    }
    public abstract class UBERequest : Request
    {
        public string reportName { get; set; }
        public string reportVersion { get; set; }
        public bool fireAndForget { get; set; }
        public string jasserver { get; set; }
        public bool jdeDebugLog { get; set; }
        public bool jdeLog { get; set; }
        public int reportLoggingLevel { get; set; }
        public string queueName { get; set; }
        public Value[] poValues { get; set; }
        public Value[] riValues { get; set; }
        public DataSelection dataSelection { get; set; }
        public DataSequence dataSequence { get; set; }
    }
    public class DiscoverUBERequest : UBERequest
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "report/discover";
    }
    public class LaunchUBERequest : UBERequest
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "report/execute";
    }
    public class StatusUBERequest : Request
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "report/status";
        public int jobNumber { get; set; }
    }
    public abstract class MoRequest : Request
    {
        public string moStructure { get; set; }
        public string[] moKey { get; set; }
        public string inputText { get; set; }
        public bool? appendText { get; set; }
        public bool? includeURLs { get; set; }
        public bool? includeData { get; set; }
        public string[] moTypes { get; set; }
        public string[] extensions { get; set; }
        public int? thumbnailSize { get; set; }
        public int? height { get; set; }
        public int? width { get; set; }
        public string fileName { get; set; }
        public string fileLocation { get; set; }
        public string itemName { get; set; }
        public int? sequence { get; set; }
        public bool? multipleMode { get; set; }
        public string thumbFileLocation { get; set; }
        public string downloadURL { get; set; }
        public string urlText { get; set; }
    }
    public class FileAttachment
    {
        public string fileName { get; set; }
        public string fileLocation { get; set; }
        public string itemName { get; set; }
        public int? sequence { get; set; }
    }
    public class MoDelete : MoRequest
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "file/delete";
    }
    public class MoAddUrl : MoRequest
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "file/addurl";
    }
    public class MoAddText : MoRequest
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "file/addtext";
    }
    public class MoGetText : MoRequest
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "file/gettext";
    }
    public class MoUpdateText : MoRequest
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "file/updatetext";
    }
    public class MoList : MoRequest
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "file/list";
    }
    public class MoUpload : MoRequest
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "file/upload";
        public FileAttachment file { get; set; }
    }
    public class MoDownload : MoRequest
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "file/download";
    }
    public class AuthRequest : Service
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "tokenrequest";
        public string requiredCapabilities { get; set; }
    }
    public class TokenValidationRequest : Service
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "tokenrequest/validate";
        public bool touch { get; set; }
    }
    public class LogoutRequest : Service
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "tokenrequest/logout";
    }
    public class DefaultConfig : Service
    {
        [JsonIgnore]
        public override string SERVICE { get; } = "defaultconfig";
    }
}
