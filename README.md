# SpreadSheetToScriptableObject

**SpreadSheetToScriptableObject** is a Unity library that integrates with Google Spreadsheets to automatically generate C# code and store spreadsheet data as `ScriptableObject` assets. It simplifies data-driven workflows by removing the need for manual data entry and boilerplate code.

## Features

- Connects to Google Spreadsheets using API Key
- Parses spreadsheet data into C# class definitions
- Automatically generates ScriptableObjects from the data
- Provides an intuitive Unity Editor interface for setup and execution

## Installation

1. Clone this repository or [download the latest `.unitypackage`](https://github.com/hibisceae/SpreadSheetToScriptableObject/releases/latest) and import it into your Unity project.
2. Enable the **Google Sheets API** in your [Google Cloud Console](https://console.cloud.google.com/).
3. Generate an API Key and prepare your spreadsheet for public or API access.

## Getting Started

1. In the Unity Project window, **right-click** and select  
   `SpreadSheetToScriptableObject/SpreadSheetSetting` to create a new settings asset.
2. Configure the following fields in the settings asset:
   - **Namespace** for the generated code (optional)
   - **Code Output Path**: where the generated `.cs` files will be saved
   - **Asset Output Path**: where the `ScriptableObject` assets will be stored
3. Enter your **Google Spreadsheet ID**.
4. Enter your **Google API Key**.
5. Add sheet names to **SheetNames** for the sheets you want to import.
6. Click **"Generate Code"** to create the data class based on the sheet structure.
7. Click **"Generate/Sync Asset"** to create ScriptableObjects from the spreadsheet data.

> If the spreadsheet **data changes**, just click **Generate/Sync Asset** again to update the existing assets.  
> If the **structure** (columns, types) of the sheet changes, you must run **Generate Code** first to regenerate the class definitions.

## Spreadsheet Format Requirement

To work properly with this library, your Google Spreadsheet must follow a specific structure.

- The **first row** should define the field names (column headers).
- The **second row** must define the **data types** for each field.
- All rows after that are treated as **data entries**.
- The first column must always be named `ID` and have the type `Int32`. This is used as the unique key.

### ✅ Required Format Example

| ID     | Name     | Attack | Deffence | Lore          |
|--------|----------|--------|----------|---------------|
| Int32  | String   | Int32  | Int32    | String        |
| 10001  | Zombie   | 100    | 100      | Zombie Lore   |
| 10002  | Skeleton | 100    | 100      | Skeleton Lore |
| 10003  | Pig      | 100    | 100      | Pig Lore      |
| 10004  | Cow      | 100    | 100      | Cow Lore      |
| 10005  | Sheep    | 100    | 100      | Sheep Lore    |

Make sure that:
- The **column names** match your data fields.
- The **types** are supported (`Int32`, `String`, `Float`, etc.).
- Every ID is unique and present.

This structure ensures that the code generation and asset creation will function correctly.

## Example

Usage Example
Once you’ve generated the code and assets, you can use the data immediately in your scripts.

1. Retrieve the DataTable
Use the following code to get the DataTable<T> instance for your spreadsheet-derived class:

```cs
DataTable<TestTableRow> testTable = DataTableLoader<TestTableRow>.GetTable();
```
2. Access the Data
You can then access rows by their key:

```cs
TestTableRow row = testTable[10001];
Debug.Log(row.ID);
Debug.Log(row.Attack);
Debug.Log(row.Deffence);
```

That’s it — you now have fully typed data from your spreadsheet available as ScriptableObject assets in your Unity project.

## Notes

- Make sure your spreadsheet is accessible via API Key (e.g., published or shared properly).

## License

This project is licensed under the MIT License.  
