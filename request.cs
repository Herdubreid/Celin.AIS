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
        public List<ColumnEvent> gridColumnEvents { get; set; } = new List<ColumnEvent>();
    }
    public class Grid
    {
        public string gridID { get; set; }
    }
    public class GridInsert : Grid
    {
        public List<RowEvent> gridRowInsertEvents { get; set; } = new List<RowEvent>();
    }
    public class GridUpdate : Grid
    {
        public List<RowEvent> gridRowUpdateEvents { get; set; } = new List<RowEvent>();
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
    public class Condition
    {
        public Value[] value { get; set; }
        public string controlId { get; set; }
        public string @operator { get; set; }
    }
    public class Query
    {
        public List<Condition> condition { get; set; } = new List<Condition>();
        public bool autoFind { get; set; }
        public string matchType { get; set; }
        public bool autoClear { get; set; }
    }
    public class ActionRequest
    {
        public string returnControlIDs { get; set; }
        public List<Action> formActions { get; set; } = new List<Action>();
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
        public bool aliasNaming { get; set; }
        public Query query { get; set; }
        public string outputType { get; set; }
    }
    public class FormRequest : Request
    {
        public override string SERVICE { get; } = "formservice";
        public string formServiceAction { get; set; }
        public string stopOnWarning { get; set; }
        public string queryObjectName { get; set; }
        public List<Input> formInputs { get; set; } = new List<Input>();
        public List<Action> formActions { get; set; } = new List<Action>();
    }
    public class StackFormRequest : Request
    {
        public override string SERVICE { get; } = "appstack";
        public string action { get; set; }
        public FormRequest formRequest { get; set; }
        public ActionRequest actionRequest { get; set; } = new ActionRequest();
        public int stackId { get; set; }
        public int stateId { get; set; }
        public string rid { get; set; }
    }
    public class DatabrowserRequest : Request
    {
        public override string SERVICE { get; } = "dataservice";
        public string targetName { get; set; }
        public string targetType { get; set; }
        public string dataServiceType;
    }
    public class BatchformRequest : Service
    {
        public override string SERVICE { get; } = "batchformservice";
        public List<FormRequest> formRequests { get; set; } = new List<FormRequest>();
        public string token { get; set; }
        public string deviceName { get; set; }
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
        public override string SERVICE { get; } = "tokenrequest/logout";
    }
    public class DefaultConfig : Service
    {
        public override string SERVICE { get; } = "defaultconfig";
    }
}
