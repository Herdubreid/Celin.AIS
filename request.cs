using System.Collections.Generic;
using Newtonsoft.Json;
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
        public Grid gridAction { get; set; }
    }
    public class FormAction : Action
    {
        public string controlID { get; set; }
        public string command { get; set; }
        public string value { get; set; }
    }
    public class Value
    {
        public string content { get; set; }
        public string specialValueId { get; set; }
    }
    public class ComplexQuery
    {
        public Query query { get; set; }
        public string andOr { get; set; }
    }
    public class Condition
    {
        public Value[] value { get; set; }
        public string controlId { get; set; }
        public string @operator { get; set; }
        public string aggregation { get; set; }
    }
    public class Query
    {
        public IEnumerable<ComplexQuery> complexQuery { get; set; }
        public IEnumerable<Condition> condition { get; set; }
        public bool? autoFind { get; set; }
        public string matchType { get; set; }
        public bool? autoClear { get; set; }
    }
    public class AggregationItem
    {
        public string aggregation { get; set; }
        public string column { get; set; }
        public string direction { get; set; }
        public string specialHandling { get; set; }
    }
    public class Aggregation
    {
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
        [JsonIgnore]
        public abstract string SERVICE { get; }
    }
    public abstract class Request : Service
    {
        public string formName { get; set; }
        public string version { get; set; }
        public string token { get; set; }
        public string deviceName { get; set; }
        public string findOnEntry { get; set; }
        public string returnControlIDs { get; set; }
        public string maxPageSize { get; set; }
        public bool? aliasNaming { get; set; }
        public Query query { get; set; }
        public string outputType { get; set; }
        public string formServiceDemo { get; set; }
    }
    public class FormRequest : Request
    {
        public override string SERVICE { get; } = "formservice";
        public string formServiceAction { get; set; }
        public string stopOnWarning { get; set; }
        public string queryObjectName { get; set; }
        public IEnumerable<Input> formInputs { get; set; }
        public IEnumerable<Action> formActions { get; set; }
    }
    public class StackFormRequest : Request
    {
        public override string SERVICE { get; } = "appstack";
        public string action { get; set; }
        public FormRequest formRequest { get; set; }
        public ActionRequest actionRequest { get; set; }
        public int stackId { get; set; }
        public int stateId { get; set; }
        public string rid { get; set; }
    }
    public class DatabrowserRequest : Request
    {
        public override string SERVICE { get; } = "dataservice";
        public string targetName { get; set; }
        public string targetType { get; set; }
        public string dataServiceType { get; set; }
        public Aggregation aggregation { get; set; }
        public IEnumerable<Condition> having { get; set; }
        public bool? batchDataRequest { get; set; }
        public IEnumerable<DatabrowserRequest> dataRequests { get; set; }
    }
    public class BatchformRequest : Service
    {
        public override string SERVICE { get; } = "batchformservice";
        public IEnumerable<FormRequest> formRequests { get; set; }
    }
    public abstract class MoRequest : Request
    {
        public string moStructure { get; set; }
        public string[] moKey { get; set; }
        public string inputText { get; set; }
        public bool appendText { get; set; }
        public bool includeURLs { get; set; }
        public bool includeData { get; set; }
        public string[] moTypes { get; set; }
        public string[] extensions { get; set; }
        public string thumbnailSize { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string fileName { get; set; }
        public string fileLocation { get; set; }
        public string itemName { get; set; }
        public int sequence { get; set; }
        public string thumbFileLocation { get; set; }
        public string downloadURL { get; set; }
    }
    public class MoGetText : MoRequest
    {
        public override string SERVICE { get; } = "file/gettext";
    }
    public class MoUpdateText : MoRequest
    {
        public override string SERVICE { get; } = "file/updatetext";
    }
    public class MoList : MoRequest
    {
        public override string SERVICE { get; } = "file/list";
    }
    public class MoUpload : MoRequest
    {
        public override string SERVICE { get; } = "file/upload";
    }
    public class MoDownload : MoRequest
    {
        public override string SERVICE { get; } = "file/download";
    }
    public class AuthRequest : Service
    {
        public override string SERVICE { get; } = "tokenrequest";
        public string username { get; set; }
        public string password { get; set; }
        public string deviceName { get; set; }
        public string requiredCapabilities { get; set; }
    }
    public class LogoutRequest : Service
    {
        public string token { get; set; }
        public override string SERVICE { get; } = "tokenrequest/logout";
    }
    public class DefaultConfig : Service
    {
        public override string SERVICE { get; } = "defaultconfig";
    }
}
