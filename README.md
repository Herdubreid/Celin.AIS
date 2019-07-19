# Celin.AIS

Celin.AIS is a .NET Standard 2.1 Library for Oracle E1/JDE AIS.

## Install

Install NuGet package or download the source from Github.

## Usage

1. Initialise a new `Celin.AIS.Server` instance with the AIS Url (note the trailing '/').
2. Populate the `AuthRequest` member of the instance.
3. Call the `Authenticate` method.

```csharp
// Initalise E1 Server
var e1 = new Server("http://e1.celin.io:9300/jderest/");
// Set the authentication parameters
e1.AuthRequest.deviceName = "aisTest";
e1.AuthRequest.username = "demo";
e1.AuthRequest.password = "testing";

try
{
	// Authenticate
	await e1.AuthenticateAsync();
	// Success...
```

### Return Generic Response Type

Once a server instance has been authenticated, it can be used to make a form request.
1. Create a new `Celin.AIS.FormRequest` with the form's definition.
2. Add any `Celin.AIS.FormAction` to the `formActions` member.
3. Submit the instance to the `Request` method along with the desired response type (a generic `Newtonsoft.Json.Linq.JObject` in the below example). 

```csharp
	// Create an AB Form Request
	var ab = new FormRequest()
	{
		formName = "P01012_W01012B",
		version = "ZJDE0001",
		formServiceAction = "R",
		maxPageSize = "10",
		formActions = new List<Celin.AIS.Action>()
	};
	// Create Form Actions
	ab.formActions = new[]
	{
		// Set the Search Type to "C"
		new FormAction() { controlID = "54", command = "SetControlValue", value = "C" },
		// Press the Find Button
		new FormAction() { controlID = "15", command = "DoAction" }
	};

	// Submit the Form Request with a Generic Response Object
	var genRsp =  await e1.RequestAsync<JObject>(ab);
	// Request successful, dumpt the output to the Console
	Console.WriteLine(genRsp);
```

### Return a Custom Response Type

Even though generic response types like `JObject` can be queried for the requested data, another approach is to write custom classes that encapsulate the response into code-friendly properties.

The above example opens the `Work With Address Book` form, sets the `Search Type` to `C` and presses the `Find Button` for a max return of 10 grid rows.  
If we are ony interested in the address book number and name in the returned rows, we can write a class to encapsulate it.

```csharp
using Celin.AIS;
namespace aisTest
{
    public class AddressBookRow : Row
    {
        public Number mnAddressNumber_19 { get; set; }
        public String sAlphaName_20 { get; set; }
    }
    public class AddressBookForm : FormResponse
    {
        public Form<FormData<AddressBookRow>> fs_P01012_W01012B;
    }
}
```

The `AddressBookForm` class inherits `Celin.AIS.FormResponse` and has only one property, the form name prefixed with `fs_`.  
The type is `Celin.AIS.Form<Celin.AIS.FormData<AddressBookRow>>` where `AddressBookRow` is our grid row custom class, and inhertis `Celin.AIS.Row`.  
The `AddressBookRow` parameter names of `mnAddressNumber_19` and `sAlphaName_20` can either be deducted from variable type, name and ID in the Tools Designer, or by a generice dump like in the previous example.  
We know that address number is numeric so we can use `Celin.AIS.Number` as its type and `Celin.AIS.String` for name.

Since we are only interested in Ids 19 and 20 from the grid, we can suppress reduntant data by setting the `returnControlIDs` parameter to `1[19,20]`.

```csharp
	// Limit the response to Grid Columns Number and Name
	ab.returnControlIDs = "1[19,20]";

	// Submit the form Request with our AB class definition
	var abRsp = await e1.RequestAsync<AddressBookForm>(ab);
	// Print the Grid Items to the Console
	foreach (var r in abRsp.fs_P01012_W01012B.data.gridData.rowset)
	{
		Console.WriteLine("{0, 12} {1}", r.mnAddressNumber_19.value, r.sAlphaName_20.value);
	}
}
catch (Exception e)
{
	Console.WriteLine("Failed!\n", e.Message);
}
```

The example above can be downloaded from [Github](https://github.com/Herdubreid/aisTest/tree/master).

## License

MIT © [Bragason and Associates Pty Ltd](fbragason@outlook.com)
